using System;
using UnityEngine;

public class PlayerSanity : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerCandle candle;

    [Header("Sanity")]
    [SerializeField, Min(1f)] private float maxSanity = 100f;
    [SerializeField, Min(0f)] private float sanityLossPerSecond = 10f;

    [Header("Temporary Test Input")]
    [SerializeField] private KeyCode restoreSanityKey = KeyCode.N;

    private float currentSanity;
    private bool isDead;

    public float CurrentSanity => currentSanity;
    public float MaxSanity => maxSanity;
    public float NormalizedSanity =>
        Mathf.Clamp01(currentSanity / maxSanity);

    public bool IsDead => isDead;

    public event Action OnDeath;

    private void Awake()
    {
        currentSanity = maxSanity;
    }

    private void Update()
    {
        if (isDead)
            return;

        if (Input.GetKeyDown(restoreSanityKey))
            RestoreSanity();

        if (candle != null && !candle.HasLight)
        {
            currentSanity = Mathf.Max(
                0f,
                currentSanity - sanityLossPerSecond * Time.deltaTime
            );
        }

        if (currentSanity <= 0f)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }

    public void RestoreSanity()
    {
        if (isDead)
            return;

        currentSanity = maxSanity;
    }
}