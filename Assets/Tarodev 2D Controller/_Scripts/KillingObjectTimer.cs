using System.Collections;
using UnityEngine;
using TarodevController;

public class KillingObjectTimer : MonoBehaviour
{
    public Collider2D triggerCollider2D;
    public float killDelay = 1.0f; // Tempo di ritardo per l'azione di killing object
    private Coroutine killCoroutine; // Riferimento alla coroutine in esecuzione

    private void Awake()
    {
        triggerCollider2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("dentro");
            // Avvia il ritardo per l'azione di killing object
            killCoroutine = StartCoroutine(KillObjectAfterDelay(collision));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("fuori");
            // Annulla la coroutine se il player esce dal collider
            if (killCoroutine != null)
            {
                StopCoroutine(killCoroutine);
                killCoroutine = null;
            }
        }
    }

    private IEnumerator KillObjectAfterDelay(Collider2D collision)
    {
        // Aspetta per il tempo specificato da killDelay
        yield return new WaitForSeconds(killDelay);

        // Chiama la funzione di respawn dal PlayerController dopo il ritardo
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Respawn();
        }
    }
}
