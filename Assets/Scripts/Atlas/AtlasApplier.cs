using UnityEngine;

public class AtlasApplier : MonoBehaviour
{
    [Header("Apply Atlas")]
    public AtlasSO atlasMaster;
    public AtlasType atlasType;

    private Renderer objectRenderer;
    
    [Header("Atlas Texture Size")]
    public Vector2 atlasTextureSize = new Vector2(1280f, 720f);

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
            return;
        }
        
        Material sharedMaterial = AtlasManager.GetSharedAtlasMaterial(atlasMaster);

        if (sharedMaterial != null)
        {
            if (objectRenderer.sharedMaterial != sharedMaterial)
            {
                objectRenderer.sharedMaterial = sharedMaterial;
            }
            
            Vector2 scale = new Vector2(1f / atlasTextureSize.x, 1f / atlasTextureSize.y);
            Vector2 offset = new Vector2(uvRect.x / atlasTextureSize.x, uvRect.y / atlasTextureSize.y);
            
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            objectRenderer.GetPropertyBlock(block);
            block.SetVector("_MainTex_ST", new Vector4(scale.x, scale.y, offset.x, offset.y));
            objectRenderer.SetPropertyBlock(block);
        }
    }
}