using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    private AudioSource music;
    private AudioSource sfx;
    private List<AudioClip> bgTracks;
    private List<AudioSO> soundEffects;

    public AudioManager(List<AudioClip> bgTracks, List<AudioSO> soundEffects)
    {
        this.bgTracks = bgTracks;
        this.soundEffects = soundEffects;
    }

    public void Init(AudioSource musicSource, AudioSource sfxSource)
    {
        this.music = musicSource;
        this.sfx = sfxSource;
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
    }
}