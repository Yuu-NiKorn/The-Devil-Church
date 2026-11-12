using UnityEngine;

public class DogPillarTrigger : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private DogBrain targetDog;

    [Header("Pillar")]
    [SerializeField] private PillarFall pillarFall;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (targetDog == null || pillarFall == null)
            return;

        DogBrain dog = other.GetComponentInParent<DogBrain>();

        if (dog != targetDog)
            return;

        hasTriggered = true;
        pillarFall.Fall();
    }
}