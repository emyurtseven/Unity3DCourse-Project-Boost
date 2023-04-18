using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] AudioClip levelFinishedSfx;
    [SerializeField] float levelLoadDelay;

    GameObject player;
    CollisionHandler playerCollision;
    Movement playerMovement;
    Vector3 startPosition;
    Quaternion startRotation;
    CinemachineVirtualCamera followCam;

    float respawnDelay;

    void Awake()
    {
        
        int instanceCount = FindObjectsOfType<GameManager>().Length;

        if (instanceCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            SceneManager.activeSceneChanged += InitializeLevel;            // Subscribe to scene changed event
            DontDestroyOnLoad(gameObject);
        }
    }

    private void InitializeLevel(Scene scene, Scene nextScene)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollision = player.GetComponent<CollisionHandler>();
        playerMovement = player.GetComponent<Movement>();
        followCam = FindObjectOfType<CinemachineVirtualCamera>();
        startPosition = player.transform.position;
        startRotation = player.transform.rotation;
        respawnDelay = playerCollision.RespawnDelay;
    }


    /// <summary>
    /// Respawns player at start position after death, with a delay.
    /// </summary>
    public void RespawnPlayer()
    {
        StartCoroutine(RespawnCoroutine());
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        // No need to enable later since we instantiate a new Player with default values = true
        playerCollision.IsInteractable = false;   
        playerMovement.ControlsEnabled = false;

        yield return new WaitForSeconds(respawnDelay);

        player = Instantiate(playerPrefab, startPosition, startRotation);
        playerCollision = player.GetComponent<CollisionHandler>();
        playerMovement = player.GetComponent<Movement>();
        followCam.Follow = player.transform;
    }

    IEnumerator LoadNextLevelCoroutine()
    {
        GetComponent<AudioSource>().PlayOneShot(levelFinishedSfx);
        // No need to enable later since we load a new scene with default values = true
        playerCollision.IsInteractable = false;
        playerMovement.ControlsEnabled = false;

        yield return new WaitForSeconds(levelLoadDelay);

        int levelCount = SceneManager.sceneCountInBuildSettings;
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene((currentLevelIndex + 1) % levelCount);       // We loop back to first level at the end
    }
}
