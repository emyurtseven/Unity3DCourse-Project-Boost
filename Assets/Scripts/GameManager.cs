using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float skyboxRotationSpeed = 1f;

    [SerializeField] Tutorial tutorial;
    [SerializeField] AudioClip levelFinishedSfx;
    [SerializeField] float levelLoadDelay;

    GameObject player;
    CollisionHandler playerCollision;
    PlayerMovement playerMovement;
    Vector3 startPosition;
    Quaternion startRotation;
    CinemachineVirtualCamera followCam;

    float respawnDelay;
    float skyboxStartRotation = 145f;

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


    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", skyboxStartRotation + Time.time * skyboxRotationSpeed);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="scene"> Unused parameter, required for delegate </param>
    /// <param name="nextScene"> Unused parameter, required for delegate </param>
    private void InitializeLevel(Scene scene, Scene nextScene)
    {
        // Return if we're at main menu scene
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            return;
        }
        // Start in-game music if we're in a new game
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Camera.main.GetComponent<Animator>().SetTrigger("LevelStarted");
            AudioManager.PlayMusicFadeIn(AudioClipName.LevelMusic, 1, 2, 0.75f);
        }
        

        player = GameObject.FindGameObjectWithTag("Player");
        playerCollision = player.GetComponent<CollisionHandler>();
        playerMovement = player.GetComponent<PlayerMovement>();
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
        playerCollision.IsInteractable = false;   
        playerMovement.ControlsEnabled = false;

        yield return new WaitForSeconds(respawnDelay);

        player.transform.rotation = startRotation;
        player.transform.position = startPosition;
        playerCollision.IsInteractable = true;
        playerMovement.ControlsEnabled = true;

        player.GetComponent<MeshRenderer>().enabled = true;
        player.GetComponent<Rigidbody>().useGravity = true;
        player.GetComponent<Rigidbody>().isKinematic = false;

        foreach (var item in player.GetComponents<Collider>())
        {
            item.enabled = true;
        }

        foreach (Transform child in player.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    IEnumerator LoadNextLevelCoroutine()
    {
        AudioManager.PlayOnce(AudioClipName.LevelFinished);
        // No need to enable later since we load a new scene with default values = true
        playerCollision.IsInteractable = false;
        playerMovement.ControlsEnabled = false;

        yield return new WaitForSeconds(levelLoadDelay);

        int levelCount = SceneManager.sceneCountInBuildSettings;
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene((currentLevelIndex + 1) % levelCount);       // We loop back to first level at the end
    }
}
