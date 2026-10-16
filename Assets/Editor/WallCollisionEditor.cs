
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class WallCollisionEditor
{
    private static Transform selectedTransform;
    private static Vector3 lastValidPosition;
    private static bool correcting;

    private const float EPSILON = 0.00001f;

    static WallCollisionEditor()
    {
        Selection.selectionChanged += OnSelectionChanged;
        EditorApplication.update += CheckCollision;

        OnSelectionChanged();
    }

    private static void OnSelectionChanged()
    {
        selectedTransform = Selection.activeTransform;

        if (selectedTransform != null)
            lastValidPosition = selectedTransform.position;
    }

    private static void CheckCollision()
    {
        if (correcting || selectedTransform == null)
            return;

        BoxCollider moving =
            selectedTransform.GetComponent<BoxCollider>();

        if (moving == null || !moving.enabled)
            return;

        Vector3 target = selectedTransform.position;
        Vector3 delta = target - lastValidPosition;

        if (delta.sqrMagnitude < EPSILON * EPSILON)
            return;

        // On repart de la dernière position valide.
        correcting = true;
        selectedTransform.position = lastValidPosition;
        Physics.SyncTransforms();

        Vector3 position = lastValidPosition;

        // Déplacement séparé sur chaque axe.
        // Cela permet de glisser le long des obstacles.
        for (int axis = 0; axis < 3; axis++)
        {
            float movement = delta[axis];

            if (Mathf.Abs(movement) < EPSILON)
                continue;

            float allowed = movement;

            Bounds movingBounds = moving.bounds;

            Collider[] obstacles =
                Object.FindObjectsByType<Collider>(
                    FindObjectsSortMode.None
                );

            foreach (Collider obstacle in obstacles)
            {
                if (obstacle == moving || !obstacle.enabled)
                    continue;

                // Ignore les colliders appartenant
                // au même objet ou à ses enfants.
                if (obstacle.transform == selectedTransform ||
                    obstacle.transform.IsChildOf(selectedTransform))
                    continue;

                Bounds other = obstacle.bounds;

                bool overlapsOtherAxes = true;

                for (int otherAxis = 0; otherAxis < 3; otherAxis++)
                {
                    if (otherAxis == axis)
                        continue;

                    if (movingBounds.max[otherAxis] <=
                            other.min[otherAxis] + EPSILON ||
                        movingBounds.min[otherAxis] >=
                            other.max[otherAxis] - EPSILON)
                    {
                        overlapsOtherAxes = false;
                        break;
                    }
                }

                if (!overlapsOtherAxes)
                    continue;

                if (movement > 0)
                {
                    float distance =
                        other.min[axis] -
                        movingBounds.max[axis];

                    if (distance >= -EPSILON)
                        allowed = Mathf.Min(
                            allowed,
                            Mathf.Max(0, distance)
                        );
                }
                else
                {
                    float distance =
                        other.max[axis] -
                        movingBounds.min[axis];

                    if (distance <= EPSILON)
                        allowed = Mathf.Max(
                            allowed,
                            Mathf.Min(0, distance)
                        );
                }
            }

            position[axis] += allowed;
            selectedTransform.position = position;
            Physics.SyncTransforms();
        }

        lastValidPosition = position;
        correcting = false;

        SceneView.RepaintAll();
    }
}