using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Replaces the object with a pre-fractured prefab and adds force to pieces to simulate an explosion.
/// Pieces are put in a temporary container and destroyed after a delay.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ExplodeOnImpact : MonoBehaviour
{
    [Header("Auto-explode options")]
    [SerializeField] bool willAutoExplode;
    [SerializeField] bool hasParticleEffects;
    [Tooltip("Only used if auto-explode is enabled.")]
    [SerializeField] float thresholdVelocity;
    [SerializeField] string excludedObjectTag = string.Empty;

    [Header("Prefab of the object")]
    [SerializeField] GameObject explodedObjectPrefab;
    [SerializeField] GameObject explosionPrefab;

    [Header("Explosion parameters")]
    [Range(0, 1000)]
    [SerializeField] float minExplosionForce;
    [Range(0, 1000)]
    [SerializeField] float maxExplosionForce;
    [SerializeField] int brokenPartCount;

    // Name of the parent object to be created that holds the broken part objects
    string containerName = "DebrisContainer";

    GameObject debrisContainer;
    
    /// <summary>
    /// If auto explode is enabled, object explodes on collision depending on velocity.
    /// An optional tag can be passed to specify objects that are excluded from triggering the explosion.
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.tag == excludedObjectTag)
        {
            return;
        }

        if (willAutoExplode)
        {
            float velocity = GetComponent<Rigidbody>().velocity.magnitude;
            if (velocity > thresholdVelocity)
            {
                ExplodeObject(other);
            }
        }
    }

    /// <summary>
    /// Explodes a duplicate gameObject mesh into many pieces, while keeping original object.
    /// </summary>
    /// <param name="collision"> Collision parameter is passed in from PlayerCollision script.</param>
    public void ExplodeObject(Collision collision)
    {
        debrisContainer = new GameObject(containerName);
        
        Transform containerTransform = debrisContainer.transform;

        // Create a duplicate game object to be exploded
        GameObject explodedObject = Instantiate(explodedObjectPrefab,
                                                transform.position,
                                                transform.rotation);
                                                
        UnityEngine.Random.InitState((int)Time.time);
        Vector3 collisionPoint = collision.contacts[0].point;
        float colliderRadius = GetComponent<Collider>().bounds.extents.magnitude;

        // Play explosion particles if applicable
        if (hasParticleEffects)
        {
            Instantiate(explosionPrefab, collisionPoint, Quaternion.identity);
        }

        // This creates the mesh pieces
        new ScamScatter.Scatter().Run(this, null, explodedObject, brokenPartCount, containerTransform);

        AdjustBrokenPartCount(containerTransform);
        AddExplosionForce(collisionPoint, colliderRadius, containerTransform);

        AudioManager.PlaySfx(AudioClipName.RocketExplosion, 1);

        debrisContainer.AddComponent<FadeDebris>();
        StartCoroutine(DisableComponents());
    }

    /// <summary>
    /// Creates a temp gameobject to hold debris pieces.
    /// </summary>
    private void CreateDebrisContainer()
    {

    }

    /// <summary>
    /// Disables and hides the original object.
    /// </summary>
    IEnumerator DisableComponents()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;

        // Colliders are turned off after a delay to make broken parts fly off, giving an explosion effect
        yield return new WaitForSeconds(0.1f);      

        foreach (var item in GetComponents<Collider>())
        {
            item.enabled = false;
        }

        // Deactivate every child onject 
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Destroys excess broken parts that were instantiated.
    /// </summary>
    private void AdjustBrokenPartCount(Transform containerTransform)
    {
        for (int i = 0; i < containerTransform.childCount; i++)
        {
            Transform piece = containerTransform.GetChild(i);

            if (i >= brokenPartCount)
            {
                Destroy(piece.gameObject);
            }
            else
            {
                // set debris pieces to Debris layer, to avoid collision with player
                piece.gameObject.layer = LayerMask.NameToLayer("Debris");
            }
        }
    }

    /// <summary>
    /// Adds explosion force to each broken part.
    /// </summary>
    private void AddExplosionForce(Vector3 collisionPoint, float colliderRadius, Transform containerTransform)
    {
        foreach (Transform piece in containerTransform)
        {
            var rb = piece.transform.gameObject.AddComponent<Rigidbody>();
            float force = UnityEngine.Random.Range(minExplosionForce, maxExplosionForce);
            rb.AddExplosionForce(force, collisionPoint, colliderRadius);
        }
    }
}
