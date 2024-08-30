using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoSkip : MonoBehaviour
{
    private void Start()
    {
        // Check if the scene is video_intro and destroy MainMenuManager if it exists
        if (SceneManager.GetActiveScene().name == "video_intro")
        {
            UIMainMenu mainMenuManager = FindObjectOfType<UIMainMenu>();
            if (mainMenuManager != null)
            {
                Destroy(mainMenuManager.gameObject);
            }
        }
    }

    private void Update()
    {
        // Controlla se viene premuto il tasto Invio sulla tastiera o il tasto "X" sul controller
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Jump"))
        {
            SceneManager.LoadScene("test_tutorial"); // Carica una nuova scena
        }
    }

   

}
