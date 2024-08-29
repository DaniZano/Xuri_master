using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class Miniboss : MonoBehaviour
{
    public GameObject player; // Riferimento al GameObject del giocatore
    public GameObject laserPrefab; // Prefab del laser da sparare
    public float fireRate = 2f; // Intervallo di tempo tra un colpo e l'altro
    public float laserSpeed = 10f; // Velocit� del laser
    public Collider2D activationTrigger; // Collider di attivazione
    public Transform laserOrigin; // Punto di origine per i laser
    public int maxHealth = 100; // Salute massima del miniboss
    public Slider healthBar; // Riferimento alla barra della salute UI
    public int damage = 10; // Danno inflitto dal potere del giocatore

    private float nextFireTime = 0f;
    private bool isActive = false; // Stato di attivazione del miniboss
    private int currentHealth; // Salute attuale del miniboss
    private bool playerInPowerArea = false; // Stato del giocatore nell'area del potere
    private bool isDefeated = false; // Stato del miniboss (sconfitto o meno)
    public float laserLifetime = 5f; // Durata del laser in secondi

    private AudioSource audioSource; // Reference to AudioSource
    public AudioClip victorySound; // Suono da riprodurre alla vittoria
    public GameObject VictoryPanel; // Pannello di vittoria

    //private List<GameObject> lasers = new List<GameObject>(); // Lista per tracciare i laser creati


    public PlayerController playerController;


    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController non trovato sul giocatore.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource non trovato sul GameObject del miniboss. Aggiungilo per riprodurre il suono di vittoria.");
        }

        if (VictoryPanel != null)
        {
            VictoryPanel.SetActive(false);
        }

        currentHealth = maxHealth;


        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            healthBar.gameObject.SetActive(false); // Inizialmente nascondi la barra della salute
        }
    }

    void Update()
    {
        // Controlla se il miniboss � attivo e se � ora di sparare
        if (isActive && !isDefeated && Time.time > nextFireTime)
        {
            FireLaser();
            nextFireTime = Time.time + fireRate;
        }

        // Controlla se il giocatore usa il potere all'interno dell'area specifica
        if (playerInPowerArea && (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.LeftControl)) && playerController.HasCollectibles())
        {
            TakeDamage(damage);
        }
    }

    void FireLaser()
    {
        // Calcola la direzione verso il giocatore
        Vector3 direction = (player.transform.position - laserOrigin.position).normalized;

        // Crea il laser e imposta la direzione e velocit�
        GameObject laser = Instantiate(laserPrefab, laserOrigin.position, Quaternion.identity);
        //lasers.Add(laser); // Aggiungi il laser alla lista

        Debug.Log("Laser creato a posizione: " + laserOrigin.position);
        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction * laserSpeed;
            Debug.Log("Velocit� del laser impostata a: " + (direction * laserSpeed));
        }
        else
        {
            Debug.LogError("Rigidbody2D non trovato nel prefab del laser.");
        }

        StartCoroutine(DestroyLaserAfterTime(laser, laserLifetime));

    }

    private IEnumerator DestroyLaserAfterTime(GameObject laser, float time)
    {
        yield return new WaitForSeconds(time);
        if (laser != null)
        {
            Destroy(laser);
        }
    }

    void TakeDamage(int damage)
    {
        if (isDefeated) return; // Se gi� sconfitto, non applicare danno

        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
        if (currentHealth <= 0)
        {
            // Logica per gestire la morte del miniboss
            isDefeated = true;
            Debug.Log("Miniboss sconfitto!");
            StartCoroutine(HandleMinibossDefeat());
            // Puoi aggiungere animazioni di morte, disattivazione del miniboss, ecc.
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Controlla se il giocatore entra nell'area di attivazione del miniboss
        if (other.gameObject == player)
        {
            //if (other == activationTrigger)
            //{
            isActive = true;
            Debug.Log("Giocatore entrato nel trigger, miniboss attivato.");
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(true); // Mostra la barra della salute
            }
            //}
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Controlla se il giocatore esce dall'area di attivazione del miniboss
        if (other.gameObject == player)
        {
            //if (other == activationTrigger)
            //{
            isActive = false;
            Debug.Log("Giocatore uscito dal trigger, miniboss disattivato.");
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false); // Nascondi la barra della salute
            }
            //}
        }
    }

    private IEnumerator HandleMinibossDefeat()
    {
        isDefeated = true;
        Debug.Log("Miniboss sconfitto!");

        // Display victory panel
        if (VictoryPanel != null)
        {
            VictoryPanel.SetActive(true);
        }

        // Play the victory sound
        if (audioSource != null && victorySound != null)
        {
            audioSource.clip = victorySound; // Set victory sound
            audioSource.Play(); // Play it
        }

        // Wait for 5 seconds before loading the MainMenu
        yield return new WaitForSeconds(5f);

        // Load the MainMenu scene
        SceneManager.LoadScene("final_scene");
    }

    // Metodo per gestire l'entrata del giocatore nell'area di attivazione del potere
    public void PlayerEnteredPowerArea()
    {
        playerInPowerArea = true;
        Debug.Log("Giocatore entrato nell'area di attivazione del potere.");
    }

    // Metodo per gestire l'uscita del giocatore dall'area di attivazione del potere
    public void PlayerExitedPowerArea()
    {
        playerInPowerArea = false;
        Debug.Log("Giocatore uscito dall'area di attivazione del potere.");
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
        isDefeated = false;
        //ClearLasers(); // Rimuovi i proiettili

    }

    //void ClearLasers()
    //{
    //    foreach (var laser in lasers)
    //    {
    //        if (laser != null)
    //        {
    //            Destroy(laser);
    //        }
    //    }
    //    lasers.Clear();
    //}
}