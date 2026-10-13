using UnityEngine;

public class DogHearing : MonoBehaviour
{
    [Header("Hearing Settings")]
    [SerializeField] private float hearingMultiplier = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    public bool HasHeardNoise { get; private set; }
    public Vector3 LastHeardPosition { get; private set; }

    public void HearNoise(Vector3 noisePosition, float noiseRadius)
    {
        float distance = Vector3.Distance(
            transform.position,
            noisePosition
        );

        float effectiveRadius = noiseRadius * hearingMultiplier;

        if (distance > effectiveRadius)
            return;

        LastHeardPosition = noisePosition;
        HasHeardNoise = true;

        if (showDebugLogs)
            Debug.Log("DOG HEARD NOISE at " + noisePosition);
    }

    public void ClearNoise()
    {
        HasHeardNoise = false;
    }
}