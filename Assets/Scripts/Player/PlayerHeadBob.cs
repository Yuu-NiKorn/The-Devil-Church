using UnityEngine;

public class PlayerHeadBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bobPivot;
    [SerializeField] private PlayerMovement movement;

    [Header("Walking Bob")]
    [SerializeField] private float walkFrequency = 7f;
    [SerializeField] private float walkAmplitude = 0.025f;

    [Header("Running Bob")]
    [SerializeField] private float runFrequency = 10f;
    [SerializeField] private float runAmplitude = 0.04f;

    [Header("Smoothing")]
    [SerializeField] private float returnSpeed = 8f;

    private Vector3 initialPosition;
    private float timer;

    private void Start()
    {
        initialPosition = bobPivot.localPosition;
    }

    private void LateUpdate()
    {
        HandleHeadBob();
    }

    private void HandleHeadBob()
    {
        if (!movement.IsGrounded || !movement.IsMoving)
        {
            timer = 0f;

            bobPivot.localPosition = Vector3.Lerp(
                bobPivot.localPosition,
                initialPosition,
                returnSpeed * Time.deltaTime
            );

            return;
        }

        float frequency =
            movement.IsRunning
                ? runFrequency
                : walkFrequency;

        float amplitude =
            movement.IsRunning
                ? runAmplitude
                : walkAmplitude;

        timer += Time.deltaTime * frequency;

        float verticalBob =
            Mathf.Sin(timer) * amplitude;

        float horizontalBob =
            Mathf.Cos(timer * 0.5f) * amplitude * 0.5f;

        Vector3 targetPosition =
            initialPosition +
            new Vector3(
                horizontalBob,
                verticalBob,
                0f
            );

        bobPivot.localPosition = Vector3.Lerp(
            bobPivot.localPosition,
            targetPosition,
            returnSpeed * Time.deltaTime
        );
    }
}