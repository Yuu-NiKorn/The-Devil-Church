
using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup dialogueCanvas;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Audio")]
    [SerializeField] private AudioSource voiceSource;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerInteractor playerInteractor;

    [Header("Settings")]
    [SerializeField] private KeyCode nextLineKey = KeyCode.E;
    [SerializeField, Min(0.001f)]
    private float letterDelay = 0.035f;

    private DialogueDataSO currentDialogue;
    private Coroutine typingCoroutine;
    private int currentLineIndex;

    private bool isTyping;
    private bool isDialogueActive;

    public bool IsDialogueActive => isDialogueActive;

    public event Action OnDialogueFinished;

    private void Start()
    {
        HideDialogue();
    }

    private void Update()
    {
        if (!isDialogueActive)
            return;

        if (Input.GetKeyDown(nextLineKey))
        {
            if (isTyping)
                CompleteCurrentLine();
            else
                NextLine();
        }
    }

    public void StartDialogue(DialogueDataSO dialogue)
    {
        if (isDialogueActive ||
            dialogue == null ||
            dialogue.lines == null ||
            dialogue.lines.Count == 0)
            return;

        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;

        dialogueCanvas.alpha = 1f;
        dialogueCanvas.interactable = true;
        dialogueCanvas.blocksRaycasts = true;

        SetPlayerControls(false);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        DialogueLine line =
            currentDialogue.lines[currentLineIndex];

        speakerText.text = line.speakerName;
        dialogueText.text = line.text ?? "";

        dialogueText.maxVisibleCharacters = 0;

        if (voiceSource != null)
        {
            voiceSource.Stop();

            if (line.voiceClip != null)
            {
                voiceSource.clip = line.voiceClip;
                voiceSource.Play();
            }
        }

        typingCoroutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;

        dialogueText.ForceMeshUpdate();

        int totalCharacters =
            dialogueText.textInfo.characterCount;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;

            yield return new WaitForSecondsRealtime(
                letterDelay
            );
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void CompleteCurrentLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.maxVisibleCharacters = int.MaxValue;
        isTyping = false;
    }

    private void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (voiceSource != null)
            voiceSource.Stop();

        isDialogueActive = false;
        currentDialogue = null;

        HideDialogue();
        SetPlayerControls(true);

        OnDialogueFinished?.Invoke();
    }

    private void HideDialogue()
    {
        if (dialogueCanvas == null)
            return;

        dialogueCanvas.alpha = 0f;
        dialogueCanvas.interactable = false;
        dialogueCanvas.blocksRaycasts = false;
    }

    private void SetPlayerControls(bool enabled)
    {
        if (playerMovement != null)
            playerMovement.enabled = enabled;

        if (playerLook != null)
            playerLook.enabled = enabled;

        if (playerInteractor != null)
            playerInteractor.enabled = enabled;
    }
}