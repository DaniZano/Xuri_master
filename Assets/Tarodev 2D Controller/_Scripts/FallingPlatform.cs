using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay = 1f; // Ritardo prima del crollo
    [SerializeField] private float destroyDelay = 2f; // Ritardo prima della distruzione della piattaforma
    [SerializeField] private Rigidbody2D _rigidbody;

    private void Start()
    {
        // Assicurati che il Rigidbody2D sia impostato come Kinematic all'inizio
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);

        // Cambia il layer della piattaforma per evitare collisioni con altre piattaforme
        gameObject.layer = LayerMask.NameToLayer("FallingPlatform");

        // Imposta il Rigidbody2D come Dynamic per far cadere la piattaforma
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;

        // Distruggi la piattaforma dopo un ritardo
        Destroy(gameObject, destroyDelay);
    }
}
