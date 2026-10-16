
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class WallCornerSnapEditor
{
    // Distance à laquelle les coins s'aimantent
    private const float snapDistance = 0.35f;

    private static Transform selected;
    private static bool isSnapping;

    static WallCornerSnapEditor()
    {
        EditorApplication.update += UpdateSnap;
    }

    private static void UpdateSnap()
    {
        if (isSnapping)
            return;

        selected = Selection.activeTransform;

        if (selected == null)
            return;

        BoxCollider moving =
            selected.GetComponent<BoxCollider>();

        if (moving == null || !moving.enabled)
            return;

        Bounds movingBounds = moving.bounds;

        BoxCollider[] colliders =
            Object.FindObjectsByType<BoxCollider>(
                FindObjectsSortMode.None
            );

        Vector3 bestOffset = Vector3.zero;
        float bestDistance = snapDistance;

        foreach (BoxCollider other in colliders)
        {
            if (other == moving || !other.enabled)
                continue;

            if (other.transform.IsChildOf(selected))
                continue;

            Bounds otherBounds = other.bounds;

            // On travaille sur le plan horizontal XZ
            Vector3[] movingCorners = GetCorners(movingBounds);
            Vector3[] otherCorners = GetCorners(otherBounds);

            foreach (Vector3 cornerA in movingCorners)
            {
                foreach (Vector3 cornerB in otherCorners)
                {
                    Vector3 offset = cornerB - cornerA;
                    offset.y = 0;

                    float distance = offset.magnitude;

                    if (distance < bestDistance &&
                        distance > 0.0001f)
                    {
                        bestDistance = distance;
                        bestOffset = offset;
                    }
                }
            }
        }

        if (bestOffset == Vector3.zero)
            return;

        isSnapping = true;

        selected.position += bestOffset;

        Physics.SyncTransforms();

        isSnapping = false;

        SceneView.RepaintAll();
    }

    private static Vector3[] GetCorners(Bounds bounds)
    {
        return new Vector3[]
        {
            new Vector3(bounds.min.x, bounds.center.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.center.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.center.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.center.y, bounds.max.z)
        };
    }
}
