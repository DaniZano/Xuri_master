using UnityEngine;

public class EntityVert : MonoBehaviour
{
    public float speed = 2f; // Velocità di movimento
    public float patrolDistance = 5f; // Distanza massima di movimento in una direzione
    private bool isMovingUp = true;
    private float startPositionY;

    private void Awake()
    {
        startPositionY = transform.position.y;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // Movimento su e giù sull'asse Y
        float movement = speed * Time.fixedDeltaTime;
        Vector3 newPosition;

        if (isMovingUp)
        {
            newPosition = new Vector3(transform.position.x, transform.position.y + movement, transform.position.z);
            if (newPosition.y >= startPositionY + patrolDistance)
            {
                newPosition.y = startPositionY + patrolDistance;
                isMovingUp = false;
            }
        }
        else
        {
            newPosition = new Vector3(transform.position.x, transform.position.y - movement, transform.position.z);
            if (newPosition.y <= startPositionY - patrolDistance)
            {
                newPosition.y = startPositionY - patrolDistance;
                isMovingUp = true;
            }
        }

        transform.position = newPosition;
    }
}

