using UnityEngine;
using UnityEngine.UI;

public class SanityBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerSanity playerSanity;
    [SerializeField] private Image sanityFill;

    [Header("Animation")]
    [SerializeField] private float smoothSpeed = 5f;

    private float displayedSanity = 1f;

    private void Start()
    {
        if (playerSanity != null)
        {
            displayedSanity = playerSanity.NormalizedSanity;
        }

        UpdateBarInstantly();
    }

    private void Update()
    {
        if (playerSanity == null || sanityFill == null)
            return;

        float targetSanity =
            playerSanity.NormalizedSanity;

        displayedSanity = Mathf.MoveTowards(
            displayedSanity,
            targetSanity,
            smoothSpeed * Time.deltaTime
        );

        sanityFill.fillAmount = displayedSanity;
    }

    private void UpdateBarInstantly()
    {
        if (playerSanity == null || sanityFill == null)
            return;

        sanityFill.fillAmount =
            playerSanity.NormalizedSanity;
    }
}