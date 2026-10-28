using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPCQuestController questController;
    [SerializeField] private Transform interactionPoint;

    public Vector3 InteractionPosition =>
        interactionPoint != null
            ? interactionPoint.position
            : transform.position;

    public bool CanInteract =>
        questController != null &&
        questController.CanInteract;

    public void Interact()
    {
        if (CanInteract)
            questController.Interact();
    }
}