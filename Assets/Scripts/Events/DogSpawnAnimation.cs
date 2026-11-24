using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(DogBrain))]
public class DogSpawnAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform dogVisual;
    [SerializeField] private GameObject spawnVFX;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float spawnDepth = 1.5f;
    [SerializeField, Min(0f)] private float vfxStartDelay = 1f;
    [SerializeField, Min(0f)] private float riseDuration = 3f;
    [SerializeField, Min(0f)] private float vfxFadeDuration = 2f;
    [SerializeField, Min(0f)] private float aiStartDelay = 0.5f;
    [SerializeField] private AnimationCurve riseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public bool IsSpawning { get; private set; }
    private bool queuedChase;
    private Behaviour[] behaviours;
    private bool[] behaviourStates;
    private Collider[] colliders;
    private bool[] colliderStates;
    private Vector3 visualPosition, vfxScale;

    public void QueueForcedChase() => queuedChase = true;

    public void PlaySpawn()
    {
        if (IsSpawning || gameObject.activeSelf) return;
        if (dogVisual == null || spawnVFX == null)
        {
            Debug.LogError("DogSpawnAnimation : references manquantes.", this);
            return;
        }

        IsSpawning = true;
        queuedChase = false;
        behaviours = new Behaviour[] { GetComponent<DogBrain>(), GetComponent<DogPatrol>(),
            GetComponent<DogVision>(), GetComponent<DogHearing>(), GetComponent<DogKill>(),
            GetComponent<Light>(), GetComponent<NavMeshAgent>() };
        behaviourStates = new bool[behaviours.Length];
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] == null) continue;
            behaviourStates[i] = behaviours[i].enabled;
            behaviours[i].enabled = false;
        }
        colliders = GetComponentsInChildren<Collider>(true);
        colliderStates = new bool[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            colliderStates[i] = colliders[i].enabled;
            colliders[i].enabled = false;
        }
        visualPosition = dogVisual.localPosition;
        vfxScale = spawnVFX.transform.localScale;
        dogVisual.position -= Vector3.up * spawnDepth;
        dogVisual.gameObject.SetActive(false);
        gameObject.SetActive(true);
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        spawnVFX.SetActive(true);
        yield return new WaitForSeconds(vfxStartDelay);
        dogVisual.gameObject.SetActive(true);
        Vector3 start = dogVisual.localPosition;
        for (float t = 0; t < riseDuration; t += Time.deltaTime)
        {
            dogVisual.localPosition = Vector3.LerpUnclamped(start, visualPosition, riseCurve.Evaluate(t / riseDuration));
            yield return null;
        }
        dogVisual.localPosition = visualPosition;
        for (float t = 0; t < vfxFadeDuration; t += Time.deltaTime)
        {
            spawnVFX.transform.localScale = Vector3.Lerp(vfxScale, Vector3.zero, Mathf.SmoothStep(0, 1, t / vfxFadeDuration));
            yield return null;
        }
        spawnVFX.SetActive(false);
        spawnVFX.transform.localScale = vfxScale;
        yield return new WaitForSeconds(aiStartDelay);
        RestoreComponents();
        IsSpawning = false;
        // Allow DogBrain.Start to initialize patrol before forcing a chase.
        yield return null;
        if (queuedChase) GetComponent<DogBrain>().StartForcedChase();
    }

    private void RestoreComponents()
    {
        // Restore NavMeshAgent first, before DogPatrol.OnEnable.
        for (int i = behaviours.Length - 1; i >= 0; i--)
            if (behaviours[i] != null) behaviours[i].enabled = behaviourStates[i];
        for (int i = 0; i < colliders.Length; i++)
            if (colliders[i] != null) colliders[i].enabled = colliderStates[i];
    }

    private void OnDisable()
    {
        if (!IsSpawning) return;
        StopAllCoroutines();
        dogVisual.localPosition = visualPosition;
        dogVisual.gameObject.SetActive(true);
        spawnVFX.SetActive(false);
        spawnVFX.transform.localScale = vfxScale;
        RestoreComponents();
        IsSpawning = false;
    }
}
