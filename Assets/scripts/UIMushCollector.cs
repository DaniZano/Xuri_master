using UnityEngine;
using TMPro;
using TarodevController; 

public class UIMushCollector : MonoBehaviour
{
    public TextMeshProUGUI collectibleText;
    public GameObject mushCanvas; // Canvas che mostra il conteggio dei funghi
    private PlayerController playerController;
    private bool canvasVisible = false; // Per tenere traccia dello stato del canvas

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController non trovato nella scena!");
        }
    }

    private void Start()
    {
        // Assicurati che il canvas sia inizialmente nascosto
        if (mushCanvas != null)
        {
            mushCanvas.SetActive(false);
        }

        // Iscriviti all'evento OnCollected per mostrare il canvas quando il primo fungo è raccolto
        PlayerController.OnCollected += OnCollectibleCollected;
    }

    private void OnDestroy()
    {
        // Disiscriversi dall'evento per evitare errori
        PlayerController.OnCollected -= OnCollectibleCollected;
    }

    private void Update()
    {
        if (playerController != null && mushCanvas != null)
        {
            collectibleText.text = "x " + playerController.GetCollectibleCount().ToString();
        }
    }

    private void OnCollectibleCollected()
    {
        // Mostra il canvas se non è già visibile
        if (!canvasVisible && mushCanvas != null)
        {
            mushCanvas.SetActive(true);
            canvasVisible = true;
        }
    }
}
