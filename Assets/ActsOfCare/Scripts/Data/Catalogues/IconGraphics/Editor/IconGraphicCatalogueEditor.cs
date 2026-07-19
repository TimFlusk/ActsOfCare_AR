#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace ActsOfCare.Data.Catalogues.IconGraphics.Editor
{
    [CustomEditor(typeof(IconGraphicCatalogue))]
    public class IconGraphicCatalogueEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Populate From Folder", EditorStyles.boldLabel);

            var folderPathProp = serializedObject.FindProperty("sourceFolderPath");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(folderPathProp, new GUIContent("Folder"));
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                var selected = EditorUtility.OpenFolderPanel("Select Icon Folder", folderPathProp.stringValue, "");
                if (!string.IsNullOrEmpty(selected))
                    folderPathProp.stringValue = "Assets" + selected.Substring(Application.dataPath.Length);
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Populate Icons From Folder"))
            {
                PopulateFromFolder((IconGraphicCatalogue)target, folderPathProp.stringValue);
            }
        }

        private void PopulateFromFolder(IconGraphicCatalogue catalogue, string path)
        {
            var textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { path });
            var assetPaths = textureGuids.Select(AssetDatabase.GUIDToAssetPath).ToList();

            int reimportedCount = 0;

            foreach (var assetPath in assetPaths)
            {
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer == null) continue;

                if (importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                    reimportedCount++;
                }
            }

            if (reimportedCount > 0)
            {
                Debug.Log($"[IconGraphicCatalogue] Re-imported {reimportedCount} texture(s) as Sprite.");
                AssetDatabase.Refresh();
            }

            var sprites = assetPaths
                .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
                .Where(s => s != null)
                .OrderBy(s => s.name)
                .ToList();

            var serializedObj = new SerializedObject(catalogue);
            var iconsProperty = serializedObj.FindProperty("icons");

            iconsProperty.ClearArray();

            for (int i = 0; i < sprites.Count; i++)
            {
                iconsProperty.InsertArrayElementAtIndex(i);
                var element = iconsProperty.GetArrayElementAtIndex(i);

                var imageProp = element.FindPropertyRelative("<Image>k__BackingField");
                var idProp    = element.FindPropertyRelative("<Id>k__BackingField");

                imageProp.objectReferenceValue = sprites[i];
                idProp.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sprites[i]));
            }

            serializedObj.ApplyModifiedProperties();

            Debug.Log($"[IconGraphicCatalogue] Populated {sprites.Count} icon(s) from {path}");
        }
    }
}
#endif