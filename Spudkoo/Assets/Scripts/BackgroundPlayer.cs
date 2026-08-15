using UnityEngine;
using System.Collections.Generic;

public class BackgroundPlayer : MonoBehaviour

{
    public List<AudioClip> backgroundMusicClips = new();
    private AudioClip currentMusic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayNewMusic();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private AudioClip GetRandomMusic()
    {
        return backgroundMusicClips[Random.Range(0, backgroundMusicClips.Count)];

    }
    private void PlayNewMusic()
    {
        AudioClip clip = GetRandomMusic();
        while (clip == currentMusic)
        {
            clip = GetRandomMusic();
        }
        AudioManager.Instance.PlayMusic(clip);
        currentMusic = clip;
        }
    }
