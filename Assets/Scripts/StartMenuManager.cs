using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] Button[] mainMenuButtons;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] AudioSource menuMusic;

    [SerializeField] float menuMusicStartDelay = 0.5f;
    [SerializeField] float menuMusicFadeInDuration = 1f;

    private void Start() 
    {
        optionsMenu.SetActive(false);


        mainMenuButtons[0].onClick.AddListener(OnNewGameClicked);
        mainMenuButtons[1].onClick.AddListener(OnContinueClicked);
        mainMenuButtons[2].onClick.AddListener(OnOptionsClicked);
        mainMenuButtons[3].onClick.AddListener(OnQuitClicked);

        StartCoroutine(FadeInAudio(menuMusicFadeInDuration));
    }

    IEnumerator FadeInAudio(float seconds)
    {
        yield return new WaitForSeconds(menuMusicStartDelay);

        AudioManager.PlayMusic(AudioClipName.StartMenuMusic, 0);

        float volume = 0f;
        while (volume < 1f)
        {
            // increment volume and pitch timer values
            volume += (Time.deltaTime / seconds);
            // set volume based on volume curve set in editor, mapping timer to volume
            menuMusic.volume = volume;
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnNewGameClicked()
    {
        SceneManager.LoadScene(1);
    }
    public void OnContinueClicked()
    {
        Debug.Log("Load Game");

    }
    public void OnOptionsClicked()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }
    public void OnQuitClicked()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    
}
