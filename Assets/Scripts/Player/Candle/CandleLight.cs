using UnityEngine;

[RequireComponent(typeof(Light))]
public class CandleLight : MonoBehaviour
{
    [Header("Intensity")]
    [SerializeField] private float baseIntensity = 2f;
    [SerializeField] private float intensityVariation = 0.25f;

    [Header("Range")]
    [SerializeField] private float baseRange = 6f;
    [SerializeField] private float rangeVariation = 0.2f;

    [Header("Flicker")]
    [SerializeField] private float flickerSpeed = 8f;

    private Light candleLight;

    private float noiseOffset;

    private void Awake()
    {
        candleLight = GetComponent<Light>();

        noiseOffset = Random.Range(
            0f,
            1000f
        );
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(
            noiseOffset,
            Time.time * flickerSpeed
        );

        float centeredNoise =
            (noise - 0.5f) * 2f;

        candleLight.intensity =
            baseIntensity +
            centeredNoise * intensityVariation;

        candleLight.range =
            baseRange +
            centeredNoise * rangeVariation;
    }
}