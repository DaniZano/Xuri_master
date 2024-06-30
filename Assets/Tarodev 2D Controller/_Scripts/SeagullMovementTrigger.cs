using UnityEngine;

public class SeagullMovementTrigger : MonoBehaviour
{
    private SeagullMover seagullMover;
    private bool playerInTrigger = false; // Controlla se il giocatore è nel trigger

    public KeyCode activationKey = KeyCode.A; // Tasto da premere per avviare o continuare il movimento

    void Start()
    {
        seagullMover = GetComponent<SeagullMover>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(activationKey))
        {
            if (seagullMover.IsWaitingForPlayer())
            {
                seagullMover.ContinueMovement();
            }
            else if (!seagullMover.IsMoving)
            {
                seagullMover.StartMovement();
            }
        }
    }
}
