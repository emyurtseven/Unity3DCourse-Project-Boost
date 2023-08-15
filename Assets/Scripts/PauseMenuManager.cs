using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages pause menu, attached to InGameMenuCanvas object.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] TextMeshProUGUI stageText;

    ThrusterAudio[] thrusterAudios;

    bool gamePaused;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        thrusterAudios = player.GetComponentsInChildren<ThrusterAudio>();

        // display stage number on screen while we're at it
        stageText.text = "Stage " + GameManager.Instance.CurrentLevel.ToString();
    }

    private void Update() 
    {
        // pause/unpause game with player input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gamePaused)
            {
                pauseMenu.SetActive(true);
                PauseGame(true);
                gamePaused = true;
            }
            else
            {
                OnResumePressed();
            }
        }
    }

    public void PauseGame(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0;
            foreach (ThrusterAudio thruster in thrusterAudios)
            {
                thruster.PauseAudio(true);
            }
        }
        else
        {
            Time.timeScale = 1;
            foreach (ThrusterAudio thruster in thrusterAudios)
            {
                thruster.PauseAudio(false);
            }
        }
    }

    /// <summary>
    /// Callback for UI button resume
    /// </summary>
    public void OnResumePressed()
    {
        PauseGame(false);
        gamePaused = false;
        pauseMenu.SetActive(false);
    }

    /// <summary>
    /// Load start scene.
    /// </summary>
    public void OnQuitLevelPressed()
    {
        Time.timeScale = 1;
        AudioManager.StopMusic(0);
        SceneManager.LoadScene(0);
    }
}
