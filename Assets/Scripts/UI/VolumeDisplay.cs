using UnityEngine;
using TMPro;

[System.Serializable]
public class VolumeDisplay
{
    public string id;
    public TMP_Text valueText;
    public string prefix = "";
    public string suffix = "%";
    public int decimalPlaces = 0;

    public void UpdateValue(float value)
    {
        if (valueText != null)
        {
            float displayValue = value * 100f;
            string formattedValue = displayValue.ToString($"F{decimalPlaces}");
            valueText.text = $"{prefix}{formattedValue}{suffix}";
        }
    }
}
