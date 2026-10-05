using UnityEngine;

public class PlayerCandle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CandleVisual candleVisual;
    [SerializeField] private Light candleLight;

    [Header("Candle Lifetime")]
    [Tooltip("Durée d'une bougie complète en secondes.")]
    [SerializeField] private float candleDuration = 300f;

    [Header("Temporary Test Input")]
    [SerializeField] private KeyCode replaceCandleKey = KeyCode.B;

    private float remainingTime;

    public bool HasLight { get; private set; }

    public bool IsBurnedOut =>
        remainingTime <= 0f;

    public float RemainingNormalized =>
        candleDuration <= 0f
            ? 0f
            : Mathf.Clamp01(remainingTime / candleDuration);

    private void Start()
    {
        ReplaceCandle();
    }

    private void Update()
    {
        HandleDebugInput();
        UpdateCandle();
    }

    private void HandleDebugInput()
    {
        if (Input.GetKeyDown(replaceCandleKey))
        {
            ReplaceCandle();
        }
    }

    private void UpdateCandle()
    {
        if (IsBurnedOut)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;

            Extinguish();
        }

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (candleVisual != null)
        {
            candleVisual.SetBurnAmount(
                RemainingNormalized
            );
        }
    }

    private void Extinguish()
    {
        HasLight = false;

        if (candleLight != null)
        {
            candleLight.enabled = false;
        }
    }

    public void ReplaceCandle()
    {
        remainingTime = candleDuration;

        HasLight = true;

        if (candleLight != null)
        {
            candleLight.enabled = true;
        }

        UpdateVisual();
    }
}