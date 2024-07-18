using UnityEngine;
using TMPro; // Assicurati di avere questo using se stai usando TextMeshPro

public class DialogueTrigger : MonoBehaviour
{
    public GameObject dialoguePanel; // Assegna il pannello di dialogo qui
    public TMP_Text dialogueText; // Assegna il componente TMP_Text qui
    public string dialogueMessage = "Benvenuto nel nostro mondo!"; // Testo del dialogo
    public KeyCode hideDialogueKey = KeyCode.Escape; // Tasto per non mostrare più il dialogo

    private bool isPlayerInRange = false;

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // Nasconde il dialogo all'inizio
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(hideDialogueKey))
        {
            dialoguePanel.SetActive(false); // Nasconde il dialogo se il tasto è premuto
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player")) // Assicurati che il tuo oggetto player abbia il tag "Player"
        {
            isPlayerInRange = true;
            if (dialogueText != null)
            {
                dialogueText.text = dialogueMessage; // Imposta il testo del dialogo qui
            }
            dialoguePanel.SetActive(true); // Mostra il dialogo quando il player entra
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialoguePanel.SetActive(false); // Nasconde il dialogo quando il player esce
        }
    }
}
