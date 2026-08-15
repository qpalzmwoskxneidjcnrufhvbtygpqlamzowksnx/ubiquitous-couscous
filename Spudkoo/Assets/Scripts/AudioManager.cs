using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public class AudioManager : MonoBehaviour
{


    public static AudioManager Instance;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup SFXGroup;

    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private bool canGrow = true;
    private AudioSource musicSource;
    private List<AudioSource> sfxPool;
    private GameObject poolContainer;
    private void InitializeManager()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        if (musicGroup != null) musicSource.outputAudioMixerGroup = musicGroup;
        poolContainer = new GameObject("SFX_Pool");
        poolContainer.transform.SetParent(transform);
        sfxPool = new List<AudioSource>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewPoolSource();

        }
    }
    private AudioSource CreateNewPoolSource()
    {
        GameObject obj = new GameObject($"Pooled_SFX_{sfxPool.Count}");

        obj.transform.SetParent(poolContainer.transform);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        if (SFXGroup != null) source.outputAudioMixerGroup = SFXGroup;

        sfxPool.Add(source);
        return source;
        
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeManager();
       
    }
    private AudioSource GetAvailableSFXSource()
    {
        for (int i = 0; i < sfxPool.Count; i++)
        {
            if (!sfxPool[i].isPlaying)
            {
                return sfxPool[i];
            }
        }
        if(canGrow)
        {
            CreateNewPoolSource();

        }
        return sfxPool[0];
        
    }
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        if (musicSource.isPlaying && musicSource.clip == clip) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
        Debug.Log($"Now playing {clip.name}");
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }
    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;
        AudioSource source = GetAvailableSFXSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = 0f;
        source.Play();
    }
}