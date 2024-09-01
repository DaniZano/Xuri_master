using UnityEngine;

public class SeagullMovementTrigger : MonoBehaviour
{
    public AudioClip flySound;
    private AudioSource audioSource;
    private SeagullMover seagullMover;
    private bool playerInTrigger = false; // Controlla se il giocatore � nel trigger

    public KeyCode activationKey = KeyCode.A; // Tasto da premere per avviare o continuare il movimento

    void Start()
    {
        seagullMover = GetComponent<SeagullMover>();
        audioSource = GetComponent<AudioSource>();
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
        if (playerInTrigger && (Input.GetKeyDown(activationKey) || Input.GetButtonDown("Fire2")))
        {
            if (seagullMover.IsWaitingForPlayer())
            {
                seagullMover.ContinueMovement();
                
            }
            else if (!seagullMover.IsMoving)
            {
                seagullMover.StartMovement();
                

            }
            PlayFlySound(); 
        }
    }

    void PlayFlySound()
    {
        if (audioSource != null && flySound != null)
        {
            audioSource.PlayOneShot(flySound); // Riproduci il suono del volo
        }
    }
}
