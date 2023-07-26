using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] GameObject startMenuCanvas;
    [SerializeField] Button[] mainMenuButtons;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] AudioSource menuMusic;

    [SerializeField] float menuMusicStartDelay = 1f;
    [SerializeField] float menuMusicFadeInDuration = 1f;

    Animator mainCameraAnimator;

    private void Start() 
    {
        mainCameraAnimator = Camera.main.gameObject.GetComponent<Animator>();

        optionsMenu.SetActive(false);

        mainMenuButtons[0].onClick.AddListener(OnNewGameClicked);
        mainMenuButtons[1].onClick.AddListener(OnContinueClicked);
        mainMenuButtons[2].onClick.AddListener(OnOptionsClicked);
        mainMenuButtons[3].onClick.AddListener(OnQuitClicked);

        AudioManager.PlayMusicFadeIn(AudioClipName.StartMenuMusic, 1, menuMusicFadeInDuration, menuMusicStartDelay);
    }

    public void OnNewGameClicked()
    {
        AudioManager.FadeOutAudio(0, 1f);
        StartCoroutine(StartNewGameTransition());
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

    IEnumerator StartNewGameTransition()
    {
        while(startMenuCanvas.GetComponent<CanvasGroup>().alpha > 0)
        {
            startMenuCanvas.GetComponent<CanvasGroup>().alpha -= Time.deltaTime;

            if (startMenuCanvas.GetComponent<CanvasGroup>().alpha < 0.7f)
            {
                mainCameraAnimator.SetTrigger("NewGamePressed");
            }

            AudioListener.volume -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitUntil(() => (mainCameraAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1));

        SceneManager.LoadScene(1);
    }

    // IEnumerator StartCameraTransition()
    // {
    //     mainCameraAnimator.SetTrigger("NewGamePressed");

    //     yield return new WaitForSeconds(0.2f);

    //     while (mainCameraAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
    //     {
    //         startMenuCanvas.GetComponent<CanvasGroup>().alpha -= Time.deltaTime * 2;
    //         yield return new WaitForEndOfFrame();
    //     }

    //     SceneManager.LoadScene(1);
    // }
    
}
