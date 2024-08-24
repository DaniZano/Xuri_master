using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoSkip : MonoBehaviour
{
    

    private void Update()
    {
        // Controlla se viene premuto il tasto Invio sulla tastiera o il tasto "X" sul controller
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Jump"))
        {
            SceneManager.LoadScene("test_tutorial"); // Carica una nuova scena
        }
    }

   

}
