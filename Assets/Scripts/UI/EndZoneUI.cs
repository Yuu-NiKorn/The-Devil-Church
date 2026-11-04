using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZoneUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject endPanel;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerInteractor playerInteractor;

    private bool isOpen;

    private void Start()
    {
        if (endPanel != null)
            endPanel.SetActive(false);
    }

    public void ShowEndUI()
    {
        if (isOpen)
            return;

        isOpen = true;

        if (endPanel != null)
            endPanel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        if (playerInteractor != null)
            playerInteractor.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Replay()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}