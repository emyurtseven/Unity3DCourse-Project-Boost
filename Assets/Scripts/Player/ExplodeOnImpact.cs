using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ExplodeOnImpact : MonoBehaviour
{
    [Header("Auto-explode options")]
    [SerializeField] bool willAutoExplode;
    [SerializeField] bool hasParticleEffects;
    [Tooltip("Only used if auto-explode is enabled.")]
    [SerializeField] float thresholdVelocity;

    [Header("Prefab of the object")]
    [SerializeField] GameObject explodedObjectPrefab;
    [SerializeField] GameObject explosionPrefab;

    [Header("Explosion parameters")]
    [Range(0, 1000)]
    [SerializeField] float minExplosionForce;
    [Range(0, 1000)]
    [SerializeField] float maxExplosionForce;
    [SerializeField] int brokenPartCount;
    [SerializeField] float debrisPersistTime = 2f;

    // Name of the parent object to be created that holds the broken part objects
    string containerName = "DebrisContainer";

    Transform containerTransform;
    GameObject debrisContainer;

    public float DebrisPersistTime { get { return debrisPersistTime; } set { debrisPersistTime = value; } }
    public Transform ContainerTransform {get { return containerTransform; } }
    
    private void OnCollisionEnter(Collision other) 
    {
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
    /// Explodes a duplicate gameObject mesh into many pieces.
    /// </summary>
    /// <param name="collision"> Collision parameter is passed in CollisionHandler() script.</param>
    public void ExplodeObject(Collision collision)
    {
        CreateDebrisContainer();

        // Create a duplicate game object to be exploded
        GameObject explodedObject = Instantiate(explodedObjectPrefab,
                                                transform.position,
                                                transform.rotation);
        UnityEngine.Random.InitState((int)Time.time);
        Vector3 collisionPoint = collision.contacts[0].point;
        float colliderRadius = GetComponent<Collider>().bounds.extents.magnitude;

        if (hasParticleEffects)
        {
            Instantiate(explosionPrefab, collisionPoint, Quaternion.identity);
        }
        // This creates the mesh pieces
        new ScamScatter.Scatter().Run(this, null, explodedObject, brokenPartCount, containerTransform);

        AdjustBrokenPartCount();
        AddExplosionForce(collisionPoint, colliderRadius);

        StartCoroutine(DisableComponents());
        StartCoroutine(FadeDebris());
    }

    private void CreateDebrisContainer()
    {
        debrisContainer = new GameObject(containerName);
        // debrisContainer.transform.parent = transform;
        containerTransform = debrisContainer.transform;
    }

    /// <summary>
    /// Disables and hides original object.
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

        // Deactivate every child onject except the debris container
        foreach (Transform child in transform)
        {
            if (child.name != containerName)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Destroys excess broken parts that were instantiated.
    /// </summary>
    private void AdjustBrokenPartCount()
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
                piece.gameObject.layer = LayerMask.NameToLayer("Debris");
            }
        }
    }

    /// <summary>
    /// Adds explosion force to each broken part.
    /// </summary>
    /// <param name="collisionPoint"></param>
    /// <param name="colliderRadius"></param>
    private void AddExplosionForce(Vector3 collisionPoint, float colliderRadius)
    {
        foreach (Transform piece in containerTransform)
        {
            var rb = piece.transform.gameObject.AddComponent<Rigidbody>();
            float force = UnityEngine.Random.Range(minExplosionForce, maxExplosionForce);
            rb.AddExplosionForce(force, collisionPoint, colliderRadius);
        }
    }


    /// <summary>
    /// Gradually decreases the alpha of mesh renderers to gently fade out the pieces, then destroys the game object.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeDebris()
    {
        yield return new WaitForSeconds(debrisPersistTime);

        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= 0.02f;
            Color color = new Color(1, 1, 1, alpha);

            foreach (Transform fragment in containerTransform)
            {
                fragment.gameObject.GetComponent<MeshRenderer>().materials[0].color = color;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        Destroy(debrisContainer);
    }
}
