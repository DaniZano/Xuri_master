using UnityEngine;
using System.Collections;

public class AudioFadeOut : MonoBehaviour
{
    public AudioSource audioSource; // Riferimento all'AudioSource
    public float fadeDuration = 1.0f; // Durata del fade out in secondi

    private void Awake()
    {
        // Se l'audioSource non è assegnato dall'Inspector, tenta di ottenerlo automaticamente
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();

            // Se non riesci a trovare un AudioSource, logga un errore
            if (audioSource == null)
            {
                Debug.LogError("AudioSource non trovato! Assicurati che sia assegnato.");
            }
        }
    }

    public void FadeOutAndStop()
    {
        if (audioSource != null)
        {
            StartCoroutine(FadeOutCoroutine());
        }
        else
        {
            Debug.LogWarning("AudioSource non assegnato o non trovato. Impossibile eseguire il fade out.");
        }
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Resetta il volume per futuri utilizzi
    }
}
