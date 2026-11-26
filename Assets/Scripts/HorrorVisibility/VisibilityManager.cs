using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    private const int MaxSources = 32;

    private static readonly List<VisibilitySource> sources = new();
    
    private static readonly int HvSourceCount = Shader.PropertyToID("_HVSourceCount");
    private static readonly int HvPositions = Shader.PropertyToID("_HVPositions");
    private static readonly int HvSettings = Shader.PropertyToID("_HVSettings");
    private static readonly int HvDarkness = Shader.PropertyToID("_HVDarkness");
    private static readonly int MinBrightness = Shader.PropertyToID("_MinBrightness");

    [Header("Darkness")]
    [Range(0f, 1f)]
    public float darkness = 0.99f;
    [Range(0f, 1f)]
    public float minBrightness = 0.2f;

    private readonly Vector4[] positions = new Vector4[MaxSources];
    private readonly Vector4[] settings = new Vector4[MaxSources];

    public static void Register(VisibilitySource source)
    {
        if (!sources.Contains(source))
            sources.Add(source);
    }

    public static void Unregister(VisibilitySource source)
    {
        sources.Remove(source);
    }

    private void LateUpdate()
    {
        int count = 0;

        foreach (VisibilitySource source in sources)
        {
            if (source == null || !source.IsVisible)
                continue;

            if (count >= MaxSources)
                break;

            Vector3 pos = source.transform.position;

            float outer = Mathf.Max(0.01f, source.outerRadius);
            float inner = Mathf.Clamp(
                source.innerRadius, 0f, outer - 0.001f
            );

            positions[count] = new Vector4(
                pos.x, pos.y, pos.z, outer
            );

            settings[count] = new Vector4(
                inner, source.visibility, 0f, 0f
            );

            count++;
        }

        Shader.SetGlobalInt(HvSourceCount, count);
        Shader.SetGlobalVectorArray(HvPositions, positions);
        Shader.SetGlobalVectorArray(HvSettings, settings);
        Shader.SetGlobalFloat(HvDarkness, darkness);
        Shader.SetGlobalFloat(MinBrightness, minBrightness);
    }

    private void OnDisable()
    {
        Shader.SetGlobalInt("_HVSourceCount", 0);
        Shader.SetGlobalFloat("_HVDarkness", 0f);
    }
}