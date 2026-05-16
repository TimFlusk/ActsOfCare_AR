using System.IO;
using UnityEditor;
using UnityEngine;

namespace ActsOfCare.Scripts.Editor
{
    /// <summary>
    /// Converts all PNGs in a selected folder into Unlit/Transparent materials.
    /// Opens via: Tools > Create Unlit Materials From Textures
    /// 
    /// For each PNG found:
    /// <list type="number">
    ///   <item>Imports it as a texture (not sprite) with alpha transparency.</item>
    ///   <item>Creates a Material using the Universal Render Pipeline (URP) Unlit shader.
    ///      with alpha blending — falls back to Legacy Transparent/Diffuse if URP not present</item>
    ///   <item>Assigns the texture to the material's _BaseMap (URP) / _MainTex (Legacy)</item>
    ///   <item>Saves the material as a .mat file alongside the texture</item>
    /// </list>
    /// </summary>
    public class CreateUnlitMaterials : EditorWindow
    {
        private string _folderPath    = "";
        private bool   _reimport      = true;
        private Vector2 _scroll;

        // Shader name options in priority order
        private static readonly string[] ShaderPriority = new[]
        {
            "Universal Render Pipeline/Unlit",   // URP
            "Unlit/Transparent",                 // Legacy
            "Hidden/Universal Render Pipeline/FallbackError", // fallback signal
        };

        [MenuItem("Tools/Create Unlit Materials From Textures")]
        public static void Open() =>
            GetWindow<CreateUnlitMaterials>("Unlit Material Creator");

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.Space(8);
            GUILayout.Label("Unlit Material Creator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Creates an Unlit/Transparent material for every PNG in the selected folder. " +
                "Materials are saved as .mat files in the same folder as the textures.",
                MessageType.Info);

            EditorGUILayout.Space(6);

            EditorGUILayout.LabelField("Texture Folder", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _folderPath = EditorGUILayout.TextField(_folderPath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string chosen = EditorUtility.OpenFolderPanel("Select PNG folder", "Assets", "");
                if (!string.IsNullOrEmpty(chosen))
                    _folderPath = chosen.StartsWith(Application.dataPath)
                        ? "Assets" + chosen.Substring(Application.dataPath.Length)
                        : chosen;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
            _reimport = EditorGUILayout.Toggle("Re-import textures (ensure alpha)", _reimport);

            EditorGUILayout.Space(10);

            GUI.enabled = !string.IsNullOrEmpty(_folderPath);
            if (GUILayout.Button("Create Materials", GUILayout.Height(36)))
                CreateMaterials();
            GUI.enabled = true;

            EditorGUILayout.EndScrollView();
        }

        private void CreateMaterials()
        {
            if (!Directory.Exists(_folderPath))
            {
                EditorUtility.DisplayDialog("Error", $"Folder not found:\n{_folderPath}", "OK");
                return;
            }

            string[] pngPaths = Directory.GetFiles(_folderPath, "*.png",
                SearchOption.TopDirectoryOnly);
            if (pngPaths.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No PNG files found.", "OK");
                return;
            }

            // Find best available shader
            Shader shader = FindShader();
            if (shader == null)
            {
                EditorUtility.DisplayDialog("Error",
                    "Could not find a suitable Unlit shader. " +
                    "Make sure URP is installed, or that Legacy shaders are available.", "OK");
                return;
            }

            bool isURP = shader.name.Contains("Universal Render Pipeline");
            Debug.Log($"[UnlitMaterials] Using shader: {shader.name}");

            // Optionally reimport textures to ensure correct alpha settings
            if (_reimport)
            {
                AssetDatabase.StartAssetEditing();
                foreach (string p in pngPaths)
                    ConfigureTexture(AbsToAsset(p));
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }

            int created = 0;
            int skipped = 0;

            AssetDatabase.StartAssetEditing();
            foreach (string absPath in pngPaths)
            {
                string assetPath = AbsToAsset(absPath);
                string matPath   = Path.ChangeExtension(assetPath, ".mat");

                // Skip if material already exists
                if (File.Exists(matPath.Replace("Assets/", Application.dataPath + "/")))
                {
                    Debug.Log($"[UnlitMaterials] Skipping — material already exists: {matPath}");
                    skipped++;
                    continue;
                }

                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                if (tex == null)
                {
                    Debug.LogWarning($"[UnlitMaterials] Could not load texture: {assetPath}");
                    continue;
                }

                var mat = new Material(shader);
                mat.name = Path.GetFileNameWithoutExtension(assetPath);

                if (isURP)
                {
                    // URP Unlit: set surface type to Transparent
                    mat.SetFloat("_Surface", 1);          // 0 = Opaque, 1 = Transparent
                    mat.SetFloat("_Blend", 0);             // Alpha blend
                    mat.SetFloat("_AlphaClip", 0);
                    mat.SetFloat("_ZWrite", 0);
                    mat.SetTexture("_BaseMap", tex);
                    mat.SetColor("_BaseColor", Color.white);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.SetOverrideTag("RenderType", "Transparent");
                }
                else
                {
                    // Legacy Unlit/Transparent
                    mat.SetTexture("_MainTex", tex);
                    mat.SetColor("_Color", Color.white);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }

                AssetDatabase.CreateAsset(mat, matPath);
                created++;
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();

            string msg = $"Created {created} material(s).";
            if (skipped > 0) msg += $"\nSkipped {skipped} (already existed).";
            EditorUtility.DisplayDialog("Done", msg, "OK");
            Debug.Log($"[UnlitMaterials] {msg}");
        }

        private static void ConfigureTexture(string assetPath)
        {
            var imp = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (imp == null) return;

            imp.textureType         = TextureImporterType.Default;   // NOT Sprite — we want a Texture2D
            imp.alphaIsTransparency = true;
            imp.alphaSource         = TextureImporterAlphaSource.FromInput;
            imp.sRGBTexture         = true;
            imp.filterMode          = FilterMode.Bilinear;
            imp.textureCompression  = TextureImporterCompression.Uncompressed;
            imp.wrapMode            = TextureWrapMode.Clamp;
            imp.SaveAndReimport();
        }

        private static Shader FindShader()
        {
            foreach (string name in ShaderPriority)
            {
                Shader s = Shader.Find(name);
                if (s != null && !s.name.Contains("FallbackError"))
                    return s;
            }
            return null;
        }

        private static string AbsToAsset(string abs) =>
            abs.StartsWith(Application.dataPath)
                ? "Assets" + abs.Substring(Application.dataPath.Length).Replace('\\', '/')
                : abs.Replace('\\', '/');
    }
}