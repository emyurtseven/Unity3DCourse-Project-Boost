using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The audio manager
/// </summary>
public static class AudioManager
{
    static bool initialized = false;
    static AudioSource audioSource;
    static GameAudioSource gameAudioSource;

    public static Dictionary<AudioClipName, AudioClip> audioClips =
        new Dictionary<AudioClipName, AudioClip>();

    /// <summary>
    /// Gets whether or not the audio manager has been initialized
    /// </summary>
    public static bool Initialized
    {
        get { return initialized; }
    }

    public static AudioSource AudioSource { get => audioSource; set => audioSource = value; }

    /// <summary>
    /// Initializes the audio manager
    /// </summary>
    /// <param name="source">audio source</param>
    public static void Initialize(AudioSource source)
    {
        initialized = true;
        audioSource = source;
        gameAudioSource = source.GetComponent<GameAudioSource>();

        foreach (AudioClipName clipName in Enum.GetValues(typeof(AudioClipName)))
        {
            string clipNameString = Enum.GetName(typeof(AudioClipName), clipName);
            AudioClip audioClip = Resources.Load<AudioClip>("Audio/" + clipNameString);

            if (audioClip != null)
            {
                audioClips.Add(clipName, audioClip);
            }
        }
    }

    /// <summary>
    /// Plays the audio clip with the given name
    /// </summary>
    /// <param name="name">name of the audio clip to play</param>
    public static void PlayOnce(AudioClipName name, float volume=1f)
    {
        if (audioClips.ContainsKey(name))
        {
            audioSource.PlayOneShot(audioClips[name], volume);
        }
    }

    public static void PlayMusic(AudioClipName clipName, float volume=1f)
    {
        audioSource.Stop();
        
        if (!audioClips.ContainsKey(clipName))
        {
            Debug.LogWarning("Audio file {name} missing");
            return;
        }
        audioSource.clip = audioClips[clipName];
        audioSource.volume = volume;
        audioSource.Play();
    }

    public static void PlayMusicFadeIn(AudioClipName clipName, float volume, float fadeDuration, float fadeDelay = 0)
    {
        gameAudioSource.StopAllCoroutines();
        gameAudioSource.PlayMusicFadeIn(audioClips[clipName], volume, fadeDuration, fadeDelay);
    }

    public static void FadeInAudio(float finalVolume, float fadeDuration, float fadeDelay=0)
    {
        gameAudioSource.StopAllCoroutines();
        gameAudioSource.StartCoroutine(gameAudioSource.FadeInAudio(finalVolume, fadeDuration, fadeDelay));
    }

    public static void FadeOutAudio(float finalVolume, float fadeDuration, float fadeDelay=0)
    {
        gameAudioSource.StopAllCoroutines();
        gameAudioSource.StartCoroutine(gameAudioSource.FadeOutAudio(finalVolume, fadeDuration, fadeDelay));
    }
}
