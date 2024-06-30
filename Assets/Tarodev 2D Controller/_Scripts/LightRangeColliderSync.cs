using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class LightRangeColliderSync : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    private Light pointLight;

    void Start()
    {
        // Ottieni il CircleCollider2D attaccato a questo GameObject
        circleCollider = GetComponent<CircleCollider2D>();

        // Ottieni il componente Light (Point Light) attaccato a questo GameObject
        pointLight = GetComponent<Light>();

        // Sincronizza la dimensione del collider con il range iniziale della luce
        SyncColliderWithLightRange();
    }

    void Update()
    {
        // Verifica se il range della luce è cambiato e sincronizza il collider di conseguenza
        if (HasLightRangeChanged())
        {
            SyncColliderWithLightRange();
        }
    }

    void SyncColliderWithLightRange()
    {
        // Imposta il raggio del collider uguale al range della Point Light
        circleCollider.radius = pointLight.range;
    }

    bool HasLightRangeChanged()
    {
        // Verifica se il range della Point Light è cambiato rispetto all'ultimo frame
        return Mathf.Abs(circleCollider.radius - pointLight.range) > Mathf.Epsilon;
    }
}
