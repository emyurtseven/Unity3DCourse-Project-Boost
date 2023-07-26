using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuAnimations : MonoBehaviour
{
    
    [Header("Start menu parameters")]
    [SerializeField] float landingSequenceDelay = 1f;
    [SerializeField] float liftoffSequenceDelay = 1f;

    [SerializeField] float landingRocketThrust = 1f;
    [SerializeField] float departingRocketThrust = 1f;

    [Header("Object references")]
    [SerializeField] GameObject landingRocket;
    [SerializeField] GameObject departingRocket;
    [SerializeField] Transform landingPad;
    [SerializeField] Transform launchPad;

    PlayerMovement landingRocketMovement;
    PlayerMovement departingRocketMovement;

    Rigidbody landingRocketRigidbody;
    Rigidbody departingRocketRigidbody;

    bool landingInputOn;


    void Start()
    {
        landingRocket.transform.position = landingPad.transform.position + Vector3.up * 25;
        landingRocketMovement = landingRocket.GetComponent<PlayerMovement>();
        landingRocketMovement.ControlsEnabled = false;
        landingRocketRigidbody = landingRocket.GetComponent<Rigidbody>();
        landingRocketRigidbody.isKinematic = true;

        departingRocketMovement = departingRocket.GetComponent<PlayerMovement>();
        departingRocketRigidbody = departingRocket.GetComponent<Rigidbody>();
        departingRocketMovement.ControlsEnabled = false;

        landingRocketMovement.MainThrustForce = landingRocketThrust;
        departingRocketMovement.MainThrustForce = departingRocketThrust;

        Invoke("StartLandingSequence", landingSequenceDelay);
        Invoke("StartLiftoffSequence", liftoffSequenceDelay);
    }

    void Update()
    {
        if (landingInputOn || landingRocket == null)
        {
            return;
        }

        if (landingRocketRigidbody.velocity.y < -8.5f)
        {
            landingInputOn = true;
            StartCoroutine(SimulateLanding());
        }
    }



    void StartLandingSequence()
    {
        landingRocketRigidbody.isKinematic = false;
    }

    void StartLiftoffSequence()
    {
        StartCoroutine(SimulateLiftoff());
    }

    IEnumerator SimulateLanding()
    {
        while (landingRocketRigidbody.velocity.y < -0.1f)
        {
            landingRocketMovement.ProcessInput(true, 0);
            yield return new WaitForEndOfFrame();
        }

        landingRocketMovement.ProcessInput(false, 0);
        landingInputOn = false;

        yield return new WaitForSeconds(5f);

        landingRocketMovement.MainThrustForce = 12f;

        while (landingRocket.transform.position.y < 25)
        {
            landingRocketMovement.ProcessInput(true, 0.05f);
            yield return new WaitForEndOfFrame();
        }

        landingRocketMovement.ProcessInput(false, 0);
        Destroy(landingRocket);
    }

    IEnumerator SimulateLiftoff()
    {
        float thrustDirection = Random.Range(-0.1f, 0.1f);

        while (true)
        {

            if (departingRocket.transform.position.y < 6)
            {
                departingRocketMovement.ProcessInput(true, 0f);
            }
            else if (Mathf.Abs(departingRocket.transform.rotation.z) < 0.12f)
            {
                departingRocketMovement.ProcessInput(true, thrustDirection);
            }
            else
            {
                departingRocketMovement.ProcessInput(true, 0f);
            }
            
            yield return new WaitForEndOfFrame();

            
            if (departingRocket.transform.position.y > 30)
            {
                Destroy(departingRocket);
                yield break;
            }
        }
    }
}
