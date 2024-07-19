using UnityEngine;
using TarodevController;

public class Entity : MonoBehaviour
{
    public float speed = 2f; // Velocità di movimento
    public float patrolDistance = 5f; // Distanza massima di movimento in una direzione
    private bool isMovingRight = true;
    private float startPositionX;

  

    private void Awake()
    {
        startPositionX = transform.position.x;
    }

    private void Update()
    {
        
        Move();
        
    }

    private void Move()
    {
        // Movimento avanti e indietro sull'asse X
        float movement = speed * Time.deltaTime;
        if (isMovingRight)
        {
            transform.Translate(movement, 0, 0);
            if (transform.position.x >= startPositionX + patrolDistance)
            {
                isMovingRight = false;
            }
        }
        else
        {
            transform.Translate(-movement, 0, 0);
            if (transform.position.x <= startPositionX - patrolDistance)
            {
                isMovingRight = true;
            }
        }
    }

}
