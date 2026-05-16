using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ActsOfCare.Scripts.Editor
{
    /// <summary>
    /// Editor tool: builds a 2D cutout puppet hierarchy from a folder of PNGs.
    /// Naming convention expected: ##_PartName.png (e.g. 11_Head.png, 06_Torse.png)
    /// Opens via: Tools > Cutout Puppet Builder
    /// </summary>
    public class CutOutPuppetBuilder : EditorWindow
    {
        // ── Inspector state ────────────────────────────────────────────────────
        private string _folderPath = "";
        private string _puppetName = "WrestlerPuppet";
        private float  _pixelsPerUnit = 100f;
        private bool   _inPlace       = true;
        private bool   _matchScale    = false;
        private Vector2 _scroll;

        // ── Pivot table ────────────────────────────────────────────────────────
        // For each recognised body-part keyword we store:
        //   • pivot anchor  (0–1 UV, applied when we create the sprite)
        //   • sort order    (higher = drawn in front)
        //   • parent keyword (which part this hangs from in the hierarchy)
        private struct PartConfig
        {
            public Vector2 Pivot;        // sprite pivot in 0-1 UV space
            public int     SortOrder;
            public string  ParentKey;    // null = child of root
        }

        // Keys are lowercase, matched against the filename after stripping the number prefix.
        private static readonly Dictionary<string, PartConfig> PartTable =
            new Dictionary<string, PartConfig>
            {
                // ── Core ──────────────────────────────────────────────────────────
                { "torse",         new PartConfig { Pivot = new Vector2(0.5f, 0.5f), SortOrder = 10, ParentKey = null } },
                { "torso",         new PartConfig { Pivot = new Vector2(0.5f, 0.5f), SortOrder = 10, ParentKey = null } },

                // ── Head ──────────────────────────────────────────────────────────
                { "head",          new PartConfig { Pivot = new Vector2(0.5f, 0f),   SortOrder = 20, ParentKey = "torse" } },

                // ── Shoulders (upper arm) — pivot at shoulder = TOP of the sprite ─
                { "leftshoulder",  new PartConfig { Pivot = new Vector2(1f,   1f),   SortOrder = 8,  ParentKey = "torse" } },
                { "rightshoulder", new PartConfig { Pivot = new Vector2(0f,   1f),   SortOrder = 8,  ParentKey = "torse" } },

                // ── Arms (forearm + hand) — pivot at elbow = proximal end ─────────
                { "leftarm",       new PartConfig { Pivot = new Vector2(1f,   0.5f), SortOrder = 7,  ParentKey = "leftshoulder" } },
                { "rightarm",      new PartConfig { Pivot = new Vector2(0f,   0.5f), SortOrder = 7,  ParentKey = "rightshoulder" } },

                // ── Thighs — pivot at hip = top ───────────────────────────────────
                { "leftthigh",     new PartConfig { Pivot = new Vector2(0.5f, 1f),   SortOrder = 9,  ParentKey = "torse" } },
                { "rightthigh",    new PartConfig { Pivot = new Vector2(0.5f, 1f),   SortOrder = 9,  ParentKey = "torse" } },

                // ── Feet/lower legs — pivot at knee = top ─────────────────────────
                { "leftfoot",      new PartConfig { Pivot = new Vector2(0.5f, 1f),   SortOrder = 6,  ParentKey = "leftthigh" } },
                { "rightfoot",     new PartConfig { Pivot = new Vector2(0.5f, 1f),   SortOrder = 6,  ParentKey = "rightthigh" } },
            };

        // ── Menu entry ─────────────────────────────────────────────────────────
        [MenuItem("Tools/Cutout Puppet Builder")]
        public static void Open() =>
            GetWindow<CutOutPuppetBuilder>("Cutout Puppet Builder");

        // ── GUI ────────────────────────────────────────────────────────────────
        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.Space(8);
            GUILayout.Label("Cutout Puppet Builder", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Point to a folder containing your exported PNGs (named ##_PartName.png). " +
                "The tool will import them as sprites and build a quad hierarchy ready for " +
                "the Real-Time Full Body Tracking System.",
                MessageType.Info);

            EditorGUILayout.Space(6);

            // ── Folder picker ──────────────────────────────────────────────────
            EditorGUILayout.LabelField("Texture Folder", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _folderPath = EditorGUILayout.TextField(_folderPath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string chosen = EditorUtility.OpenFolderPanel(
                    "Select PNG folder", "Assets", "");
                if (!string.IsNullOrEmpty(chosen))
                {
                    // Convert absolute path → project-relative
                    if (chosen.StartsWith(Application.dataPath))
                        _folderPath = "Assets" + chosen.Substring(Application.dataPath.Length);
                    else
                        _folderPath = chosen;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            // ── Settings ───────────────────────────────────────────────────────
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            _puppetName    = EditorGUILayout.TextField("Root Object Name", _puppetName);
            _pixelsPerUnit = EditorGUILayout.FloatField("Pixels Per Unit", _pixelsPerUnit);

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("RTFBT PoseTracking Settings", EditorStyles.boldLabel);
            _inPlace    = EditorGUILayout.Toggle("In Place (lock Z movement)", _inPlace);
            _matchScale = EditorGUILayout.Toggle("Match Scale", _matchScale);

            EditorGUILayout.Space(10);

            // ── Build button ───────────────────────────────────────────────────
            GUI.enabled = !string.IsNullOrEmpty(_folderPath);
            if (GUILayout.Button("Build Puppet Hierarchy", GUILayout.Height(36)))
                BuildHierarchy();
            GUI.enabled = true;

            EditorGUILayout.Space(6);
            EditorGUILayout.HelpBox(
                "After building:\n" +
                "1. Open an RTFBT demo scene and note the exact field names on PoseTracking.\n" +
                "2. Assign the Skeleton Parent on PoseTracking to the root GameObject.\n" +
                "3. Map each bone Transform to the matching body-part GameObject.\n" +
                "4. Set In Place = true and Match Scale = false on PoseTracking.",
                MessageType.None);

            EditorGUILayout.EndScrollView();
        }

        // ── Core builder ───────────────────────────────────────────────────────
        private void BuildHierarchy()
        {
            // 1. Find PNGs
            if (!Directory.Exists(_folderPath) &&
                !Directory.Exists(Path.Combine(Application.dataPath,
                    _folderPath.Replace("Assets/", ""))))
            {
                EditorUtility.DisplayDialog("Error",
                    $"Folder not found:\n{_folderPath}", "OK");
                return;
            }

            string[] pngPaths = Directory.GetFiles(_folderPath, "*.png",
                SearchOption.TopDirectoryOnly);
            if (pngPaths.Length == 0)
            {
                EditorUtility.DisplayDialog("Error",
                    "No PNG files found in that folder.", "OK");
                return;
            }

            // 2. Import / re-import each PNG as a Sprite
            AssetDatabase.StartAssetEditing();
            foreach (string absPath in pngPaths)
            {
                string assetPath = AbsToAsset(absPath);
                ConfigureSpriteImport(assetPath);
            }
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();

            // 3. Load sprites and build a lookup by normalised part key
            var sprites = new Dictionary<string, (Sprite sprite, string filename)>();
            foreach (string absPath in pngPaths)
            {
                string assetPath = AbsToAsset(absPath);
                Sprite  sprite   = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite == null) continue;

                string key = NormaliseKey(Path.GetFileNameWithoutExtension(absPath));
                sprites[key] = (sprite, Path.GetFileNameWithoutExtension(absPath));
            }

            // 4. Create root GameObject
            var root = new GameObject(_puppetName);
            Undo.RegisterCreatedObjectUndo(root, "Create Puppet Root");

            // Add a stub MonoBehaviour marker so we can document the RTFBT wiring.
            // (Replace with AddComponent<PoseTracking>() once you've confirmed
            //  the exact class name from the asset.)
            var marker = root.AddComponent<PuppetRTFBTMarker>();
            marker.InPlace    = _inPlace;
            marker.MatchScale = _matchScale;
            marker.Note       = "Replace this component with PoseTracking from RTFBT " +
                                "and assign bone transforms to the child GameObjects below.";

            // 5. Build parts — two passes:
            //    Pass A: create all GameObjects
            //    Pass B: parent them correctly
            var created = new Dictionary<string, GameObject>();

            foreach (var kvp in sprites)
            {
                string key            = kvp.Key;
                (Sprite sprite, string filename) = kvp.Value;

                if (!PartTable.TryGetValue(key, out PartConfig cfg))
                {
                    Debug.LogWarning($"[PuppetBuilder] No config for key '{key}' " +
                                     $"(file: {filename}.png) — creating unparented.");
                    cfg = new PartConfig
                        { Pivot = new Vector2(0.5f, 0.5f), SortOrder = 0, ParentKey = null };
                }

                GameObject go = CreatePartObject(filename, sprite, cfg);
                created[key] = go;
            }

            // Pass B — parent
            foreach (var kvp in created)
            {
                string key = kvp.Key;
                GameObject go = kvp.Value;

                if (!PartTable.TryGetValue(key, out PartConfig cfg) ||
                    cfg.ParentKey == null)
                {
                    go.transform.SetParent(root.transform, false);
                    continue;
                }

                // Try exact parent key, then "torse"/"torso" fallback
                string parentKey = cfg.ParentKey;
                if (!created.TryGetValue(parentKey, out GameObject parent))
                {
                    // Normalise: torse↔torso
                    string alt = parentKey == "torse" ? "torso" : "torse";
                    if (!created.TryGetValue(alt, out parent))
                    {
                        Debug.LogWarning($"[PuppetBuilder] Parent '{parentKey}' not found " +
                                         $"for '{key}' — attaching to root.");
                        parent = root;
                    }
                }
                go.transform.SetParent(parent.transform, false);
            }

            // 6. Select root in hierarchy
            Selection.activeGameObject = root;
            EditorGUIUtility.PingObject(root);

            Debug.Log($"[PuppetBuilder] Built '{_puppetName}' with {created.Count} parts.");
            EditorUtility.DisplayDialog("Done",
                $"Puppet '{_puppetName}' created with {created.Count} parts.\n\n" +
                "Next step: open an RTFBT demo scene, inspect PoseTracking, " +
                "then assign the bone Transforms to each child GameObject.",
                "OK");
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        /// <summary>Creates a GameObject with a SpriteRenderer for one body part.</summary>
        private static GameObject CreatePartObject(
            string filename, Sprite sprite, PartConfig cfg)
        {
            var go = new GameObject(filename);
            Undo.RegisterCreatedObjectUndo(go, "Create Puppet Part");

            var sr            = go.AddComponent<SpriteRenderer>();
            sr.sprite         = sprite;
            sr.sortingOrder   = cfg.SortOrder;

            // Transparent cutout material (Unity built-in Sprites-Default handles alpha)
            // If you have a custom ink-outline shader, assign it here instead.
            sr.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>(
                "Sprites-Default.mat");

            // Add a BoxCollider2D sized to sprite bounds (useful for interaction later)
            var col    = go.AddComponent<BoxCollider2D>();
            col.size   = sprite.bounds.size;
            col.offset = sprite.bounds.center;

            return go;
        }

        /// <summary>Sets sprite import settings: Sprite mode, pivot, PPU.</summary>
        private void ConfigureSpriteImport(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return;

            importer.textureType         = TextureImporterType.Sprite;
            importer.spriteImportMode    = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.spritePixelsPerUnit = _pixelsPerUnit;
            importer.filterMode          = FilterMode.Bilinear;
            importer.textureCompression  = TextureImporterCompression.Uncompressed;

            // Pivot: we'll override per-sprite in the sprite editor if needed,
            // but set a sensible default (bottom-centre) at import time.
            // Fine-grained pivot per part is handled via Transform offsets at runtime.
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            importer.SetTextureSettings(settings);

            importer.SaveAndReimport();
        }

        /// <summary>Strips leading digits and underscores, lowercases.</summary>
        private static string NormaliseKey(string filename)
        {
            // "02_LeftFoot" → "leftfoot"
            string stripped = filename.TrimStart('0','1','2','3','4','5','6','7','8','9');
            stripped = stripped.TrimStart('_');
            return stripped.Replace(" ", "").Replace("_", "").ToLowerInvariant();
        }

        private static string AbsToAsset(string absPath)
        {
            if (absPath.StartsWith(Application.dataPath))
                return "Assets" + absPath.Substring(Application.dataPath.Length)
                    .Replace('\\', '/');
            return absPath.Replace('\\', '/');
        }
    }
}

