using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The audio manager
/// </summary>
public static class AudioManager
{
    static bool musicInitialized = false;
    static bool sfxInitialized = false;

    static MusicPlayer musicPlayer;
    static SoundEffectsPlayer sfxPlayer;

    public static Dictionary<AudioClipName, AudioClip> audioClips =
        new Dictionary<AudioClipName, AudioClip>();

    /// <summary>
    /// Gets whether or not the audio manager has been initialized
    /// </summary>

    public static bool SfxInitialized { get => sfxInitialized; }
    public static bool MusicInitialized { get => musicInitialized; }

    /// <summary>
    /// Initializes the audio manager
    /// </summary>
    /// <param name="source">audio source</param>
    public static void Initialize(MusicPlayer player)
    {
        musicPlayer = player;
        musicInitialized = true;
    }

    public static void Initialize(SoundEffectsPlayer player)
    {
        sfxPlayer = player;
        sfxInitialized = true;

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
    public static void PlaySfx(AudioClipName name, float volume=1f)
    {
        if (audioClips.ContainsKey(name))
        {
            sfxPlayer.AudioSource.PlayOneShot(audioClips[name], volume);
        }
    }

    public static void PlayMusic(AudioClipName clipName, float volume=1f)
    {
        musicPlayer.AudioSource.Stop();
        
        if (!audioClips.ContainsKey(clipName))
        {
            Debug.LogWarning("Audio file {name} missing");
            return;
        }
        musicPlayer.AudioSource.clip = audioClips[clipName];
        musicPlayer.AudioSource.volume = volume;
        musicPlayer.AudioSource.Play();
    }

    public static void PlayMusicFadeIn(AudioClipName clipName, float volume, float fadeDuration, float fadeDelay = 0)
    {
        musicPlayer.StopAllCoroutines();
        musicPlayer.PlayMusicFadeIn(audioClips[clipName], volume, fadeDuration, fadeDelay);
    }

    public static void FadeInMusic(float finalVolume, float fadeDuration, float fadeDelay=0)
    {
        musicPlayer.StopAllCoroutines();
        musicPlayer.StartCoroutine(musicPlayer.FadeInAudio(finalVolume, fadeDuration, fadeDelay));
    }

    public static void FadeOutMusic(float finalVolume, float fadeDuration, float fadeDelay=0)
    {
        musicPlayer.StopAllCoroutines();
        musicPlayer.StartCoroutine(musicPlayer.FadeOutAudio(finalVolume, fadeDuration, fadeDelay));
    }
}
