using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miniboss : MonoBehaviour
{
    public GameObject player; // Riferimento al GameObject del giocatore
    public GameObject laserPrefab; // Prefab del laser da sparare
    public float fireRate = 2f; // Intervallo di tempo tra un colpo e l'altro
    public float laserSpeed = 10f; // Velocità del laser
    public Collider2D activationTrigger; // Collider di attivazione

    private float nextFireTime = 0f;
    private bool isActive = false; // Stato di attivazione del miniboss

    bool IsColliderTrigger(Collider2D collider)
    {
        // Controlla se il collider è un trigger
        return collider.isTrigger;
    }


    void Update()
    {
        // Controlla se il miniboss è attivo e se è ora di sparare
        if (isActive && Time.time > nextFireTime)
        {
            FireLaser();
            Debug.Log("Sparaaaa");
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireLaser()
    {
        // Calcola la direzione verso il giocatore
        Vector3 direction = (player.transform.position - transform.position).normalized;

        // Crea il laser e imposta la direzione e velocità
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
        rb.velocity = direction * laserSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Attiva il miniboss se il giocatore entra nel trigger
        if (other.CompareTag("Player"))
        {
            Debug.Log("Dentro");
            isActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Disattiva il miniboss se il giocatore esce dal trigger
        if (other.CompareTag("Player"))
        {
            Debug.Log("Fuori");
            isActive = false;
        }
    }

   

}
