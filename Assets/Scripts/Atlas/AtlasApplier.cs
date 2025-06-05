using UnityEngine;

[CreateAssetMenu(fileName = "AtlasApplier", menuName = "Atlas/AtlasApplier")]
public class AtlasApplier : ScriptableObject
{
    [Header("Atlas Configuration")]
    public AtlasSO atlasMaster;
    public AtlasType atlasType;
    public Vector2 atlasTextureSize = new Vector2(1280f, 720f);

    public void ApplyAtlas(GameObject target)
    {
        var renderer = target.GetComponent<Renderer>();
        if (renderer == null) return;

        Rect uvRect = atlasMaster.GetUVRect(atlasType);
        if (uvRect.width == 0 || uvRect.height == 0) return;

        Material sharedMaterial = AtlasManager.GetSharedAtlasMaterial(atlasMaster);
        if (sharedMaterial == null) return;

        if (renderer.sharedMaterial != sharedMaterial)
        {
            renderer.sharedMaterial = sharedMaterial;
        }

        Vector2 scale = new Vector2(1f / atlasTextureSize.x, 1f / atlasTextureSize.y);
        Vector2 offset = new Vector2(uvRect.x / atlasTextureSize.x, uvRect.y / atlasTextureSize.y);

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetVector("_MainTex_ST", new Vector4(scale.x, scale.y, offset.x, offset.y));
        renderer.SetPropertyBlock(block);
    }
}