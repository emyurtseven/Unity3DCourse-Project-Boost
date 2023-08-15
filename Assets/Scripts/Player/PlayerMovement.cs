using UnityEngine;

/// <summary>
/// Manages player input and rocket movement. 
/// Physical movement values are set in GameManager object, not here.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] bool controlsEnabled = true;
    [SerializeField] GameObject mainThruster;
    [SerializeField] GameObject sideThruster;

    [SerializeField] float thrusterLightIntensity;
    [SerializeField] float thrusterLightLerpDuration;

    ParticleSystem mainThrustParticles;
    ParticleSystem sideThrustParticles;
    ThrusterAudio mainThrusterAudio;
    ThrusterAudio sideThrusterAudio;

    float mainThrusterForce = DefaultGameValues.PlayerMainThrusterForce;
    float sideThrusterForce = DefaultGameValues.PlayerSideThrusterForce;

    bool thrustKeyPressed;
    float horizontalInput;
    Rigidbody myRigidbody;

    Light mainThrusterLight;

    float timeElapsed;

    public bool ControlsEnabled { get { return controlsEnabled; } set { controlsEnabled = value; } }
    public float MainThrusterForce { get => mainThrusterForce; set => mainThrusterForce = value; }
    public float SideThrusterForce { get => sideThrusterForce; set => sideThrusterForce = value; }

    // get object references
    private void Awake() 
    {
        myRigidbody = GetComponent<Rigidbody>();
        mainThrusterAudio = mainThruster.GetComponent<ThrusterAudio>();
        sideThrusterAudio = sideThruster.GetComponent<ThrusterAudio>();
        mainThrustParticles = mainThruster.GetComponent<ParticleSystem>();
        sideThrustParticles = sideThruster.GetComponent<ParticleSystem>();
        mainThrusterLight = mainThruster.GetComponent<Light>();
    }

    private void Update()
    {
        if (controlsEnabled)
        {
            ProcessInput();
        }

        RotateRocket();
    }

    private void FixedUpdate() 
    {
        ThrustRocket();
        LerpThrusterLight();
    }

    /// <summary>
    /// Checks for input using old system.
    /// </summary>
    private void ProcessInput()
    {
        thrustKeyPressed = Input.GetKey(KeyCode.Space);
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    /// <summary>
    /// This is an automated version that supplies input values instead of getting it from the player.
    /// Used in start menu rocket movement animations.
    /// </summary>
    public void ProcessInput(bool spaceKeyPressed, float horizontalAxisValue)
    {
        thrustKeyPressed = spaceKeyPressed;
        horizontalInput = horizontalAxisValue;
    }

    /// <summary>
    /// Apply thrust to rockets local positive y axis. Called within FixedUpdate()
    /// </summary>
    private void ThrustRocket()
    {
        if (thrustKeyPressed)
        {
            mainThrustParticles.Play();
            myRigidbody.AddRelativeForce(Vector3.up * mainThrusterForce, ForceMode.Force);
            mainThrusterAudio.FadeInAudio();
        }
        else
        {
            mainThrustParticles.Stop();
            mainThrusterAudio.FadeOutAudio();
        }
    }

    /// <summary>
    /// Rotate player rocket in z axis. Called within Update()
    /// </summary>
    private void RotateRocket()
    {
        if (Mathf.Abs(horizontalInput) > Mathf.Epsilon)
        {
            // Adjust particle emission speed to positive or negative, thus changing direction
            var main = sideThrustParticles.main;
            main.startSpeed = -horizontalInput * 5f;
            sideThrustParticles.Play();

            // Set angular v to 0 to prevent conflicts with physics system
            myRigidbody.angularVelocity = Vector3.zero;
            transform.Rotate(Vector3.forward * -horizontalInput * sideThrusterForce * Time.deltaTime);
            sideThrusterAudio.FadeInAudio();
        }
        else
        {
            sideThrustParticles.Stop();
            sideThrusterAudio.FadeOutAudio();
        }
    }

    /// <summary>
    /// Fades in and out light from thrust.
    /// </summary>
    private void LerpThrusterLight()
    {
        if (thrustKeyPressed)
        {
            float t = timeElapsed / thrusterLightLerpDuration;
            mainThrusterLight.intensity = Mathf.Lerp(0, thrusterLightIntensity, t);
            timeElapsed = Mathf.Clamp(timeElapsed + Time.fixedDeltaTime, 0, thrusterLightLerpDuration);
        }
        else
        {
            float t = timeElapsed / thrusterLightLerpDuration;
            mainThrusterLight.intensity = Mathf.Lerp(0, thrusterLightIntensity, t);
            timeElapsed = Mathf.Clamp(timeElapsed - Time.fixedDeltaTime, 0, thrusterLightLerpDuration);
        }
    }
}
