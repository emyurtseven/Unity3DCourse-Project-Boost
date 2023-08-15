using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [Header("Player Normal Difficulty physics settings")]
    [Tooltip("Overrides prefab values")]
    [SerializeField] float gravityValueNormal = DefaultGameValues.Gravity;
    [SerializeField] float dragValueNormal = DefaultGameValues.Drag;
    [SerializeField] float mainThrusterForceNormal = DefaultGameValues.PlayerMainThrusterForce;
    [SerializeField] float sideThrusterForceNormal = DefaultGameValues.PlayerSideThrusterForce;
    [SerializeField] float deathThresholdVelocityNormal = DefaultGameValues.DeathThresholdVelocity;
    
    [Header("Player Hard Difficulty physics settings")]
    [Tooltip("Overrides prefab values")]
    [SerializeField] float gravityValueHard;
    [SerializeField] float dragValueHard;
    [SerializeField] float mainThrusterForceHard;
    [SerializeField] float sideThrusterForceHard;
    [SerializeField] float deathThresholdVelocityHard = 4.5f;

    [SerializeField] float debrisPersistTime = 2f;


    [Space(5f)]
    [Header("Various scene settings")]
    [SerializeField] float skyboxRotationSpeed = 1f;
    [SerializeField] AudioClip levelFinishedSfx;
    [SerializeField] float levelLoadDelay = 1f;

    string difficulty = DefaultGameValues.NormalDifficulty;
    bool levelStartedFromMainMenu;
    int currentLevel = 1;

    GameObject player;
    PlayerCollision playerCollision;
    PlayerMovement playerMovement;
    Vector3 startPosition;
    Quaternion startRotation;
    CinemachineVirtualCamera followCam;

    float respawnDelay;                     // Delay after crashing
    float skyboxStartRotation = 145f;       // Starting rotation of skybox 

    public bool LevelStartedFromMainMenu { get => levelStartedFromMainMenu; set => levelStartedFromMainMenu = value; }
    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public string Difficulty { get => difficulty; set => difficulty = value; }

    /// Static Instance property allows easy access to objects current instance from any script.
    public static GameManager Instance { get; private set; }
    public float DebrisPersistTime { get => debrisPersistTime; }

    void Awake()
    {
        SingletonThisObject();

        AudioManager.Initialize();
    }

    /// <summary>
    /// Make this a singleton object that persists through scenes. 
    /// </summary>
    private void SingletonThisObject()
    {
        int instanceCount = GameObject.FindGameObjectsWithTag(GameObjectTags.GameManager.ToString()).Length;

        if (instanceCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            SceneManager.activeSceneChanged += InitializeLevel;            // Subscribe to scene changed event
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        // Rotate skybox for visual improvement
        RenderSettings.skybox.SetFloat("_Rotation", skyboxStartRotation + Time.time * skyboxRotationSpeed);
    }

    /// <summary>
    /// SceneManager.activeSceneChanged is executed after Awake, before Start.
    /// This function is ran whenever a scene is loaded.  
    /// </summary>
    /// <param name="previousScene"> Unused parameter, required for delegate </param>
    /// <param name="loadedScene"> Unused parameter, required for delegate </param>
    private void InitializeLevel(Scene previousScene, Scene loadedScene)
    {
        // Return if we're at main menu scene
        if (loadedScene.buildIndex == 0)
        {
            return;
        }

        AudioListener.volume = 1f;
        InitializePlayerRocket();
        InitializeCamera();
        ApplyDifficultySettings();
    }

    /// <summary>
    /// Applies various values that are set in the editor, according to selected game difficulty.
    /// </summary>
    private void ApplyDifficultySettings()
    {
        if (this.difficulty == DefaultGameValues.NormalDifficulty)
        {
            Physics.gravity = new Vector3(0, gravityValueNormal, 0);
            player.GetComponent<Rigidbody>().drag = dragValueNormal;
            playerMovement.MainThrusterForce = mainThrusterForceNormal;
            playerMovement.SideThrusterForce = sideThrusterForceNormal;
            playerCollision.DeathThresholdVelocity = deathThresholdVelocityNormal;
        }
        else if (this.difficulty == DefaultGameValues.HardDifficulty)
        {
            Physics.gravity = new Vector3(0, gravityValueHard, 0);
            player.GetComponent<Rigidbody>().drag = dragValueHard;
            playerMovement.MainThrusterForce = mainThrusterForceHard;
            playerMovement.SideThrusterForce = sideThrusterForceHard;
            playerCollision.DeathThresholdVelocity = deathThresholdVelocityHard;
        }
    }

    /// <summary>
    /// Plays camera animation if level is loaded from main menu.
    /// </summary>
    private void InitializeCamera()
    {
        if (levelStartedFromMainMenu)
        {
            // Start in-game music and camera animation
            AudioManager.PlayMusicFadeIn(0, AudioClipName.LevelMusic, DefaultGameValues.MusicMaxVolume, 2, 0.75f); 
            Camera.main.transform.eulerAngles = new Vector3(-90, 0, 0);
            Camera.main.GetComponent<Animator>().SetTrigger("LevelStarted");
        }
        else
        {
            // Set follow camera otherwise
            playerMovement.ControlsEnabled = true;
            Camera.main.GetComponent<CinemachineBrain>().enabled = true;
        }
    }

    /// <summary>
    /// Initializes player rocket.
    /// </summary>
    private void InitializePlayerRocket()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollision = player.GetComponent<PlayerCollision>();
        playerMovement = player.GetComponent<PlayerMovement>();
        followCam = GameObject.FindGameObjectWithTag("FollowCamera").GetComponent<CinemachineVirtualCamera>();
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

    /// <summary>
    /// Called from PlayerCollision script
    /// </summary>
    public void LoadNextLevel()
    {
        levelStartedFromMainMenu = false;
        StartCoroutine(LoadNextLevelCoroutine());
    }

    /// <summary>
    /// Deactivates, repositions and reactivates player components after rocket explosion.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Deactivates player controls, plays animation and loads next scene.
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadNextLevelCoroutine()
    {
        AudioManager.PlaySfx(AudioClipName.LevelFinished);
        // No need to enable later since we load a new scene with default values = true
        playerCollision.IsInteractable = false;
        playerMovement.ControlsEnabled = false;

        yield return new WaitForSeconds(levelLoadDelay);

        int levelCount = SceneManager.sceneCountInBuildSettings;
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        currentLevel = (currentLevelIndex + 1) % levelCount;   // We loop back to first level at the end
        PlayerPrefs.SetInt(PlayerPreferenceKeys.SavedLevel, currentLevel);
        SceneManager.LoadScene(currentLevel);      
    }
}
