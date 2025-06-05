using UnityEngine;

[CreateAssetMenu(fileName = "ButtonActionConfig", menuName = "Button/ButtonActionConfig")]
public class ButtonSO : ScriptableObject
{
    [System.Serializable]
    public class ButtonMapping
    {
        public string buttonName;
        public ButtonActionType actionType;
    }

    public enum ButtonActionType
    {
        Play,
        Resume,
        MainMenu,
        Quit
    }

    [Header("Button Mappings")]
    public ButtonMapping[] buttonMappings;
}