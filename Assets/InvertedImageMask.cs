using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class InvertedImageMask : MonoBehaviour, IMaterialModifier
{
    [SerializeField] private Image img;
    [SerializeField] private Vector2 maskOffsetPixels = Vector2.zero;
    [SerializeField] private Vector2 maskSizePixels = new(100f, 100f);
    [SerializeField] private Texture maskTexture;

    private Material runtimeMaterial;
    private Material runtimeMaterialBase;


    private void OnValidate() => MarkDirty();

    private void MarkDirty()
    {
        if (!img) return;
        img.SetMaterialDirty();
        img.SetVerticesDirty();
    }

    public Vector2 offset
    {
        get => maskOffsetPixels;
        set
        {
            maskOffsetPixels = value;
            MarkDirty();
        }
    }

    public Vector2 size
    {
        get => maskSizePixels;
        set
        {
            maskSizePixels = value;
            MarkDirty();
        }
    }

    private void OnEnable()
    {
        if (!img)
            img = GetComponent<Image>();
        MarkDirty();
    }

    private void OnDisable()
    {
        if (runtimeMaterial != null)
        {
            DestroyImmediate(runtimeMaterial);
        }
    }
    
    
    public Material GetModifiedMaterial(Material baseMaterial)
    {
        if (!img || baseMaterial == null)
            return baseMaterial;

        if (runtimeMaterial == null || runtimeMaterialBase != baseMaterial)
        {
            if (runtimeMaterial != null)
                DestroyImmediate(runtimeMaterial);
            runtimeMaterial = new Material(baseMaterial) {  hideFlags = HideFlags.DontSave };
        }
        
        runtimeMaterialBase = baseMaterial;
        
        ApplyProperties(runtimeMaterial);
        return runtimeMaterial;
    }

    private void ApplyProperties(Material material)
    {
        var rt = img.rectTransform;
        var rect = rt.rect;

        if (maskTexture != null)
            material.SetTexture("_MaskTex", maskTexture);

        Vector2 topLeftLocal = new(rect.xMin, rect.yMax);
        Vector2 maskTopLeftLocal = topLeftLocal + new Vector2(maskOffsetPixels.x, -maskOffsetPixels.y);
        
        // prevent division by zero
        float invWidth = 1f / Mathf.Max(maskSizePixels.x, 1e-5f);
        float invHeight = 1f / Mathf.Max(maskSizePixels.y, 1e-5f);

        Matrix4x4 translate = Matrix4x4.Translate(new Vector3(-maskTopLeftLocal.x, -maskTopLeftLocal.y, 0f));
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(invWidth, invHeight, 1f));
        Matrix4x4 maskMatrix = scale * translate * rt.worldToLocalMatrix;

        material.SetMatrix("_MaskMatrix", maskMatrix);
    }
}
