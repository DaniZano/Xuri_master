using UnityEngine;

public class EggCollection : MonoBehaviour
{
    public GameObject attachedObject; // Assegna qui l'oggetto attaccato gi� presente ma disattivato

    void Start()
    {
        // Assicurati che l'oggetto attaccato sia disattivato all'inizio
        if (attachedObject != null && attachedObject.activeSelf)
        {
            attachedObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AttachToPlayer();
        }
    }

    private void AttachToPlayer()
    {
        // Attiva l'oggetto attaccato se non � gi� attivo
        if (attachedObject != null && !attachedObject.activeSelf)
        {
            attachedObject.SetActive(true);
        }

        // Distruggi l'oggetto raccolto
        Destroy(gameObject);
    }
}
