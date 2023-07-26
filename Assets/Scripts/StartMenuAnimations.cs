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
    [SerializeField] GameObject redRocket;
    [SerializeField] GameObject blueRocket;
    [SerializeField] GameObject orangeRocket;
    [SerializeField] Transform greenPad;
    [SerializeField] Transform grayPad;

    void Start()
    {
        StartCoroutine(AnimationsCoroutine());
    }

    IEnumerator AnimationsCoroutine()
    {
        yield return new WaitForSeconds(liftoffSequenceDelay);
        StartLiftoffSequence(blueRocket, grayPad);

        yield return new WaitForSeconds(landingSequenceDelay);

        StartLandingSequence(redRocket, greenPad);

        yield return new WaitForSeconds(landingSequenceDelay + 5);

        StartLiftoffSequence(redRocket, greenPad);

        yield return new WaitForSeconds(1);

        orangeRocket.GetComponent<Rigidbody>().useGravity = true;

        yield return new WaitForSeconds(landingSequenceDelay + 2);

        StartLandingSequence(redRocket, grayPad);
        StartLandingSequence(blueRocket, greenPad);
    }

    void StartLandingSequence(GameObject landingRocket, Transform landingPad)
    {
        landingRocket.transform.position = landingPad.transform.position + Vector3.up * 25;
        Rigidbody landingRocketRigidbody = landingRocket.GetComponent<Rigidbody>();
        landingRocketRigidbody.useGravity = true;

        PlayerMovement landingRocketMovement = landingRocket.GetComponent<PlayerMovement>();
        landingRocketMovement.ControlsEnabled = false;
        landingRocketMovement.MainThrustForce = landingRocketThrust;

        StartCoroutine(SimulateLanding(landingRocketRigidbody, landingRocketMovement));
    }

    void StartLiftoffSequence(GameObject departingRocket, Transform launchPad)
    {
        departingRocket.transform.position = launchPad.transform.position + Vector3.up * 1.22f;
        PlayerMovement departingRocketMovement = departingRocket.GetComponent<PlayerMovement>();
        Rigidbody departingRocketRigidbody = departingRocket.GetComponent<Rigidbody>();
        departingRocketMovement.ControlsEnabled = false;
        departingRocketMovement.MainThrustForce = departingRocketThrust;
        departingRocketRigidbody.useGravity = true;

        StartCoroutine(SimulateLiftoff(departingRocket, departingRocketRigidbody, departingRocketMovement));
    }

    IEnumerator SimulateLanding(Rigidbody landingRocketRigidbody, PlayerMovement landingRocketMovement)
    {
        while (landingRocketRigidbody.velocity.y > -8.5f)
        {
            yield return new WaitForEndOfFrame();
        }

        while (landingRocketRigidbody.velocity.y < -0.1f)
        {
            landingRocketMovement.ProcessInput(true, 0);
            yield return new WaitForEndOfFrame();
        }

        landingRocketMovement.ProcessInput(false, 0);
    }

    IEnumerator SimulateLiftoff(GameObject departingRocket, Rigidbody departingRocketRigidbody, PlayerMovement departingRocketMovement)
    {
        float thrustDirection = Random.Range(-0.1f, 0.1f);

        while (true)
        {
            if (Mathf.Abs(departingRocket.transform.rotation.z) < 0.12f)
            {
                departingRocketMovement.ProcessInput(true, thrustDirection);
            }
            else
            {
                departingRocketMovement.ProcessInput(true, 0f);
            }
            
            yield return new WaitForEndOfFrame();

            
            if (departingRocket.transform.position.y > 35)
            {
                departingRocketMovement.ProcessInput(false, 0f);
                departingRocketRigidbody.velocity = Vector3.zero;
                departingRocket.transform.rotation = Quaternion.identity;
                departingRocketRigidbody.useGravity = false;
                yield break;
            }
        }
    }
}
