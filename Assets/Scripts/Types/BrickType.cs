public enum BrickType
{
    None,
    Weak,
    Normal,
    Strong,
    Tough
}

[System.Serializable]
public class BrickTypeConfig
{
    public BrickType type;
    public AtlasApplier atlas;
}