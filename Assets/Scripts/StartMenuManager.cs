using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] int firstLevelSceneIndex = 1;


    [Header("Panel object references")]
    [SerializeField] GameObject startMenuCanvas;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject loadFailedPanel;
    [SerializeField] GameObject continueConfirmPanel;

    [Header("Audio controls")]
    [SerializeField] float volume;
    [SerializeField] float menuMusicStartDelay = 1f;
    [SerializeField] float menuMusicFadeInDuration = 1f;
    [SerializeField] AudioSource menuMusicSource;
    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI volumeValueText;

    bool musicOn;

    int loadedLevel;

    Animator mainCameraAnimator;
    GameManager gameManager;

    private void Start() 
    {
        mainCameraAnimator = Camera.main.gameObject.GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        AudioManager.PlayMusicFadeIn(AudioClipName.StartMenuMusic, 1, menuMusicFadeInDuration, menuMusicStartDelay);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeValueText.text = volume.ToString() + " %";
    }

    public void ToggleMusic(bool value)
    {

    }

    public void ApplyVolumeSettings()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
    }

    public void OnNewGameConfirmed()
    {
        AudioManager.FadeOutMusic(0, 1f);
        gameManager.LevelStartedFromMainMenu = true;
        StartCoroutine(StartSceneTransition(firstLevelSceneIndex));
    }

    public void OnContinueClicked()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            loadedLevel = PlayerPrefs.GetInt("SavedLevel", 1);
            string template = $"Continue to level {loadedLevel} ?";
            continueConfirmPanel.SetActive(true);
            continueConfirmPanel.GetComponent<TextMeshProUGUI>().text = template;
        }
        else
        {
            loadFailedPanel.SetActive(true);
        }
    }

    /// <summary>
    /// This assumes level scene index is the same as level number.
    /// </summary>
    public void OnContinueConfirmed()
    {
        AudioManager.FadeOutMusic(0, 1f);
        gameManager.LevelStartedFromMainMenu = true;
        StartCoroutine(StartSceneTransition(loadedLevel));
    }

    public void OnSettingsClicked()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }

    public void OnQuitClicked()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    IEnumerator StartSceneTransition(int sceneIndex)
    {
        CanvasGroup menuCanvasGroup = startMenuCanvas.GetComponent<CanvasGroup>();

        while(menuCanvasGroup.alpha > 0)
        {
            menuCanvasGroup.alpha -= Time.deltaTime;
            AudioListener.volume -= Time.deltaTime;

            if (menuCanvasGroup.alpha < 0.8f)
            {
                mainCameraAnimator.SetTrigger("NewGamePressed");
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitUntil(() => (mainCameraAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1));

        SceneManager.LoadScene(sceneIndex);
    }
}
