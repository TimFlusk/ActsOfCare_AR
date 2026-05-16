using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextureScaler))]
public class TextureScalerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var scaler = (TextureScaler)target;

        // ── Universal Scale field ──────────────────────────────────────────
        EditorGUI.BeginChangeCheck();
        float newScale = EditorGUILayout.FloatField("Universal Scale", scaler.UniversalScale);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(scaler, "Change Universal Scale");
            scaler.UniversalScale = newScale;
        }

        EditorGUILayout.Space(6);

        // ── Read-only info panel ───────────────────────────────────────────
        var mr = scaler.GetComponent<MeshRenderer>();
        if (mr != null && mr.sharedMaterial != null)
        {
            Texture tex = GetTexture(mr.sharedMaterial);
            if (tex != null)
            {
                float aspect = (float)tex.width / tex.height;

                EditorGUILayout.LabelField("Texture Info", EditorStyles.boldLabel);
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.TextField("Name", tex.name);
                    EditorGUILayout.TextField("Resolution",
                        $"{tex.width} x {tex.height} px");
                    EditorGUILayout.TextField("Aspect Ratio", aspect.ToString("F4"));
                    EditorGUILayout.TextField("Result Scale",
                        $"({aspect * scaler.UniversalScale:F4},  " +
                        $"{scaler.UniversalScale:F4},  1)");
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Material found but no _BaseMap or _MainTex texture assigned.",
                    MessageType.Warning);
            }
        }
        else if (mr == null)
        {
            EditorGUILayout.HelpBox("No MeshRenderer on this GameObject.",
                MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox("MeshRenderer has no material assigned.",
                MessageType.Warning);
        }

        EditorGUILayout.Space(8);

        // ── Apply button ───────────────────────────────────────────────────
        GUI.enabled = mr != null &&
                      mr.sharedMaterial != null &&
                      GetTexture(mr.sharedMaterial) != null;

        if (GUILayout.Button("Apply Scale", GUILayout.Height(32)))
        {
            Undo.RecordObject(scaler.transform, "Apply Texture Scale");
            scaler.ApplyScale();
        }

        GUI.enabled = true;
    }

    private static Texture GetTexture(Material mat)
    {
        if (mat.HasProperty("_BaseMap"))
        {
            Texture t = mat.GetTexture("_BaseMap");
            if (t != null) return t;
        }
        if (mat.HasProperty("_MainTex"))
        {
            Texture t = mat.GetTexture("_MainTex");
            if (t != null) return t;
        }
        return null;
    }
}
