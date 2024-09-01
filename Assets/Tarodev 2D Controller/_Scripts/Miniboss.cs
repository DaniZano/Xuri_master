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
    public float laserSpeed = 10f; // Velocità del laser
    public Collider2D activationTrigger; // Collider di attivazione
    public Transform laserOrigin; // Punto di origine per i laser
    public int maxHealth = 100; // Salute massima del miniboss
    public Slider healthBar; // Riferimento alla barra della salute UI
    public int damage = 10; // Danno inflitto dal potere del giocatore

    public AudioClip victorySound; // Suono da riprodurre alla vittoria
    public GameObject VictoryCanvas; // Riferimento al Canvas che contiene tutto
    public GameObject VictoryPanel; // Riferimento diretto al Panel dentro il Canvas
    public float fadeDuration = 0.5f; // Durata del fade in/fade out in secondi

    private float nextFireTime = 0f;
    private bool isActive = false; // Stato di attivazione del miniboss
    private int currentHealth; // Salute attuale del miniboss
    private bool playerInPowerArea = false; // Stato del giocatore nell'area del potere
    private bool isDefeated = false; // Stato del miniboss (sconfitto o meno)
    private float laserLifetime = 5f; // Durata del laser in secondi

    private AudioSource audioSource; // Riferimento a AudioSource
    private List<GameObject> activeLasers = new List<GameObject>(); // Lista per tracciare i laser attivi
    private Image panelImage; // Riferimento al componente Image del VictoryPanel
    private PlayerController playerController;

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
            // Otteniamo il componente Image dal Panel
            panelImage = VictoryPanel.GetComponent<Image>();

            if (panelImage == null)
            {
                Debug.LogError("VictoryPanel non ha un componente Image!");
                return;
            }

            // Imposta l'alpha iniziale a 0 (trasparente)
            Color color = panelImage.color;
            color.a = 0;
            panelImage.color = color;

            // Disabilita il Canvas all'inizio
            VictoryCanvas.SetActive(false);
        }

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
        // Controlla se il miniboss è attivo e se è ora di sparare
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

        // Crea il laser e imposta la direzione e velocità
        GameObject laser = Instantiate(laserPrefab, laserOrigin.position, Quaternion.identity);

        // Aggiungi il laser alla lista dei laser attivi
        activeLasers.Add(laser);

        Debug.Log("Laser creato a posizione: " + laserOrigin.position);
        Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = direction * laserSpeed;
            Debug.Log("Velocità del laser impostata a: " + (direction * laserSpeed));
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
            activeLasers.Remove(laser); // Rimuovi il laser dalla lista prima di distruggerlo
            Destroy(laser);
        }
    }

    void TakeDamage(int damage)
    {
        if (isDefeated) return; // Se già sconfitto, non applicare danno

        currentHealth -= damage;
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            isDefeated = true;
            Debug.Log("Miniboss sconfitto!");

            // Disattiva il prefab del laser
            if (laserPrefab != null)
            {
                laserPrefab.SetActive(false);
                Debug.Log("LaserPrefab disattivato.");
            }

            // Disattiva tutti i laser attivi
            foreach (var laser in activeLasers)
            {
                if (laser != null)
                {
                    laser.SetActive(false);
                }
            }

            // Svuota la lista dei laser attivi
            activeLasers.Clear();

            StartCoroutine(HandleMinibossDefeat());
        }
    }

    private IEnumerator HandleMinibossDefeat()
    {
        isDefeated = true;
        Debug.Log("Miniboss sconfitto!");

        // Mostra il Canvas e inizia il fade in del pannello
        if (VictoryCanvas != null)
        {
            VictoryCanvas.SetActive(true);
            yield return StartCoroutine(Fade(0, 1)); // Fade In
        }

        // Riproduci il suono della vittoria
        if (audioSource != null && victorySound != null)
        {
            audioSource.clip = victorySound;
            audioSource.Play();
        }

        // Attendi 5 secondi
        yield return new WaitForSeconds(5f);


        // Distruggi UIManager e UIMainMenu, se esistono
        if (UIManager.Instance != null)
        {
            Destroy(UIManager.Instance.gameObject);
        }

        if (UIMainMenu.Instance != null)
        {
            Destroy(UIMainMenu.Instance.gameObject);
        }

        // Carica la scena del Main Menu
        SceneManager.LoadScene("Main Menu");
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0.0f;
        Color color = panelImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            panelImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        panelImage.color = color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Controlla se il giocatore entra nell'area di attivazione del miniboss
        if (other.gameObject == player)
        {
            isActive = true;
            Debug.Log("Giocatore entrato nel trigger, miniboss attivato.");
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(true); // Mostra la barra della salute
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Controlla se il giocatore esce dall'area di attivazione del miniboss
        if (other.gameObject == player)
        {
            isActive = false;
            Debug.Log("Giocatore uscito dal trigger, miniboss disattivato.");
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(false); // Nascondi la barra della salute
            }
        }
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
    }
}
