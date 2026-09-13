using UnityEngine;

public class PlayerHandBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform hand;
    [SerializeField] private PlayerMovement movement;

    [Header("Movement")]
    [SerializeField] private float walkFrequency = 7f;
    [SerializeField] private float runFrequency = 10f;

    [SerializeField] private float horizontalAmount = 0.015f;
    [SerializeField] private float verticalAmount = 0.025f;

    [Header("Rotation")]
    [SerializeField] private float rotationAmount = 2f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 10f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float timer;

    private void Start()
    {
        initialPosition = hand.localPosition;
        initialRotation = hand.localRotation;
    }

    private void LateUpdate()
    {
        HandleHandMovement();
    }

    private void HandleHandMovement()
    {
        if (!movement.IsMoving || !movement.IsGrounded)
        {
            timer = 0f;

            hand.localPosition = Vector3.Lerp(
                hand.localPosition,
                initialPosition,
                smoothSpeed * Time.deltaTime
            );

            hand.localRotation = Quaternion.Slerp(
                hand.localRotation,
                initialRotation,
                smoothSpeed * Time.deltaTime
            );

            return;
        }

        float frequency =
            movement.IsRunning
                ? runFrequency
                : walkFrequency;

        timer += Time.deltaTime * frequency;

        float horizontal =
            Mathf.Cos(timer * 0.5f) * horizontalAmount;

        float vertical =
            Mathf.Abs(Mathf.Sin(timer)) * verticalAmount;

        Vector3 targetPosition =
            initialPosition +
            new Vector3(
                horizontal,
                -vertical,
                0f
            );

        Quaternion targetRotation =
            initialRotation *
            Quaternion.Euler(
                Mathf.Sin(timer) * rotationAmount,
                0f,
                Mathf.Cos(timer) * rotationAmount
            );

        hand.localPosition = Vector3.Lerp(
            hand.localPosition,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        hand.localRotation = Quaternion.Slerp(
            hand.localRotation,
            targetRotation,
            smoothSpeed * Time.deltaTime
        );
    }
}