using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SecondaryLightController : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private Light pointLight;
    public bool playerInRange = false; // Ora è pubblico e accessibile da altre classi

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        pointLight = GetComponent<Light>();
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
            playerInRange = true;
            Debug.Log("Dentro");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerInRange)
        {
            playerInRange = false;
            Debug.Log("Fuori");
        }
    }
}
