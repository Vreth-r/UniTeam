using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set;}

    [Header("Library")]
    public SoundLibrary soundLibrary;

    [Header("Volume")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float gameplayVolume = 1f;

    private List<AudioSource> sfxSources = new();
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
    }

    void OnEnable()
    {
        AudioEvents.OnPlaySound += HandlePlaySound;
        AudioEvents.OnStopSound += HandleStopSound;
    }

    void OnDisable()
    {
        AudioEvents.OnPlaySound -= HandlePlaySound;
        AudioEvents.OnStopSound -= HandleStopSound;
    }

    // Event Handlers

    void HandlePlaySound(string id)
    {
        var sound = soundLibrary.Get(id);
        if (sound == null)
        {
            Debug.LogWarning($"AudioManager: Sound '{id}' not found.");
            return;
        }

        switch (sound.category)
        {
            case SoundCategory.Music:
                PlayMusic(sound);
                break;
            case SoundCategory.UI:
            case SoundCategory.Gameplay:
                PlaySFX(sound);
                break;
        }
    }

    void HandleStopSound(string id)
    {
        var sound = soundLibrary.Get(id);
        if (sound == null) return;

        if (sound.category == SoundCategory.Music)
        {
            if (musicSource.clip == sound.clip)
                musicSource.Stop();
        }
        else
        {
            foreach (var src in sfxSources)
            {
                if (src.isPlaying && src.clip == sound.clip)
                {
                    src.Stop();
                    return;
                }
            }
        }
    }

    // Playback

    void PlayMusic(SoundEntry sound)
    {
        if (musicSource.clip == sound.clip && musicSource.isPlaying)
            return;

        musicSource.clip = sound.clip;
        musicSource.volume = sound.volume * masterVolume * musicVolume;
        musicSource.pitch = sound.pitch;
        musicSource.loop = sound.loop;
        musicSource.Play();
    }

    void PlaySFX(SoundEntry sound)
    {
        AudioSource src = GetSFXSource();
        float categoryVolume = sound.category == SoundCategory.UI ? uiVolume : gameplayVolume;

        src.clip = sound.clip;
        src.volume = sound.volume * masterVolume * categoryVolume;
        src.pitch = sound.pitch;
        src.loop = sound.loop;
        src.Play();
    }

    // Pooling

    AudioSource GetSFXSource()
    {
        foreach (var src in sfxSources)
        {
            if (!src.isPlaying)
                return src;
        }

        var newSrc = gameObject.AddComponent<AudioSource>();
        sfxSources.Add(newSrc);
        return newSrc;
    }
}