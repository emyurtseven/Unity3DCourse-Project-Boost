using UnityEngine;

/// <summary>
/// There's no set audioclip in this script,
/// it plays whichever AudioSource component is attached to the gameObject.
/// This set up allows multiple continuous sound effects to fade in/out independently.
/// </summary>
public class ThrusterAudio : MonoBehaviour
{    
    // These give us curves to adjust in the editor.
    [SerializeField] AnimationCurve volumeCurve;
    [SerializeField] AnimationCurve pitchCurve;

    [SerializeField] AudioClipName audioClipName;

    AudioSource audioSource;

    // Timers that hold a value between 0-targetValue, used in evaluating volume/pitch curve.
    float volumeTimer;     
    float pitchTimer;
    // Time taken to reach targeted volume/pitch. Set within curve in editor.
    float targetVolumeTime;      
    float targetPitchTime;

    bool thrusting = false;     // Are we thrusting in any given frame?


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() 
    {
        if (audioClipName == AudioClipName.MainThruster)
        {
            audioSource.clip = transform.parent.GetComponent<PlayerMovement>().mainThrusterSfx;
        }
        else if (audioClipName == AudioClipName.SideThruster)
        {
            audioSource.clip = transform.parent.GetComponent<PlayerMovement>().sideThrusterSfx;
        }

        // Get target time values from curves set in the editor
        targetVolumeTime = volumeCurve.keys[1].time;
        targetPitchTime = pitchCurve.keys[1].time;
    }

    private void LateUpdate() 
    {
        AdjustThrustAudio();
    }

    /// <summary>
    ///  Gradually increases or decreases both volume and pitch if player is thrusting or just stopped.
    ///  Important!: Use the curves set in the editor to adjust rates and durations.
    ///  This method is called in Update().
    /// </summary>
    private void AdjustThrustAudio()
    {
        if (thrusting)
        {
            // increment volume and pitch timer values
            volumeTimer += 1f * Time.deltaTime;
            pitchTimer += 1f * Time.deltaTime;

            // set volume based on volume curve set in editor, mapping timer to volume
            audioSource.volume = volumeCurve.Evaluate(volumeTimer);

            // set pitch based on pitch curve set in editor, mapping timer to pitch
            audioSource.pitch = pitchCurve.Evaluate(pitchTimer);
        }
        else
        {
            // decrement volume and pitch timer values
            volumeTimer -= 1f * Time.deltaTime;
            pitchTimer -= 1f * Time.deltaTime;

            // same as above
            audioSource.volume = volumeCurve.Evaluate(volumeTimer);
            audioSource.pitch = pitchCurve.Evaluate(pitchTimer);
        }
    }

    /// <summary>
    /// Called in player Movement script, while key is pressed.
    /// </summary>
    public void FadeInAudio()
    {
        // To ensure we run this block only once
        if (!thrusting)
        {
            audioSource.Play();
            thrusting = true;

            // Set timers to 0, to be incremented in AdjustThrustAudio()
            volumeTimer = 0;       
            pitchTimer = 0;
        }
    }

    /// <summary>
    /// Called in player Movement script while key press is false
    /// </summary>
    public void FadeOutAudio()
    {
        // To ensure we run this block only once
        if (thrusting)
        {
            thrusting = false;

            // Set timers to their max values, to be decremented in AdjustThrustAudio()
            volumeTimer = targetVolumeTime;       
            pitchTimer = targetPitchTime;

            Invoke("StopClip", targetVolumeTime);       // Stop audioclip after volume timer is finished
        }
    }

    private void StopClip()
    {
        // If thrusting began during the delay before invoke was called, don't stop audio
        if (!thrusting && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
