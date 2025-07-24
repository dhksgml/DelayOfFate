using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;

    public float BgmSoundSize = 0.3f;
    public float SfxSoundSize = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴 방지
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 방지
            return;
        }
    }

    private void LoadVolumeSettings()
    {
        BgmSoundSize = PlayerPrefs.GetFloat("BGMVolume", 0.15f);
        SfxSoundSize = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolumeSettings();
    }

    private void ApplyVolumeSettings()
    {
        if (bgmAudioSource != null) bgmAudioSource.volume = BgmSoundSize;
        if (sfxAudioSource != null) sfxAudioSource.volume = SfxSoundSize;
    }

    public void SetBGMVolume(float vol) //배경음 소리 설정
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = BgmSoundSize;
        }
        PlayerPrefs.SetFloat("BGMVolume", BgmSoundSize);
    }

    public void SetSFXVolume(float volume)
    {
        SfxSoundSize = Mathf.Clamp(volume, 0f, 1f);
        sfxAudioSource.volume = SfxSoundSize;
        PlayerPrefs.SetFloat("SFXVolume", SfxSoundSize);
    }
    public void PlayBGM(AudioClip clip)
    {
        if (bgmAudioSource == null || clip == null) return;

        // 이미 같은 클립이 재생 중이면 아무 것도 하지 않음
        if (bgmAudioSource.isPlaying && bgmAudioSource.clip == clip)
        {
            return;
        }

        bgmAudioSource.Stop(); // 다른 곡이면 기존 재생 중지
        bgmAudioSource.clip = clip;
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = BgmSoundSize;
        bgmAudioSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        sfxAudioSource.PlayOneShot(clip, SfxSoundSize);
    }
    public void StopBGM()
    {
        if (bgmAudioSource == null) return;
        bgmAudioSource.Stop();
    }

    public void PauseBGM()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Pause();
        }
    }
    public void UnPause()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.UnPause();
        }
    }

}