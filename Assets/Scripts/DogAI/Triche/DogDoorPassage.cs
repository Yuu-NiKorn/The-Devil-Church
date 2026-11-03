using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DogDoorPassage : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private DoorController door;
    [SerializeField] private Collider[] doorColliders;

    [Header("Dog")]
    [SerializeField] private DogBrain dog;
    [SerializeField] private Collider[] dogColliders;

    private readonly HashSet<Collider> ignoredColliders = new();

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dog == null || door == null)
            return;

        if (!other.transform.IsChildOf(dog.transform))
            return;

        if (!door.IsOpen)
            return;

        SetDoorCollisions(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (dog == null)
            return;

        if (!other.transform.IsChildOf(dog.transform))
            return;

        SetDoorCollisions(true);
    }

    private void SetDoorCollisions(bool enabled)
    {
        foreach (Collider dogCollider in dogColliders)
        {
            if (dogCollider == null)
                continue;

            foreach (Collider doorCollider in doorColliders)
            {
                if (doorCollider == null)
                    continue;

                Physics.IgnoreCollision(
                    dogCollider,
                    doorCollider,
                    !enabled
                );

                if (enabled)
                    ignoredColliders.Remove(dogCollider);
                else
                    ignoredColliders.Add(dogCollider);
            }
        }
    }

    private void OnDisable()
    {
        SetDoorCollisions(true);
    }
}