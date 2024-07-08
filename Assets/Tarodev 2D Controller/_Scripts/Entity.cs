using UnityEngine;
using TarodevController;

public class Entity : MonoBehaviour
{
    public float speed = 2f; // Velocità di movimento
    public float patrolDistance = 5f; // Distanza massima di movimento in una direzione
    private bool isMovingRight = true;
    private bool isStunned = false;
    private float startPositionX;

    public float stunDuration = 5f; // Durata del stordimento

    private Collider2D collider2D;

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
        startPositionX = transform.position.x;
    }

    private void Update()
    {
        if (!isStunned)
        {
            Move();
        }
    }

    private void Move()
    {
        // Movimento avanti e indietro sull'asse X
        float movement = speed * Time.deltaTime;
        if (isMovingRight)
        {
            transform.Translate(movement, 0, 0);
            if (transform.position.x >= startPositionX + patrolDistance)
            {
                isMovingRight = false;
            }
        }
        else
        {
            transform.Translate(-movement, 0, 0);
            if (transform.position.x <= startPositionX - patrolDistance)
            {
                isMovingRight = true;
            }
        }
    }

    public void Stun()
    {
        if (!isStunned)
        {
            isStunned = true;
            // Ferma il movimento
            collider2D.enabled = false; // Disabilita il collider per permettere al giocatore di attraversare il nemico
            Invoke(nameof(Recover), stunDuration); // Imposta un timer per il recupero
        }
    }

    private void Recover()
    {
        isStunned = false;
        collider2D.enabled = true; // Riabilita il collider
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Chiama la funzione di respawn dal PlayerController quando entra in contatto con il nemico
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Respawn();
            }
        }
    }
}
