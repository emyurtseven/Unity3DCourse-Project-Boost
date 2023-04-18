using System;
using System.Collections;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] AudioClip explosionSfx;
    [SerializeField] float respawnDelay = 2f;
    [SerializeField] float finishConditionTimer = 2f;
    [SerializeField] float deathThresholdVelocity = 2f;

    [SerializeField] bool isInvulnerable = false;

    GameManager gameManager;
    ExplodeOnImpact exploder;
    CapsuleCollider bodyCollider;
    BoxCollider landingCollider;
    Rigidbody myRigidbody;

    float timer = 0f;

    bool isInteractable = true;
    bool isTouchingFinish;

    public float RespawnDelay {get { return respawnDelay; } }
    public bool IsInteractable {get { return isInteractable; } set { isInteractable = value; } }

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        exploder = GetComponent<ExplodeOnImpact>();
        bodyCollider = GetComponent<CapsuleCollider>();
        landingCollider = GetComponent<BoxCollider>();
        myRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isTouchingFinish && isInteractable && myRigidbody.velocity.magnitude <= Mathf.Epsilon)
        {
            timer += Time.deltaTime;

            if (timer >= finishConditionTimer)
            {
                HandlePlayerVictory();
                isTouchingFinish = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision) 
    {
        float relativeVelocity = collision.relativeVelocity.magnitude;

        if (!isInteractable)
        {
            return;
        }

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
                return;
            }
            else
            {
                HandlePlayerCrash(collision);
            }
        }
    }

    private void OnCollisionExit(Collision collision) 
    {
        if (collision.gameObject.tag == "Finish")
        {
            isTouchingFinish = false;
        }
    }

    private void HandlePlayerCrash(Collision collisionData)
    {
        if (isInvulnerable)
        {
            return;
        }

        GetComponent<AudioSource>().PlayOneShot(explosionSfx, 1);
        exploder.ExplodeObject(collisionData);
        gameManager = FindObjectOfType<GameManager>();
        gameManager.RespawnPlayer();
    }

    private void HandlePlayerVictory()
    {
        GameObject.FindGameObjectWithTag("Finish").GetComponentInChildren<ParticleSystem>().Play();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.LoadNextLevel();
    }
}
