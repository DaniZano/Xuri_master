using UnityEngine;

public class EggCollection : MonoBehaviour
{
    public GameObject attachedObject; // Assegna qui l'oggetto attaccato già presente ma disattivato
    public AudioClip collectEgg;
    private AudioSource audioSource; // Campo per AudioSource

    void Start()
    {
        // Assicurati che l'oggetto attaccato sia disattivato all'inizio
        if (attachedObject != null && attachedObject.activeSelf)
        {
            attachedObject.SetActive(false);
        }

        // Trova e assegna l'AudioSource se non è già stato assegnato
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource non trovato sul GameObject. Assicurati di aggiungerlo.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlaySound(collectEgg);
            AttachToPlayer();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioSource o AudioClip non assegnati.");
        }
    }

    private void AttachToPlayer()
    {
        // Attiva l'oggetto attaccato se non è già attivo
        if (attachedObject != null && !attachedObject.activeSelf)
        {
            attachedObject.SetActive(true);
        }

        // Distruggi l'oggetto raccolto
        Destroy(gameObject);
    }
}
