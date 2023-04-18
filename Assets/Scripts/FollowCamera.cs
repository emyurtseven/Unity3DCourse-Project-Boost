using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FollowCamera : MonoBehaviour
{
    CinemachineVirtualCamera followCam;
    // Start is called before the first frame update
    void Start()
    {
        followCam = GetComponent<CinemachineVirtualCamera>();
        followCam.Follow = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
