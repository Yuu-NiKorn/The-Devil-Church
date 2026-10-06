
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerSanity sanity;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private CanvasGroup deathScreen;
    [SerializeField] private Image corruptionFlash;

    [Header("Death Animation")]
    [SerializeField] private float corruptionDuration = 2f;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private bool isDying;

    private void Start()
    {
        if (deathScreen != null)
        {
            deathScreen.alpha = 0f;
            deathScreen.interactable = false;
            deathScreen.blocksRaycasts = false;
        }

        SetFlashAlpha(0f);

        if (sanity != null)
            sanity.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        if (sanity != null)
            sanity.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (isDying)
            return;

        isDying = true;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Bloquer les contrôles.
        if (movement != null)
            movement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        float timer = 0f;

        // Corruption brutale de l'écran.
        while (timer < corruptionDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(timer / Mathf.Max(0.01f, corruptionDuration));

            float flicker = Random.Range(0.1f, 0.9f);

            SetFlashAlpha(flicker * progress);

            yield return null;
        }

        // Fondu vers l'écran de mort.
        timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(timer / Mathf.Max(0.01f, fadeDuration));

            if (deathScreen != null)
                deathScreen.alpha = progress;

            SetFlashAlpha(1f - progress);

            yield return null;
        }

        if (deathScreen != null)
        {
            deathScreen.alpha = 1f;
            deathScreen.interactable = true;
            deathScreen.blocksRaycasts = true;
        }

        SetFlashAlpha(0f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SetFlashAlpha(float alpha)
    {
        if (corruptionFlash == null)
            return;

        Color color = corruptionFlash.color;
        color.a = Mathf.Clamp01(alpha);
        corruptionFlash.color = color;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        if (Application.CanStreamedLevelBeLoaded(mainMenuScene))
            SceneManager.LoadScene(mainMenuScene);
        else
            Debug.LogWarning(
                "MainMenu : scène introuvable dans les scènes du build."
            );
    }
}
