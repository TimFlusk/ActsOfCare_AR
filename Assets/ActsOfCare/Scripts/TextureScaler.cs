using UnityEngine;

/// <summary>
/// Sits on a GameObject with a MeshRenderer.
/// Reads the texture from the material (_BaseMap or _MainTex),
/// calculates its aspect ratio, and applies that to the transform
/// scaled by UniversalScale.
/// 
/// Use the "Apply Scale" button in the Inspector (custom Editor below).
/// </summary>
[DisallowMultipleComponent]
public class TextureScaler : MonoBehaviour
{
    [Tooltip("Uniform scale multiplier applied alongside the texture's aspect ratio.")]
    public float UniversalScale = 0.00333f;

    /// <summary>
    /// Finds the texture on this object's MeshRenderer material,
    /// then sets localScale to (aspectRatio * UniversalScale, UniversalScale, 1).
    /// </summary>
    public void ApplyScale()
    {
        var mr = GetComponent<MeshRenderer>();
        if (mr == null)
        {
            Debug.LogWarning($"[TextureScaler] No MeshRenderer found on '{name}'.");
            return;
        }

        Material mat = mr.sharedMaterial;
        if (mat == null)
        {
            Debug.LogWarning($"[TextureScaler] MeshRenderer on '{name}' has no material.");
            return;
        }

        Texture tex = GetTexture(mat);
        if (tex == null)
        {
            Debug.LogWarning($"[TextureScaler] No texture found on material '{mat.name}'. " +
                             "Looked for _BaseMap and _MainTex.");
            return;
        }

        float aspect = (float)tex.width / tex.height;
        transform.localScale = new Vector3(
            aspect * UniversalScale,
            UniversalScale,
            1f
        );

        Debug.Log($"[TextureScaler] '{name}' — texture: {tex.width}x{tex.height} " +
                  $"aspect: {aspect:F3}  scale: {transform.localScale}");
    }

    // ── Internal helpers ───────────────────────────────────────────────────

    private static Texture GetTexture(Material mat)
    {
        // URP / HDRP
        if (mat.HasProperty("_BaseMap"))
        {
            Texture t = mat.GetTexture("_BaseMap");
            if (t != null) return t;
        }

        // Legacy
        if (mat.HasProperty("_MainTex"))
        {
            Texture t = mat.GetTexture("_MainTex");
            if (t != null) return t;
        }

        return null;
    }
}
