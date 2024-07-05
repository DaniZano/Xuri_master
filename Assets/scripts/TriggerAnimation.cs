using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    private Animation anim;
    public string loopAnimationName = "LOOP_IDOL_EVILHAND"; // Nome dell'animazione in loop
    public string triggerAnimationName = "KILLING_EVILHAND"; // Nome dell'animazione da riprodurre una volta
    public GameObject player; // Riferimento al GameObject del giocatore
    public Collider2D triggerCollider2D; // Riferimento al BoxCollider trigger

    void Start()
    {
        anim = GetComponent<Animation>();

        // Assicurati che il triggerCollider sia impostato come trigger
        if (triggerCollider2D != null && !triggerCollider2D.isTrigger)
        {
            Debug.LogWarning("Il BoxCollider deve essere impostato come trigger.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se l'oggetto che entra nel trigger è il giocatore
        if (other.gameObject == player)
        {
            // Stop all'animazione in loop e riproduci l'animazione una volta
            if (anim != null)
            {
                anim.Stop(loopAnimationName);
                anim.Play(triggerAnimationName);
            }
        }
    }
}

