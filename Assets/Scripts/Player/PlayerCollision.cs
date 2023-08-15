using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles all player collision.
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    [SerializeField] float respawnDelay = 2f;
    [SerializeField] float finishConditionTimer = 2f;       // min time required to spend stationary on landing pad
    [SerializeField] float deathThresholdVelocity = DefaultGameValues.DeathThresholdVelocity;     
    [SerializeField] bool isInvulnerable = false;       // FOR DEBUGGING

    ExplodeOnImpact exploder;
    CapsuleCollider bodyCollider;
    BoxCollider landingCollider;
    Rigidbody myRigidbody;

    float timer = 0f;

    bool isInteractable = true;
    bool isTouchingFinish;

    public float RespawnDelay {get { return respawnDelay; } }
    public bool IsInteractable {get { return isInteractable; } set { isInteractable = value; } }

    public float DeathThresholdVelocity { get => deathThresholdVelocity; set => deathThresholdVelocity = value; }

    void Awake()
    {
        exploder = GetComponent<ExplodeOnImpact>();
        bodyCollider = GetComponent<CapsuleCollider>();
        landingCollider = GetComponent<BoxCollider>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Coroutine that checks if the player spends required time on finish for victory.
    /// </summary>
    private IEnumerator CheckVictoryCondition()
    {
        while (isTouchingFinish)
        {
            // increment timer only if player is stationary
            if (isInteractable && myRigidbody.velocity.magnitude <= Mathf.Epsilon)
            {
                timer += Time.deltaTime;
            }

            // enough time passed, player is victorious
            if (timer >= finishConditionTimer)
            {
                HandlePlayerVictory();
                isTouchingFinish = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Discard event if not interactable
        if (!isInteractable)
        {
            return;
        }

        EvaluateCollision(collision);
    }

    /// <summary>
    /// Checks if a collision is fatal, or properly with finish target.
    /// </summary>
    /// <param name="collision"> Collision event from unity engine </param>
    private void EvaluateCollision(Collision collision)
    {
        float relativeVelocity = collision.relativeVelocity.magnitude;

        // If upper rocket body touches anything: insta-death. Even if it's launch pad or finish.
        // If collision velocity > threshold: insta-death. Even if it's launch pad or finish.
        if (collision.GetContact(0).thisCollider == bodyCollider ||
                                relativeVelocity > deathThresholdVelocity)
        {
            HandlePlayerCrash(collision);
        }
        else
        {
            if (collision.gameObject.tag == "Safe")
            {
                return;
            }
            else if (collision.gameObject.tag == "Finish")
            {
                timer = 0f;
                isTouchingFinish = true;
                StartCoroutine(CheckVictoryCondition());
                return;
            }
            else
            {
                // if lower body touches anything other than safe spots, insta-death
                HandlePlayerCrash(collision);
            }
        }
    }

    // We're not touching finish anymore, so victory condition is cancelled
    private void OnCollisionExit(Collision collision) 
    {
        if (collision.gameObject.tag == "Finish")
        {
            isTouchingFinish = false;
        }
    }

    /// <summary>
    /// Explodes rocket and respawns player.
    /// </summary>
    /// <param name="collisionData"></param>
    private void HandlePlayerCrash(Collision collisionData)
    {
        if (exploder == null || isInvulnerable)
        {
            return;
        }

        exploder.ExplodeObject(collisionData);
        GameManager.Instance.RespawnPlayer();
    }

    /// <summary>
    /// Plays victory effect and load next scene.
    /// </summary>
    private void HandlePlayerVictory()
    {
        GameObject.FindGameObjectWithTag("Finish").GetComponentInChildren<ParticleSystem>().Play();
        GameManager.Instance.LoadNextLevel();
    }
}
