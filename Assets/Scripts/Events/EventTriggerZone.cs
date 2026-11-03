using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class EventTriggerZone : MonoBehaviour
{
    [Serializable]
    public class ZoneAction
    {
        public enum ActionType
        {
            ActivateObject,
            DeactivateObject,
            ForceDogChase,
            StopDogChase,
            OpenDoor,
            CloseDoor,
            UnlockDoor,
            CustomEvent
        }

        public ActionType actionType;

        [Header("References")]
        public GameObject targetObject;
        public DogBrain targetDog;
        public DoorController targetDoor;

        [Header("Custom Event")]
        public UnityEvent customEvent;

        public void Execute()
        {
            switch (actionType)
            {
                case ActionType.ActivateObject:
                    if (targetObject != null)
                        targetObject.SetActive(true);
                    break;

                case ActionType.DeactivateObject:
                    if (targetObject != null)
                        targetObject.SetActive(false);
                    break;

                case ActionType.ForceDogChase:
                    if (targetDog != null)
                        targetDog.StartForcedChase();
                    break;

                case ActionType.StopDogChase:
                    if (targetDog != null)
                        targetDog.StopForcedChase();
                    break;

                case ActionType.OpenDoor:
                    if (targetDoor != null)
                        targetDoor.Open();
                    break;

                case ActionType.CloseDoor:
                    if (targetDoor != null)
                        targetDoor.Close();
                    break;

                case ActionType.UnlockDoor:
                    if (targetDoor != null)
                        targetDoor.Unlock();
                    break;

                case ActionType.CustomEvent:
                    customEvent?.Invoke();
                    break;
            }
        }
    }

    [Header("Trigger")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool triggerOnlyOnce = true;

    [Header("Actions")]
    [SerializeField] private ZoneAction[] actions;

    private bool hasTriggered;

    public bool HasTriggered => hasTriggered;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnlyOnce)
            return;

        if (!other.CompareTag(playerTag))
            return;

        hasTriggered = true;

        foreach (ZoneAction action in actions)
            action.Execute();
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    public void SetTriggered(bool triggered)
    {
        hasTriggered = triggered;
    }
}