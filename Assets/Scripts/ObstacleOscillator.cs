using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves attached object back-forth between start pos and set position vector.
/// </summary>
public class ObstacleOscillator : MonoBehaviour
{
    [Tooltip("Object will move here before coming back")]
    [SerializeField] Vector3 movementVector;
    [Tooltip("Period in seconds")]
    [SerializeField] float movementPeriod;

    Vector3 startingPosition;
    float movementFactor;
    float timer = 0f;

    const float Tau = Mathf.PI * 2;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        if (movementPeriod <= Mathf.Epsilon) { return; }    // Skip movement if period is 0

        Oscillate();
    }

    private void Oscillate()
    {
        // Map timer value to 0-Tau radians
        movementFactor = Mathf.Lerp(0, Tau, timer / movementPeriod);
        // Get sine value of radians and fix it so it oscillates between 0-1 instead of (-1)-(+1)
        movementFactor = (Mathf.Sin(movementFactor) + 1) / 2;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;

        timer = (timer + Time.deltaTime) % movementPeriod;  // Increment timer and reset if period is reached
    }
}

