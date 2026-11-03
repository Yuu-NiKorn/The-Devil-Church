using UnityEngine;

public class DogEscapeDoor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DoorController door;
    [SerializeField] private GameObject dog;

    [Header("Settings")]
    [SerializeField] private bool lockPermanently = true;
    [SerializeField] private bool deactivateDog = true;

    private bool escapeCompleted;

    private void Awake()
    {
        if (door == null)
            door = GetComponent<DoorController>();
    }

    private void OnEnable()
    {
        if (door != null)
            door.OnDoorClosed += HandleDoorClosed;
    }

    private void OnDisable()
    {
        if (door != null)
            door.OnDoorClosed -= HandleDoorClosed;
    }

    private void HandleDoorClosed()
    {
        if (escapeCompleted)
            return;

        escapeCompleted = true;

        if (lockPermanently)
            door.LockPermanently();

        if (deactivateDog && dog != null)
            dog.SetActive(false);

        Debug.Log(
            "Le joueur a échappé au chien. Porte condamnée."
        );
    }
}