using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;
    public AudioClip[] Bgm_c;

    public float BgmSoundSize = 1f;//{ get; private set; }
    public float SfxSoundSize = 1f;//{ get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı� ����
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
            return;
        }
    }

    private void LoadVolumeSettings()
    {
        //BgmSoundSize = PlayerPrefs.GetFloat("BGMVolume", 1f);
        //SfxSoundSize = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolumeSettings();
    }

    private void ApplyVolumeSettings()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = BgmSoundSize;
        }
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = SfxSoundSize;
        }
    }

    public void SetBGMVolume(float volume) //����� �Ҹ� ����
    {
        BgmSoundSize = Mathf.Clamp(volume, 0f, 1f);
        if (bgmAudioSource != null) bgmAudioSource.volume = BgmSoundSize;
        //PlayerPrefs.SetFloat("BGMVolume", BgmSoundSize);
    }

    public void SetSFXVolume(float volume)
    {
        SfxSoundSize = Mathf.Clamp(volume, 0f, 1f);
        if (sfxAudioSource != null) sfxAudioSource.volume = SfxSoundSize;
        //PlayerPrefs.SetFloat("SFXVolume", SfxSoundSize);
    }
    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmAudioSource == null || bgmClip == null) return;

        // �̹� ���� Ŭ���� ��� ���̸� �ƹ� �͵� ���� ����
        if (bgmAudioSource.isPlaying && bgmAudioSource.clip == bgmClip)
        {
            return;
        }

        bgmAudioSource.Stop(); // �ٸ� ���̸� ���� ��� ����
        bgmAudioSource.clip = bgmClip;
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