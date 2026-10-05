using UnityEngine;

public class CandleVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform burnPivot;
    [SerializeField] private Transform flamePoint;

    [Header("Burn")]
    [Range(0.01f, 0.3f)]
    [SerializeField] private float minimumHeightPercent = 0.05f;

    [Tooltip("Distance parcourue par la flamme pendant toute la combustion.")]
    [SerializeField] private float flameTravelDistance = 0.3f;

    private Vector3 initialPivotScale;
    private Vector3 initialFlamePosition;

    private void Awake()
    {
        if (burnPivot == null || flamePoint == null)
        {
            Debug.LogError(
                "CandleVisual : Burn Pivot ou Flame Point n'est pas assigné."
            );

            enabled = false;
            return;
        }

        initialPivotScale = burnPivot.localScale;
        initialFlamePosition = flamePoint.localPosition;
    }

    public void SetBurnAmount(float amount)
    {
        amount = Mathf.Clamp01(amount);
        

        float heightPercent = Mathf.Lerp(
            minimumHeightPercent,
            1f,
            amount
        );

        float burnedPercent = 1f - amount;
        

        Vector3 scale = initialPivotScale;

        scale.z =
            initialPivotScale.z * heightPercent;

        burnPivot.localScale = scale;
        

        Vector3 flamePosition = initialFlamePosition;

        flamePosition.z =
            initialFlamePosition.z -
            flameTravelDistance * burnedPercent;

        flamePoint.localPosition = flamePosition;
    }
}