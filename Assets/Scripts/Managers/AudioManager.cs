using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("BGM")]
    public AudioSource music;
    public List<AudioClip> bgTracks;

    [Header("SFX")]
    public AudioSource sfx;
    public List<AudioSO> soundEffects;

    public void Init()
    {
        if (music == null) music = GetComponent<AudioSource>();
        if (sfx == null) sfx = gameObject.AddComponent<AudioSource>();
    }

    public void PlayBGM(int bgmIndex)
    {
        if (bgmIndex < 0 || bgmIndex >= bgTracks.Count) return;

        music.clip = bgTracks[bgmIndex];
        music.loop = true;
        music.Play();
    }

    public void PlaySFX(int sfxIndex)
    {
        if (sfxIndex < 0 || sfxIndex >= soundEffects.Count) return;

        soundEffects[sfxIndex].PlaySound(sfx);
    }
}
