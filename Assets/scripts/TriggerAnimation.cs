using System.Collections;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    private Animation anim;
    public string loopAnimationName = "LOOP_IDOL_EVILHAND"; // Nome dell'animazione in loop
    public string triggerAnimationName = "KILLING_EVILHAND"; // Nome dell'animazione da riprodurre una volta
    public GameObject player; // Riferimento al GameObject del giocatore
    public Collider2D triggerCollider2D; // Riferimento al BoxCollider trigger
    public float animationSpeed = 1.0f; // Velocità dell'animazione
    public float restoreScaleDelay = 0.5f; // Ritardo prima di ripristinare la scala originale

    private Vector3 originalScale;
    private bool isAnimating = false;

    void Start()
    {
        anim = GetComponent<Animation>();
        originalScale = transform.localScale;

        // Assicurati che il triggerCollider sia impostato come trigger
        if (triggerCollider2D != null && !triggerCollider2D.isTrigger)
        {
            Debug.LogWarning("Il BoxCollider deve essere impostato come trigger.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se l'oggetto che entra nel trigger è il giocatore
        if (other.gameObject == player && !isAnimating)
        {
            StartCoroutine(PlayKillAnimation());
        }
    }

    private IEnumerator PlayKillAnimation()
    {
        isAnimating = true;

        if (anim != null)
        {
            // Ferma l'animazione in loop e riproduci l'animazione una volta
            anim.Stop(loopAnimationName);

            // Imposta la velocità dell'animazione
            AnimationState state = anim[triggerAnimationName];
            state.speed = animationSpeed;

            // Riproduci l'animazione
            anim.Play(triggerAnimationName);

            // Attendi la fine dell'animazione di "kill"
            yield return new WaitForSeconds(state.length / animationSpeed);

            // Ripristina la scala originale
            transform.localScale = originalScale;

            // Riprendi l'animazione in loop se necessario
            anim.Play(loopAnimationName);
        }

        isAnimating = false;
    }
}
