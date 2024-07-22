using UnityEngine;
using TMPro;
using TarodevController;

public class UIMushCollector : MonoBehaviour
{
    public TextMeshProUGUI collectibleText;
    private PlayerController playerController;

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
        if (playerController != null)
        {
            collectibleText.text = "x " + playerController.GetCollectibleCount().ToString();
        }
    }
}
