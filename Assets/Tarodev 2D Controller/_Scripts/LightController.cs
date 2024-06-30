using UnityEngine;
using TarodevController;

public class LightController : MonoBehaviour
{
    public Light pointLight;
    public float decreaseRate = 0.1f;  // Velocit� di diminuzione del range della luce per secondo
    public float resetRange = 40f;     // Range della luce quando viene resettata
    public float maxRange = 40f;       // Range massimo della luce
    public float minRange = 0f;        // Range minimo della luce
    public LayerMask enemyLayer;       // Layer dei nemici
    public float enemyRange = 20f;

    public Color colorLessThan10 = Color.red;
    public Color colorGreaterThan10 = Color.white;

    public Transform respawnPoint;     // Punto di respawn del giocatore

    private PlayerController playerController;


    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController non trovato nel GameObject o nei suoi genitori.");
        }
    }

    private void Update()
    {
        // Diminuzione del range della luce nel tempo
        if (pointLight.range > minRange)
        {
            pointLight.range -= decreaseRate * Time.deltaTime;
        }

        // Aumento del range della luce quando si preme "Ctrl" e uccisione dei nemici nel raggio
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) && playerController.UseCollectible())
        {
            ResetLightAndKillEnemies();
        }

        // Cambia colore se la luce � sotto un certo livello
        if (pointLight.range < 10f)
        {
            pointLight.color = colorLessThan10;
        }
        else
        {
            pointLight.color = colorGreaterThan10;
        }

        // Controllo per la morte del giocatore
        CheckForDeath();
    }

    private void ResetLightAndKillEnemies()
    {
        pointLight.range = resetRange;

        // Trova tutti i nemici nel raggio della luce e li uccide
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, enemyRange, enemyLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            Entity enemy = hitCollider.GetComponent<Entity>();
            if (enemy != null)
            {
                enemy.Stun(); // Uccide direttamente il nemico
            }
        }
    }

    public void ResetLightAfterRespawn()
    {
        pointLight.range = resetRange;
    }

    // Controllo per la morte del giocatore
    private void CheckForDeath()
    {
        if (pointLight.range <= 0)
        {
            playerController.Respawn();
        }
    }

    

    // Per visualizzare il raggio della luce nell'editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pointLight.range);
    }
}
