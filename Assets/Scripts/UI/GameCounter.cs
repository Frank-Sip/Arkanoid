using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class GameCounter
{
    public string id;
    public TMP_Text counter;
    public string prefix = "";
    public string suffix = "";
    private int value;

    public void UpdateValue(int newValue)
    {
        value = newValue;
        UpdateUI();
    }

    public void Increment()
    {
        value++;
        UpdateUI();
    }

    public void Reset()
    {
        value = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (counter != null)
        {
            counter.text = $"{prefix} {value} {suffix}";
        }
    }
}
