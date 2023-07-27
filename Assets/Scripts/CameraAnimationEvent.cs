using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
