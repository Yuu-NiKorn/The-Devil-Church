using UnityEngine;

public class DogKill : MonoBehaviour
{
    [Header("Kill Settings")]
    [SerializeField] private float killDistance = 1.2f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerDeath playerDeath;

    private bool hasKilledPlayer;

    private void Update()
    {
        if (hasKilledPlayer || player == null || playerDeath == null)
            return;
        
        Vector3 dogPosition = transform.position;
        Vector3 playerPosition = player.position;

        dogPosition.y = 0f;
        playerPosition.y = 0f;

        float distance = Vector3.Distance(
            dogPosition,
            playerPosition
        );

        if (distance <= killDistance)
        {
            hasKilledPlayer = true;
            playerDeath.KillPlayer();
        }
    }
}