using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerNoise : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DogHearing dogHearing;

    [Header("Noise Radius")]
    [SerializeField] private float walkingNoiseRadius = 4f;
    [SerializeField] private float runningNoiseRadius = 10f;
    [SerializeField] private float crouchingNoiseRadius = 0f;

    [Header("Input")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Noise Frequency")]
    [SerializeField] private float noiseInterval = 0.6f;

    private CharacterController controller;
    private float noiseTimer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;

        bool isMoving = horizontalVelocity.magnitude > 0.15f;
        bool isCrouching = Input.GetKey(crouchKey);
        bool isRunning = Input.GetKey(sprintKey);

        if (!isMoving)
        {
            noiseTimer = 0f;
            return;
        }

        noiseTimer += Time.deltaTime;

        if (noiseTimer < noiseInterval)
            return;

        noiseTimer = 0f;

        float noiseRadius = walkingNoiseRadius;

        if (isCrouching)
            noiseRadius = crouchingNoiseRadius;
        else if (isRunning)
            noiseRadius = runningNoiseRadius;

        EmitNoise(noiseRadius);
    }

    public void EmitNoise(float radius)
    {
        if (radius <= 0f)
            return;

        if (dogHearing != null)
            dogHearing.HearNoise(transform.position, radius);
    }
}