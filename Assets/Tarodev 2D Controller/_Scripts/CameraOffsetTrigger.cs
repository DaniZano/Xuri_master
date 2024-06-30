using UnityEngine;
using System.Collections; // Aggiungi questa linea

public class CameraOffsetTrigger : MonoBehaviour
{
    public Vector3 offsetChange = new Vector3(2f, 0f, 0f); // L'offset da aggiungere temporaneamente alla camera
    public float transitionTime = 1.0f; // Tempo per la transizione dell'offset
   
    private bool isTriggered = false; // Flag per tracciare se il trigger è attivo
    private CameraFollow cameraFollow;
    private Vector3 originalOffset;

    private void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        originalOffset = cameraFollow.offset; // Salva l'offset originale da CameraFollow
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isTriggered)
            {
                // Applica l'offset temporaneo
                StartCoroutine(ChangeCameraOffset(originalOffset + offsetChange));
                isTriggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isTriggered)
            {
                // Ripristina l'offset originale
                StartCoroutine(ChangeCameraOffset(originalOffset));
                isTriggered = false;
            }
        }
    }

    IEnumerator ChangeCameraOffset(Vector3 targetOffset)
    {
        float elapsedTime = 0f;
        Vector3 startingOffset = cameraFollow.offset;

        while (elapsedTime < transitionTime)
        {
            cameraFollow.offset = Vector3.Lerp(startingOffset, targetOffset, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraFollow.offset = targetOffset;
    }
}
