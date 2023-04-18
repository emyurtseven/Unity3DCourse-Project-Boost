using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float mainThrustForce = 20f;
    [SerializeField] float rotationThrustForce = 80f;

    [SerializeField] GameObject mainThruster;
    [SerializeField] GameObject sideThruster;

    [SerializeField] public AudioClip mainThrusterSfx;
    [SerializeField] public AudioClip sideThrusterSfx;

    [SerializeField] float thrusterLightIntensity;
    [SerializeField] float thrusterLightLerpDuration;

    ParticleSystem mainThrustParticles;
    ParticleSystem sideThrustParticles;
    ThrusterAudio mainThrusterAudio;
    ThrusterAudio sideThrusterAudio;


    bool thrustKeyPressed;
    float horizontalInput;
    Rigidbody myRigidbody;

    Light mainThrusterLight;

    float timeElapsed;
    bool controlsEnabled = true;

    public bool ControlsEnabled { get { return controlsEnabled; } set { controlsEnabled = value; } }

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
        ProcessInput();
        RotateRocket();
    }

    private void FixedUpdate() 
    {
        ThrustRocket();
        LerpThrusterLight();
    }

    private void ProcessInput()
    {
        if (controlsEnabled)
        {
            thrustKeyPressed = Input.GetKey(KeyCode.Space);
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            thrustKeyPressed = false;
            horizontalInput = 0;
        }
    }

    /// <summary>
    /// Apply thrust to rockets local positive y axis. Called within FixedUpdate()
    /// </summary>
    private void ThrustRocket()
    {
        if (thrustKeyPressed)
        {
            mainThrustParticles.Play();
            myRigidbody.AddRelativeForce(Vector3.up * mainThrustForce, ForceMode.Force);
            mainThrusterAudio.FadeInAudio();
        }
        else
        {
            // mainThrusterLight.intensity = lightIntensity * 0;
            mainThrustParticles.Stop();
            mainThrusterAudio.FadeOutAudio();
        }
    }

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
            transform.Rotate(Vector3.forward * -horizontalInput * rotationThrustForce * Time.deltaTime);
            sideThrusterAudio.FadeInAudio();
        }
        else
        {
            sideThrustParticles.Stop();
            sideThrusterAudio.FadeOutAudio();
        }
    }
}
