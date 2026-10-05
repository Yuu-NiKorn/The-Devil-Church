using UnityEngine;

public class PlayerSanity : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerCandle candle;

    [Header("Sanity")]
    [SerializeField] private float maxSanity = 100f;

    [Tooltip("Santé mentale perdue par seconde dans le noir.")]
    [SerializeField] private float sanityLossPerSecond = 2f;

    [Header("Temporary Test Input")]
    [SerializeField] private KeyCode restoreSanityKey = KeyCode.M;

    private float currentSanity;

    public float CurrentSanity => currentSanity;

    public float MaxSanity => maxSanity;

    public float NormalizedSanity =>
        maxSanity <= 0f
            ? 0f
            : currentSanity / maxSanity;

    public bool IsDead =>
        currentSanity <= 0f;

    private void Start()
    {
        Debug.Log("PLAYER SANITY START !!!");

        currentSanity = maxSanity;
    }

    private void Update()
    {
        if (Input.GetKeyDown(restoreSanityKey))
        {
            RestoreSanity();
        }

        UpdateSanity();
    }
    
    
    private bool HandleDebugInput()
    {
        if (Input.GetKeyDown(restoreSanityKey))
        {
            Debug.Log("M APPUYÉ → RESTAURATION SANITY");

            RestoreSanity();

            Debug.Log("Sanity après restauration : " + currentSanity);

            return true;
        }

        return false;
    }

    private void UpdateSanity()
    {
        if (IsDead)
            return;

        if (candle != null && !candle.HasLight)
        {
            currentSanity -=
                sanityLossPerSecond *
                Time.deltaTime;

            currentSanity = Mathf.Max(
                currentSanity,
                0f
            );
        }

        if (currentSanity <= 0f)
        {
            OnSanityDepleted();
        }
    }

    public void RestoreSanity()
    {
        currentSanity = maxSanity;
    }

    private void OnSanityDepleted()
    {
        Debug.Log("GAME OVER - Sanity depleted");
    }
}