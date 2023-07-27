using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// An audio source for the entire game
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    AudioSource audioSource;

    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }

    /// <summary>
    /// Awake is called before Start
    /// </summary>
    void Awake()
	{
        // make sure we only have one of this game object
        // in the game
        if (!AudioManager.MusicInitialized)
        {
            // initialize audio manager and persist audio source across scenes
            AudioManager.Initialize(this);
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // duplicate game object, so destroy
            Destroy(gameObject);
        }
    }

    public void PlayMusicFadeIn(AudioClip clip, float volume, float fadeDuration, float startDelay = 0)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = 0;
        
        StartCoroutine(FadeInAudio(volume, fadeDuration, startDelay));
    }

    public IEnumerator FadeInAudio(float finalVolume, float fadeDuration, float startDelay=0)
    {
        float volume = audioSource.volume;
        yield return new WaitForSeconds(startDelay);

        audioSource.Play();

        while (volume <= finalVolume)
        {
            // increment volume and pitch timer values
            volume += (Time.deltaTime / fadeDuration);
            // set volume based on volume curve set in editor, mapping timer to volume
            audioSource.volume = volume;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator FadeOutAudio(float finalVolume, float fadeDuration, float fadeDelay= 0)
    {
        float volume = audioSource.volume;
        yield return new WaitForSeconds(fadeDelay);

        while (volume >= finalVolume)
        {
            // increment volume and pitch timer values
            volume -= (Time.deltaTime / fadeDuration);
            // set volume based on volume curve set in editor, mapping timer to volume
            audioSource.volume = volume;
            yield return new WaitForEndOfFrame();
        }
    }
}
