using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SecondaryLightController : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private Light pointLight;
    public bool playerInRange = false; // Ora � pubblico e accessibile da altre classi

    public float netRange = 10f;  // Range netto e definito della luce
    public float netIntensity = 5f;  // Intensit� della luce netta e definita //

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        pointLight = GetComponent<Light>();

        // Imposta il collider come trigger
        circleCollider.isTrigger = true;

        // Imposta le propriet� della luce per essere nette e definite
        pointLight.range = netRange;
        pointLight.intensity = netIntensity;
        pointLight.shadows = LightShadows.Hard;  // Usa ombre dure per maggiore definizione

        SyncColliderWithLightRange();
    }

    void Update()
    {
        if (HasLightRangeChanged())
        {
            SyncColliderWithLightRange();
        }
    }

    void SyncColliderWithLightRange()
    {
        circleCollider.radius = pointLight.range;
    }

    bool HasLightRangeChanged()
    {
        return Mathf.Abs(circleCollider.radius - pointLight.range) > Mathf.Epsilon;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerInRange)
        {
            Debug.Log("Player entered trigger zone.");
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerInRange)
        {
            Debug.Log("Player exited trigger zone.");
            playerInRange = false;
        }
    }
}
