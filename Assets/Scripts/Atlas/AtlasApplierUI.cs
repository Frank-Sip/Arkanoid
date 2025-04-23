using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class AtlasApplierUI : MonoBehaviour
{
    [Header("Apply Atlas")]
    public AtlasSO atlasMaster;
    public AtlasType atlasType;

    [Header("Atlas Texture Size")]
    public Vector2 atlasTextureSize = new Vector2(1280f, 720f);

    private RawImage rawImage;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
        ApplyAtlas();
    }

    private void ApplyAtlas()
    {
        if (atlasMaster == null)
        {
            Debug.LogWarning($"AtlasApplierUI: No atlas assigned on {name}");
            return;
        }

        Rect uvRect = atlasMaster.GetUVRect(atlasType);
        rawImage.texture = atlasMaster.sharedMaterial.mainTexture;

        if (uvRect.width == 0 || uvRect.height == 0)
        {
            Debug.LogWarning($"AtlasApplierUI: UV rect not defined for type {atlasType} on {name}");
            return;
        }

        Vector2 scale = new Vector2(uvRect.width / atlasTextureSize.x, uvRect.height / atlasTextureSize.y);
        Vector2 offset = new Vector2(uvRect.x / atlasTextureSize.x, uvRect.y / atlasTextureSize.y);

        rawImage.uvRect = new Rect(offset, scale);
    }
}
