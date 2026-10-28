using UnityEngine;

public class DialogueDebugStarter : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueDataSO testDialogue;
    [SerializeField] private KeyCode testKey = KeyCode.T;

    private void Update()
    {
        if (Input.GetKeyDown(testKey) &&
            dialogueManager != null &&
            !dialogueManager.IsDialogueActive)
        {
            dialogueManager.StartDialogue(testDialogue);
        }
    }
}