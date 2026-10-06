
using UnityEngine;
using UnityEngine.UI;

public class SanityVisionEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerSanity sanity;
    [SerializeField] private Image darknessOverlay;
    [SerializeField] private Image redOverlay;
    [SerializeField] private Image glitchBand;

    [Header("Darkness")]
    [Range(0f, 1f)]
    [SerializeField] private float maxDarkness = 0.78f;

    [Header("Corruption")]
    [Range(0f, 1f)]
    [SerializeField] private float maxRedAlpha = 0.16f;

    [SerializeField] private float glitchSpeed = 12f;

    private RectTransform glitchRect;
    private float glitchTimer;

    private void Awake()
    {
        if (glitchBand != null)
            glitchRect = glitchBand.rectTransform;
    }

    private void Update()
    {
        if (sanity == null)
            return;

        float corruption = 1f - sanity.NormalizedSanity;

        // L'obscurité commence doucement puis s'intensifie.
        float darkness = Mathf.Pow(corruption, 1.5f);

        SetAlpha(
            darknessOverlay,
            darkness * maxDarkness
        );

        // La teinte rouge apparaît surtout à faible santé.
        float danger = Mathf.InverseLerp(
            0.65f, 1f, corruption
        );

        SetAlpha(
            redOverlay,
            danger * maxRedAlpha
        );

        UpdateGlitch(danger);
    }

    private void UpdateGlitch(float danger)
    {
        if (glitchBand == null || glitchRect == null)
            return;

        if (danger <= 0f)
        {
            SetAlpha(glitchBand, 0f);
            return;
        }

        glitchTimer -= Time.deltaTime;

        if (glitchTimer > 0f)
            return;

        glitchTimer = 1f / Mathf.Max(1f, glitchSpeed);

        bool visible = Random.value < danger * 0.7f;

        SetAlpha(
            glitchBand,
            visible ? Random.Range(0.08f, 0.35f) * danger : 0f
        );

        // Une bande horizontale parasite aléatoire.
        glitchRect.anchorMin = new Vector2(0f, Random.value);
        glitchRect.anchorMax = new Vector2(1f, glitchRect.anchorMin.y);

        glitchRect.pivot = new Vector2(0.5f, 0.5f);
        glitchRect.anchoredPosition = Vector2.zero;
        glitchRect.sizeDelta = new Vector2(
            0f,
            Random.Range(3f, 35f)
        );
    }

    private void SetAlpha(Image image, float alpha)
    {
        if (image == null)
            return;

        Color color = image.color;
        color.a = Mathf.Clamp01(alpha);
        image.color = color;
    }
}
