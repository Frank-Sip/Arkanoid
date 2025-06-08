using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    private AudioSource music;
    private AudioSource sfx;
    private List<AudioClip> bgTracks;
    private List<AudioSO> soundEffects;
    private float masterVolume = 1f;

    public AudioManager(List<AudioClip> bgTracks, List<AudioSO> soundEffects)
    {
        this.bgTracks = bgTracks;
        this.soundEffects = soundEffects;
    }public void Init(AudioSource musicSource, AudioSource sfxSource)
    {
        this.music = musicSource;
        this.sfx = sfxSource;
        UpdateVolumeSettings();
    }

    public void PlayBGM(int bgmIndex)
    {
        if (music == null || bgmIndex < 0 || bgmIndex >= bgTracks.Count) return;

        music.clip = bgTracks[bgmIndex];
        music.loop = true;
        music.Play();
    }

    public void PlaySFX(int sfxIndex)
    {
        if (sfx == null || sfxIndex < 0 || sfxIndex >= soundEffects.Count) return;

        soundEffects[sfxIndex].PlaySound(sfx);
    }    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumeSettings();
        Debug.Log($"AudioManager: Volume set to {masterVolume}");
    }

    public float GetMasterVolume() => masterVolume;

    private void UpdateVolumeSettings()
    {
        if (music != null)
        {
            music.volume = masterVolume;
            Debug.Log($"AudioManager: Music volume updated to {music.volume}");
        }
        if (sfx != null)
        {
            sfx.volume = masterVolume;
            Debug.Log($"AudioManager: SFX volume updated to {sfx.volume}");
        }
    }
}