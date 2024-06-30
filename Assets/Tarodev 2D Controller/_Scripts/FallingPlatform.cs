using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 1f; // Ritardo prima del crollo
    [SerializeField] private float destroyDelay = 2f; // Ritardo prima della distruzione della piattaforma
    [SerializeField] private Rigidbody2D _rigidbody;

    private Vector3 initialPosition;
    private bool isFalling = false;

    private void Start()
    {
        // Assicurati che il Rigidbody2D sia impostato come Kinematic all'inizio
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        // Memorizza la posizione iniziale
        initialPosition = transform.position;

        // Registra questa piattaforma nel gestore delle piattaforme
        FallingPlatformManager.RegisterPlatform(this);
    }

    private void OnDestroy()
    {
        // Annulla la registrazione dal gestore delle piattaforme quando questa piattaforma viene distrutta
        FallingPlatformManager.UnregisterPlatform(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            // Calcola la normale della collisione
            Vector2 contactNormal = collision.GetContact(0).normal;

            // Controlla se l'impatto è avvenuto dall'alto (verso l'alto)
            if (contactNormal.y < -0.5f) // Modifica questo valore in base alle tue esigenze di inclinazione
            {
                StartCoroutine(Fall());
            }
        }
    }


    private IEnumerator Fall()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallDelay);

        // Cambia il layer della piattaforma per evitare collisioni con altre piattaforme
        gameObject.layer = LayerMask.NameToLayer("FallingPlatform");

        // Imposta il Rigidbody2D come Dynamic per far cadere la piattaforma
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;

        // Disattiva la piattaforma dopo un ritardo
        yield return new WaitForSeconds(destroyDelay);
        gameObject.SetActive(false); // Disabilita la piattaforma invece di distruggerla
    }

    public void ResetPlatform()
    {
        // Riporta la piattaforma alla posizione iniziale
        transform.position = initialPosition;
        // Reimposta il Rigidbody2D come Kinematic
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        // Cambia il layer della piattaforma al layer di default
        gameObject.layer = LayerMask.NameToLayer("Default");
        isFalling = false;
        gameObject.SetActive(true); // Riattiva la piattaforma
    }
}
