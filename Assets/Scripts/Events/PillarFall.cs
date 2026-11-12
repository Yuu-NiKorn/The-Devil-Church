using System.Collections;
using UnityEngine;

public class PillarFall : MonoBehaviour
{
    [Header("Pillar")]
    [SerializeField] private Transform pillar;
    [SerializeField] private Transform endPosition;

    [Header("Animation")]
    [SerializeField] private float fallDuration = 1.5f;

    [SerializeField] private AnimationCurve fallCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool hasFallen;

    public void Fall()
    {
        if (hasFallen || pillar == null || endPosition == null)
            return;

        hasFallen = true;
        StartCoroutine(FallAnimation());
    }

    private IEnumerator FallAnimation()
    {
        Vector3 startPos = pillar.position;
        Quaternion startRot = pillar.rotation;

        Vector3 endPos = endPosition.position;
        Quaternion endRot = endPosition.rotation;

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, fallDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float progress = fallCurve.Evaluate(t);

            pillar.position = Vector3.LerpUnclamped(
                startPos, endPos, progress
            );

            pillar.rotation = Quaternion.SlerpUnclamped(
                startRot, endRot, progress
            );

            yield return null;
        }

        pillar.SetPositionAndRotation(endPos, endRot);
    }
}