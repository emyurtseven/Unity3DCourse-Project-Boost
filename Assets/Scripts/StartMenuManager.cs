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

    [Header("Scene object references")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI volumeValueText;
    [SerializeField] GameObject musicOnButton;
    [SerializeField] GameObject musicOffButton;
    [SerializeField] GameObject difficultyNormalButton;
    [SerializeField] GameObject difficultyHardButton;

    MusicPlayer musicPlayer;

    int loadedLevel;
    int musicOn = 1;

    Animator mainCameraAnimator;

    private void Start() 
    {
        mainCameraAnimator = Camera.main.gameObject.GetComponent<Animator>();

        musicPlayer = AudioManager.GetMusicPlayer(0);

        LoadSavedGame();
        LoadSoundSettings();
        LoadDifficultySettings();

        volumeSlider.value = AudioListener.volume;
    }

    private void LoadSavedGame()
    {
        if (PlayerPrefs.HasKey(PlayerPreferenceKeys.SavedLevel))
        {
            GameManager.Instance.CurrentLevel = PlayerPrefs.GetInt(PlayerPreferenceKeys.SavedLevel);
        }
        else
        {
            GameManager.Instance.CurrentLevel = 1;
        }
    }

    private void LoadSoundSettings()
    {
        if (PlayerPrefs.HasKey(PlayerPreferenceKeys.Volume))
        {
            AudioListener.volume = PlayerPrefs.GetFloat(PlayerPreferenceKeys.Volume);
        }
        else
        {
            AudioListener.volume = 1;
        }

        musicOn = PlayerPrefs.GetInt(PlayerPreferenceKeys.MusicOn, 1);

        if (musicOn == 1)
        {
            SetMusicOn();
        }
        else if (musicOn == 0)
        {
            SetMusicOff();
        }
    }

    private void LoadDifficultySettings()
    {
        string difficulty = PlayerPrefs.GetString(PlayerPreferenceKeys.Difficulty, DefaultGameValues.NormalDifficulty);
        SetDifficulty(difficulty);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeValueText.text = volume.ToString("P0");
    }

    public void SetMusicOn()
    {
        musicPlayer.Mute(false);
        musicOn = 1;
        musicOnButton.GetComponent<Image>().enabled = true;
        musicOffButton.GetComponent<Image>().enabled = false;
        musicOnButton.GetComponent<Button>().interactable = false;
        musicOffButton.GetComponent<Button>().interactable = true;

        if (!musicPlayer.IsPlaying)
        {
            AudioManager.PlayMusicFadeIn(0, AudioClipName.StartMenuMusic, DefaultGameValues.MusicMaxVolume,
                                             menuMusicFadeInDuration, menuMusicStartDelay);
        }
    }

    public void SetMusicOff()
    {
        musicPlayer.Mute(true);
        musicOn = 0;
        musicOnButton.GetComponent<Image>().enabled = false;
        musicOffButton.GetComponent<Image>().enabled = true;
        musicOnButton.GetComponent<Button>().interactable = true;
        musicOffButton.GetComponent<Button>().interactable = false; 
    }

    public void SetDifficulty(string value)
    {
        // value is checked to avoid possible wrong string reference set in editor
        if (value == DefaultGameValues.NormalDifficulty)
        {
            GameManager.Instance.Difficulty = value;
            difficultyNormalButton.GetComponent<Image>().enabled = true;
            difficultyHardButton.GetComponent<Image>().enabled = false;

            difficultyNormalButton.GetComponent<Button>().interactable = false;
            difficultyHardButton.GetComponent<Button>().interactable = true;
        }
        else if (value == DefaultGameValues.HardDifficulty)
        {
            GameManager.Instance.Difficulty = value;
            difficultyNormalButton.GetComponent<Image>().enabled = false;
            difficultyHardButton.GetComponent<Image>().enabled = true;

            difficultyNormalButton.GetComponent<Button>().interactable = true;
            difficultyHardButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            Debug.LogWarning($"Difficulty string {value} is erroneous." + 
                                " Compare the value on button OnClick() UI element with DefaultGameValues class");
        }

        PlayerPrefs.SetString(PlayerPreferenceKeys.Difficulty, GameManager.Instance.Difficulty);
    }

    public void ApplySoundSettings()
    {
        PlayerPrefs.SetFloat(PlayerPreferenceKeys.Volume, AudioListener.volume);
        PlayerPrefs.SetInt(PlayerPreferenceKeys.MusicOn, musicOn);
    }

    public void OnNewGameConfirmed()
    {
        AudioManager.FadeOutMusic(0, 0, 1f);
        GameManager.Instance.LevelStartedFromMainMenu = true;
        GameManager.Instance.CurrentLevel = 1;

        PlayerPrefs.SetInt(PlayerPreferenceKeys.SavedLevel, 1);
        StartCoroutine(StartSceneTransition(firstLevelSceneIndex));
    }

    public void OnContinueClicked()
    {
        if (PlayerPrefs.HasKey(PlayerPreferenceKeys.SavedLevel))
        {
            loadedLevel = PlayerPrefs.GetInt(PlayerPreferenceKeys.SavedLevel, 1);
            string template = $"Continue to level {loadedLevel} ?";
            continueConfirmPanel.SetActive(true);
            continueConfirmPanel.GetComponentInChildren<TextMeshProUGUI>().text = template;
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
        AudioManager.FadeOutMusic(0, 0, 1f);
        GameManager.Instance.LevelStartedFromMainMenu = true;
        GameManager.Instance.CurrentLevel = loadedLevel;
        StartCoroutine(StartSceneTransition(loadedLevel));
    }

    public void OnQuitClicked()
    {
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
