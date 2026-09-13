using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 16f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float groundedForce = -2f;

    [Header("Inputs")]
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    private CharacterController controller;

    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    public Vector3 HorizontalVelocity => horizontalVelocity;

    public float CurrentSpeed => horizontalVelocity.magnitude;

    public bool IsMoving => horizontalVelocity.sqrMagnitude > 0.01f;

    public bool IsRunning { get; private set; }

    public bool IsGrounded => controller.isGrounded;

    public float SpeedMultiplier { get; set; } = 1f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();
        ApplyMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection.sqrMagnitude > 1f)
            inputDirection.Normalize();

        Vector3 moveDirection =
            transform.right * inputDirection.x +
            transform.forward * inputDirection.z;

        IsRunning =
            Input.GetKey(runKey) &&
            vertical > 0f &&
            inputDirection.sqrMagnitude > 0f;

        float targetSpeed = IsRunning ? runSpeed : walkSpeed;
        targetSpeed *= SpeedMultiplier;

        Vector3 targetVelocity = moveDirection * targetSpeed;

        float smoothRate =
            targetVelocity.sqrMagnitude > horizontalVelocity.sqrMagnitude
                ? acceleration
                : deceleration;

        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            targetVelocity,
            smoothRate * Time.deltaTime
        );
    }

    private void HandleGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedForce;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void ApplyMovement()
    {
        Vector3 velocity = horizontalVelocity;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}