using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;

    public float BgmSoundSize = 0.3f;
    public float SfxSoundSize = 1f;

    private Dictionary<AudioClip, List<AudioSource>> activeSFX = new(); private List<AudioSource> sfxSources = new List<AudioSource>();

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
        BgmSoundSize = PlayerPrefs.GetFloat("BGMVolume", 0.15f);
        SfxSoundSize = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolumeSettings();
    }

    private void ApplyVolumeSettings()
    {
        if (bgmAudioSource != null) bgmAudioSource.volume = BgmSoundSize;
        if (sfxAudioSource != null) sfxAudioSource.volume = SfxSoundSize;
    }

    public void SetBGMVolume(float vol) //����� �Ҹ� ����
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

        // �̹� ���� Ŭ���� ��� ���̸� �ƹ� �͵� ���� ����
        if (bgmAudioSource.isPlaying && bgmAudioSource.clip == clip)
        {
            return;
        }

        bgmAudioSource.Stop(); // �ٸ� ���̸� ���� ��� ����
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
    public void Play_stop_ok_SFX(AudioClip clip)
    {
        if (clip == null) return;

        // �̹� ��� ���� ȿ�����̸� ����
        if (activeSFX.TryGetValue(clip, out var sources))
        {
            foreach (var s in sources)
            {
                if (s != null && s.isPlaying)
                    return;
            }
        }
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = SfxSoundSize;
        source.Play();

        if (!activeSFX.ContainsKey(clip))
            activeSFX[clip] = new List<AudioSource>();
        activeSFX[clip].Add(source);

        StartCoroutine(RemoveSourceWhenFinished(clip, source));
    }

    public IEnumerator RemoveSourceWhenFinished(AudioClip clip, AudioSource source)
    {
        yield return new WaitWhile(() => source != null && source.isPlaying);

        if (activeSFX.ContainsKey(clip))
        {
            activeSFX[clip].Remove(source);
            if (activeSFX[clip].Count == 0)
                activeSFX.Remove(clip);
        }

        if (source != null)
            Destroy(source);
    }


    public void StopSFX(AudioClip clip)
    {
        if (!activeSFX.ContainsKey(clip)) return;

        foreach (var source in activeSFX[clip])
        {
            if (source != null)
            {
                source.Stop();
                Destroy(source);
            }
        }
        activeSFX.Remove(clip);
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