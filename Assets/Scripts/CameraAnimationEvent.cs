using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fired in camera animation after level load. Attach this to Main Camera.
/// </summary>
public class CameraAnimationEvent : MonoBehaviour
{
    PlayerMovement playerMovement;

    private void Start() 
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    public void OnCameraPanDownEvent()
    {
        playerMovement.ControlsEnabled = true;
    }
}
