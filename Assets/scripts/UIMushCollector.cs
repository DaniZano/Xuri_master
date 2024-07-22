using UnityEngine;
using TMPro;
using TarodevController;

public class UIMushCollector : MonoBehaviour
{
    public TextMeshProUGUI collectibleText;
    private PlayerController playerController;
    public GameObject mushCanvas;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController non trovato nella scena!");
        }
    }

    private void Update()
    {
        if (playerController != null &&mushCanvas!=null)
        {
            if (playerController.GetCollectibleCount()==0) {
                mushCanvas.SetActive(false);
            }
            else {
                mushCanvas.SetActive(false);
                collectibleText.text = "x " + playerController.GetCollectibleCount().ToString();
                }
        }
    }
}

