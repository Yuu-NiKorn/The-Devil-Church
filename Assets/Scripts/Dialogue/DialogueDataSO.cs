using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [Header("Character")]
    public string speakerName;

    [Header("Dialogue")]
    [TextArea(3, 8)]
    public string text;

    [Header("Voice Acting")]
    public AudioClip voiceClip;
}

[CreateAssetMenu(
    fileName = "NewDialogue",
    menuName = "The Devil Church/Dialogue/Dialogue"
)]
public class DialogueDataSO : ScriptableObject
{
    [Header("Dialogue Lines")]
    public List<DialogueLine> lines = new();
}