using System.Collections.Generic;
using UnityEngine;

public class SettingsManager
{
    private SettingsSO settingsConfig;
    private AudioManager audioManager;

    public SettingsManager(SettingsSO config)
    {
        settingsConfig = config;
    }    public void Init()
    {
        audioManager = ServiceProvider.GetService<AudioManager>();
        settingsConfig.LoadFromPlayerPrefs();
        ApplyCurrentSettings();
    }    public void ApplyCurrentSettings()
    {
        if (audioManager != null)
        {
            audioManager.SetMasterVolume(settingsConfig.masterVolume);
            Debug.Log($"SettingsManager: Applied volume {settingsConfig.masterVolume} to AudioManager");
        }
        else
        {
            Debug.LogWarning("SettingsManager: AudioManager is null, cannot apply volume settings");
        }
    }public void IncreaseMasterVolume()
    {
        float oldVolume = settingsConfig.masterVolume;
        settingsConfig.IncreaseMasterVolume();
        Debug.Log($"SettingsManager: Volume increased from {oldVolume} to {settingsConfig.masterVolume}");
        ApplyCurrentSettings();
        UpdateVolumeDisplays();
    }    public void DecreaseMasterVolume()
    {
        float oldVolume = settingsConfig.masterVolume;
        settingsConfig.DecreaseMasterVolume();
        Debug.Log($"SettingsManager: Volume decreased from {oldVolume} to {settingsConfig.masterVolume}");
        ApplyCurrentSettings();
        UpdateVolumeDisplays();
    }

    public float GetMasterVolume() => settingsConfig.masterVolume;

    private void UpdateVolumeDisplays()
    {
        var uiManager = ServiceProvider.GetService<UIManager>();
        uiManager?.UpdateVolumeDisplay("master", settingsConfig.masterVolume);
    }
}
