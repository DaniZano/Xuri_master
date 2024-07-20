using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SecondaryLightController : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private Light pointLight;
    public bool playerInRange = false; // Ora è pubblico e accessibile da altre classi

    public float netRange = 10f;  // Range netto e definito della luce
    public float netIntensity = 5f;  // Intensità della luce netta e definita //

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        pointLight = GetComponent<Light>();

        // Imposta il collider come trigger
        circleCollider.isTrigger = true;

        // Imposta le proprietà della luce per essere nette e definite
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
        // Ottieni la scala dell'oggetto genitore
        float parentScale = transform.parent != null ? transform.parent.localScale.x : 1f; // Assumendo scala uniforme
        float inverseScale = 1f / parentScale;
        circleCollider.radius = pointLight.range * inverseScale;
    }

    bool HasLightRangeChanged()
    {
        float parentScale = transform.parent != null ? transform.parent.localScale.x : 1f; // Assumendo scala uniforme
        float inverseScale = 1f / parentScale;
        return Mathf.Abs(circleCollider.radius - (pointLight.range * inverseScale)) > Mathf.Epsilon;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerInRange)
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerInRange)
        {
            playerInRange = false;
        }
    }
}
