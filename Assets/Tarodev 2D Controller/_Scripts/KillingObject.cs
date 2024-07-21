using UnityEngine;
using TarodevController;

public class KillingObject : MonoBehaviour
{
    public Collider2D collider2D;

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("dentro");
            // Chiama la funzione di respawn dal PlayerController quando entra in contatto con il nemico
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Respawn();
            }
        }
    }

    
}
