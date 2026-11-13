using UnityEngine;

[RequireComponent(typeof(DogBrain))]
public class DogKill : MonoBehaviour
{
    [Header("Kill Settings")]
    [SerializeField, Min(0.1f)] private float killDistance = 1.2f;
    [SerializeField] private bool killOnlyDuringChase = true;

    [Header("Line Of Sight")]
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private float dogCheckHeight = 0.8f;
    [SerializeField] private float playerCheckHeight = 1f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerDeath playerDeath;

    private DogBrain brain;
    private bool hasKilledPlayer;

    private void Awake()
    {
        brain = GetComponent<DogBrain>();
    }

    private void Update()
    {
        if (hasKilledPlayer || player == null || playerDeath == null)
            return;

        if (killOnlyDuringChase &&
            brain.CurrentState != DogBrain.DogState.Chase)
            return;

        Vector3 dogPosition = transform.position;
        Vector3 playerPosition = player.position;

        // Vérification de la distance horizontale.
        Vector2 dogXZ = new Vector2(
            dogPosition.x,
            dogPosition.z
        );

        Vector2 playerXZ = new Vector2(
            playerPosition.x,
            playerPosition.z
        );

        if (Vector2.Distance(dogXZ, playerXZ) > killDistance)
            return;

        // Vérification des obstacles entre le chien et le joueur.
        Vector3 start = dogPosition + Vector3.up * dogCheckHeight;
        Vector3 end = playerPosition + Vector3.up * playerCheckHeight;

        if (Physics.Linecast(
                start,
                end,
                obstacleLayers,
                QueryTriggerInteraction.Ignore))
        {
            return;
        }

        hasKilledPlayer = true;
        playerDeath.KillPlayer();
    }

    public void ResetKill()
    {
        hasKilledPlayer = false;
    }
}