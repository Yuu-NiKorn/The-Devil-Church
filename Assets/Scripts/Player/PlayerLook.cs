using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform lookPivot;

    [Header("Mouse")]
    [SerializeField] private float sensitivityX = 2f;
    [SerializeField] private float sensitivityY = 2f;

    [Header("Vertical Look")]
    [SerializeField] private float minLookAngle = -85f;
    [SerializeField] private float maxLookAngle = 85f;

    [Header("Cursor")]
    [SerializeField] private bool lockCursor = true;

    private float pitch;

    private void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        HandleLook();
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivityY;

        transform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(
            pitch,
            minLookAngle,
            maxLookAngle
        );

        lookPivot.localRotation = Quaternion.Euler(
            pitch,
            0f,
            0f
        );
    }
}