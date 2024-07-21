using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Assign in inspector
    public string nextSceneName; // Assign in inspector

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
