using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to explosion object
/// </summary>
public class Explosion : MonoBehaviour
{
    ParticleSystem particles;
    Light explosionLight;

    float explosionLightIntensity;

    float explosionDuration;
    float timeElapsed;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        explosionDuration = particles.main.duration;
        explosionLight = GetComponent<Light>();
        explosionLightIntensity = explosionLight.intensity;
    }

    void Update()
    {
        LerpExplosionLight();
        
        // Destroy the game object if the explosion has finished its emission
        if (explosionDuration <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            explosionDuration -= Time.deltaTime;
        }
    }

    private void LerpExplosionLight()
    {
        float t = 1 - timeElapsed / explosionDuration;
        explosionLight.intensity = Mathf.Lerp(0, explosionLightIntensity, t);
        timeElapsed += Time.deltaTime;
    }
}
