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
    public static Dictionary<AudioClipName, AudioClip> audioClips =
        new Dictionary<AudioClipName, AudioClip>();

    /// <summary>
    /// Gets whether or not the audio manager has been initialized
    /// </summary>
    public static bool Initialized
    {
        get { return initialized; }
    }

    /// <summary>
    /// Initializes the audio manager
    /// </summary>
    /// <param name="source">audio source</param>
    public static void Initialize(AudioSource source)
    {
        initialized = true;
        audioSource = source;
        
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

    public static void PlayMusic(AudioClipName name, float volume=1f)
    {
        if (!audioClips.ContainsKey(name))
        {
            Debug.LogWarning("Audio file {name} missing");
            return;
        }
        audioSource.clip = audioClips[name];
        audioSource.volume = volume;
        audioSource.Play();
    }
}
