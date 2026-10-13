using UnityEngine;

[CreateAssetMenu(
    fileName = "NewNote",
    menuName = "The Devil Church/Inventory/Note"
)]
public class NoteDataSO : ScriptableObject
{
    [Header("Note Information")]
    public string noteID;
    public string noteTitle;

    [Header("Content")]
    [TextArea(10, 40)]
    public string noteText;
}