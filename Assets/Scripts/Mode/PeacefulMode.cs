using UnityEngine;
using UnityEngine.SceneManagement;

public class PeacefulMode : MonoBehaviour
{
    public static PeacefulMode Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.P;
    [SerializeField] private KeyCode toggleKeyRestart = KeyCode.P;
    [SerializeField] private bool peacefulOnStart = false;

    [Header("Dog")]
    [SerializeField] private GameObject dog;

    private bool isPeaceful;

    public bool IsPeaceful => isPeaceful;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        isPeaceful = peacefulOnStart;
    }

    private void Start()
    {
        ApplyMode();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isPeaceful = !isPeaceful;
            ApplyMode();

            Debug.Log(
                "Peaceful Mode : " +
                (isPeaceful ? "ON" : "OFF")
            );
        }
        if (Input.GetKeyDown(toggleKeyRestart))
        {

            Replay();
        }
    }
    
    public void Replay()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    private void ApplyMode()
    {
        if (isPeaceful && dog != null)
            dog.SetActive(false);
    }

    public bool CanSpawnDog()
    {
        return !isPeaceful;
    }
}