using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UIAtlasApplier", menuName = "Atlas/AtlasApplierUI")]
public class AtlasApplierUI : ScriptableObject
{
    [Header("UI Atlas Configuration")]
    public AtlasSO atlasMaster;
    public Vector2 atlasTextureSize = new Vector2(1280f, 720f);

    [System.Serializable]
    public class UIElementMapping
    {
        public string objectName;
        public AtlasType atlasType;
    }

    [Header("UI Element Mappings")]
    public UIElementMapping[] uiMappings;

    public void ApplyAtlasToLayout(GameObject layout)
    {
        if (layout == null) return;
        
        foreach (var image in layout.GetComponentsInChildren<RawImage>())
        {
            ApplyAtlasToUIElement(image.gameObject);
        }
    }

    public void ApplyAtlasToUIElement(GameObject target)
    {
        if (target == null) return;

        string targetName = target.name;
        UIElementMapping mapping = System.Array.Find(uiMappings, m => m.objectName == targetName);

        if (mapping != null)
        {
            ApplyAtlas(target, mapping.atlasType);
        }
    }

    private void ApplyAtlas(GameObject target, AtlasType type)
    {
        var rawImage = target.GetComponent<RawImage>();
        if (rawImage == null) return;

        Rect uvRect = atlasMaster.GetUVRect(type);
        if (uvRect.width == 0 || uvRect.height == 0) return;

        rawImage.texture = atlasMaster.sharedMaterial.mainTexture;
        Vector2 scale = new Vector2(uvRect.width / atlasTextureSize.x, uvRect.height / atlasTextureSize.y);
        Vector2 offset = new Vector2(uvRect.x / atlasTextureSize.x, uvRect.y / atlasTextureSize.y);
        rawImage.uvRect = new Rect(offset, scale);
    }
}
