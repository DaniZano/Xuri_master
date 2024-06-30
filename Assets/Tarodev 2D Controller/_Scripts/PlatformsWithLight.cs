using UnityEngine;
using TarodevController;
using System.Collections;

public class PlatformsWithLight : MonoBehaviour
{
    public GameObject[] platforms; // Array of platforms to activate
    public KeyCode keyToPress = KeyCode.LeftControl; // Key to press to activate the platforms
    public Collider2D triggerCollider; // Collider trigger to activate the platforms
    public float deactivateDelay = 5f; // Time in seconds to wait before deactivating platforms

    private bool canActivate = false;
    private PlayerController playerController;

    void Start()
    {
        // Deactivate all platforms at start
        DeactivatePlatforms();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canActivate = true;
            playerController = other.GetComponent<PlayerController>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canActivate = false;
            playerController = null;
        }
    }

    void Update()
    {
        if (canActivate && Input.GetKeyDown(keyToPress))
        {
            if (playerController != null && playerController.HasCollectibles())
            {
                ActivatePlatforms();
                StartCoroutine(DeactivatePlatformsAfterDelay(deactivateDelay));
            }
            else
            {
                Debug.Log("Ctrl key pressed but no collectibles available.");
            }
        }
    }

    void ActivatePlatforms()
    {
        foreach (GameObject platform in platforms)
        {
            platform.SetActive(true);
        }
    }

    void DeactivatePlatforms()
    {
        foreach (GameObject platform in platforms)
        {
            platform.SetActive(false);
        }
    }

    IEnumerator DeactivatePlatformsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DeactivatePlatforms();
    }
}
