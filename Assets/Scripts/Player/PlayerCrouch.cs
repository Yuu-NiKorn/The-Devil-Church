using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCrouch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform viewRoot;
    [SerializeField] private Transform leftHand;
    [SerializeField] private PlayerMovement movement;

    [Header("Input")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Camera Height")]
    [SerializeField] private float standingViewHeight = 1.65f;
    [SerializeField] private float crouchingViewHeight = 1.05f;

    [Header("Hand")]
    [SerializeField] private float handCrouchOffset = -0.25f;

    [Header("Movement")]
    [Range(0.1f, 1f)]
    [SerializeField] private float crouchSpeedMultiplier = 0.55f;

    [Header("Controller Height")]
    [SerializeField] private float standingControllerHeight = 1.8f;
    [SerializeField] private float crouchingControllerHeight = 1.1f;

    [Header("Ceiling Check")]
    [Tooltip("Layers considérées comme obstacles au-dessus de la tête")]
    [SerializeField] private LayerMask ceilingMask = ~0;
    [Tooltip("Marge de sécurité ajoutée au rayon lors du sphere cast de plafond")]
    [SerializeField] private float ceilingCheckSkin = 0.02f;

    [Header("Transition")]
    [SerializeField] private float transitionSpeed = 8f;

    private CharacterController controller;

    private Vector3 handStandingPosition;
    private float standingCenterY;
    private float crouchingCenterY;

    private bool wantsToStand;

    public bool IsCrouching { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (leftHand != null)
        {
            handStandingPosition = leftHand.localPosition;
        }
        
        standingCenterY = standingControllerHeight / 2f;
        crouchingCenterY = crouchingControllerHeight / 2f;
        
        controller.height = standingControllerHeight;
        controller.center = new Vector3(controller.center.x, standingCenterY, controller.center.z);

        IsCrouching = false;
    }

    private void Update()
    {
        HandleInput();
        UpdateController();
        UpdateCamera();
        UpdateHand();
    }

    private void HandleInput()
    {
        bool crouchHeld = Input.GetKey(crouchKey);

        if (crouchHeld)
        {
            IsCrouching = true;
        }
        else
        {
            IsCrouching = HasCeilingAbove();
        }

        if (movement != null)
        {
            movement.SpeedMultiplier = IsCrouching
                ? crouchSpeedMultiplier
                : 1f;
        }
    }
    
    private bool HasCeilingAbove()
    {
        float radius = Mathf.Max(0.01f, controller.radius - ceilingCheckSkin);
        
        Vector3 origin = transform.position + Vector3.up * (controller.height - radius);

        float castDistance = standingControllerHeight - controller.height;

        if (castDistance <= 0f)
        {
            return false;
        }

        bool blocked = Physics.SphereCast(
            origin,
            radius,
            Vector3.up,
            out _,
            castDistance,
            ceilingMask,
            QueryTriggerInteraction.Ignore
        );

        return blocked;
    }

    private void UpdateController()
    {
        float targetHeight = IsCrouching
            ? crouchingControllerHeight
            : standingControllerHeight;

        float targetCenterY = IsCrouching
            ? crouchingCenterY
            : standingCenterY;

        controller.height = Mathf.Lerp(
            controller.height,
            targetHeight,
            transitionSpeed * Time.deltaTime
        );

        Vector3 center = controller.center;
        center.y = Mathf.Lerp(center.y, targetCenterY, transitionSpeed * Time.deltaTime);
        controller.center = center;
    }

    private void UpdateCamera()
    {
        if (viewRoot == null)
            return;

        float targetHeight = IsCrouching
            ? crouchingViewHeight
            : standingViewHeight;

        Vector3 position = viewRoot.localPosition;

        position.y = Mathf.Lerp(
            position.y,
            targetHeight,
            transitionSpeed * Time.deltaTime
        );

        viewRoot.localPosition = position;
    }

    private void UpdateHand()
    {
        if (leftHand == null)
            return;

        Vector3 targetPosition = handStandingPosition;

        if (IsCrouching)
        {
            targetPosition.y += handCrouchOffset;
        }

        leftHand.localPosition = Vector3.Lerp(
            leftHand.localPosition,
            targetPosition,
            transitionSpeed * Time.deltaTime
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (controller == null)
            return;

        Gizmos.color = Color.cyan;
        float radius = Mathf.Max(0.01f, controller.radius - ceilingCheckSkin);
        Vector3 origin = transform.position + Vector3.up * (controller.height - radius);
        Gizmos.DrawWireSphere(origin, radius);
        Gizmos.DrawWireSphere(origin + Vector3.up * (standingControllerHeight - controller.height), radius);
    }
#endif
}