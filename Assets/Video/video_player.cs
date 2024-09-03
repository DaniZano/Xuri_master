using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class VideoSceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Assegna nel Inspector
    public string nextSceneName; // Assegna nel Inspector
    public Image fadeImage; // Immagine usata per il fade, assegna nel Inspector
    public float fadeDuration = 1.0f; // Durata del fade

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        // Inizialmente rendi l'immagine del fade completamente trasparente
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true); 
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Inizia il fade out
        if (fadeImage != null)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
        else
        {
            // Se non c'è immagine di fade, carica semplicemente la scena successiva
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        float elapsedTime = 0.0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Assicurati che l'opacità sia completamente nera alla fine
        color.a = 1;
        fadeImage.color = color;

        // Carica la scena successiva
        SceneManager.LoadScene(nextSceneName);
    }
}
