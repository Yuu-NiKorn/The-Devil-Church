using UnityEngine;

public class VisibilitySource : MonoBehaviour
{
    [Header("Light")]
    public Light sourceLight;

    [Header("Visibility")]
    [Min(0f)] public float innerRadius = 2f;
    [Min(0.01f)] public float outerRadius = 6f;

    [Range(0f, 1f)]
    public float visibility = 1f;

    public bool IsVisible =>
        isActiveAndEnabled &&
        (sourceLight == null ||
        (sourceLight.isActiveAndEnabled &&
         sourceLight.intensity > 0.001f));

    private void OnEnable()
    {
        VisibilityManager.Register(this);
    }

    private void OnDisable()
    {
        VisibilityManager.Unregister(this);
    }

    private void Reset()
    {
        sourceLight = GetComponent<Light>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
    }
}