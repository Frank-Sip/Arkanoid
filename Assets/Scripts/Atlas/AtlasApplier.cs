using UnityEngine;

public class AtlasApplier : MonoBehaviour
{
    [Header("Apply Atlas")]
    public AtlasSO atlasMaster;
    public AtlasType atlasType;

    private Renderer objectRenderer;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        ApplyAtlas();
    }

    private void ApplyAtlas()
    {
        Rect uvRect = atlasMaster.GetUVRect(atlasType);

        if (uvRect.width == 0 || uvRect.height == 0)
        {
            Debug.LogWarning("No UVs found for " + atlasType);
            return;
        }

        Material sharedMaterial = atlasMaster.sharedMaterial;

        if (sharedMaterial != null)
        {
            objectRenderer.sharedMaterial = sharedMaterial;

            Vector2 scale = new Vector2(1f / 1280f, 1f / 720f);
            Vector2 offset = new Vector2(uvRect.x / 1280f, uvRect.y / 720f);

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            objectRenderer.GetPropertyBlock(block);
            block.SetVector("_MainTex_ST", new Vector4(scale.x, scale.y, offset.x, offset.y));
            objectRenderer.SetPropertyBlock(block);
        }
        else
        {
            Debug.LogWarning("Shared material is missing in AtlasSO.");
        }
    }
}
