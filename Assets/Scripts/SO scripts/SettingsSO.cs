using UnityEngine;

[CreateAssetMenu(fileName = "SettingsSO", menuName = "Settings/Settings Configuration")]
public class SettingsSO : ScriptableObject
{
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;

    [Header("UI Settings")]
    public float volumeStepSize = 0.1f;
    public float minVolume = 0f;
    public float maxVolume = 1f;

    private const string MASTER_VOLUME_KEY = "MasterVolume";

    public void LoadFromPlayerPrefs()
    {
        masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
        masterVolume = Mathf.Clamp(masterVolume, minVolume, maxVolume);
    }

    public void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp(volume, minVolume, maxVolume);
        SaveToPlayerPrefs();
    }

    public void IncreaseMasterVolume()
    {
        float newVolume = masterVolume + volumeStepSize;
        SetMasterVolume(newVolume);
    }    public void DecreaseMasterVolume()
    {
        float newVolume = masterVolume - volumeStepSize;
        SetMasterVolume(newVolume);
    }

    public void ResetToDefault()
    {
        SetMasterVolume(1f);
    }
}
