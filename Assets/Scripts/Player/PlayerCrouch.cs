using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform viewRoot;
    [SerializeField] private PlayerMovement movement;

    [Header("Input")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Camera Height")]
    [SerializeField] private float standingViewHeight = 1.65f;
    [SerializeField] private float crouchingViewHeight = 1.05f;

    [Header("Movement")]
    [Range(0.1f, 1f)]
    [SerializeField] private float crouchSpeedMultiplier = 0.55f;

    [Header("Transition")]
    [SerializeField] private float transitionSpeed = 8f;

    public bool IsCrouching { get; private set; }

    public float StandingViewHeight => standingViewHeight;
    public float CrouchingViewHeight => crouchingViewHeight;

    private float externalHeight = -1f;
    
    private CharacterController controller;
    
    public CharacterController Controller => controller;
    
    private bool wasCrouching;
    
    private bool forceCrouch;

    public bool IsForcedCrouching => forceCrouch;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleInput();
        UpdateViewHeight();
    }

    private void HandleInput()
    {
        bool crouchInput = Input.GetKey(crouchKey);

        IsCrouching = crouchInput || forceCrouch;

        if (IsCrouching && !wasCrouching)
        {
            SetControllerHeight(1.0f);
        }
        else if (!IsCrouching && wasCrouching)
        {
            SetControllerHeight(1.8f);
        }

        wasCrouching = IsCrouching;

        if (movement != null)
        {
            movement.SpeedMultiplier = IsCrouching
                ? crouchSpeedMultiplier
                : 1f;
        }
    }

    private void UpdateViewHeight()
    {
        if (viewRoot == null)
            return;

        float targetHeight;

        // Si un autre système demande une hauteur spécifique
        // (par exemple être sous une table)
        if (IsCrouching && externalHeight >= 0f)
        {
            targetHeight = Mathf.Min(
                crouchingViewHeight,
                externalHeight
            );
        }
        else
        {
            targetHeight = IsCrouching
                ? crouchingViewHeight
                : standingViewHeight;
        }

        Vector3 position = viewRoot.localPosition;

        position.y = Mathf.MoveTowards(
            position.y,
            targetHeight,
            transitionSpeed * Time.deltaTime
        );

        viewRoot.localPosition = position;
    }

    public void SetExternalHeight(float height)
    {
        externalHeight = height;
    }

    public void ClearExternalHeight()
    {
        externalHeight = -1f;
    }
    
    public void SetLowClearanceControllerHeight(float height)
    {
        if (!IsCrouching)
            return;

        SetControllerHeight(height);
    }
    
    private void SetControllerHeight(float height)
    {
        controller.height = height;

        Vector3 center = controller.center;

        center.y = height * 0.5f;

        controller.center = center;
    }
    
    public void SetForcedCrouch(bool forced)
    {
        forceCrouch = forced;
    }
}