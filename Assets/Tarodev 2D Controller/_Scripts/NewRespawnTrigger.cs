using UnityEngine;
using TarodevController;


public class NewRespawnTrigger : MonoBehaviour
{
    public Transform newRespawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetRespawnPoint(newRespawnPoint);
            }

            gameObject.SetActive(false);

        }
    }
}
