using UnityEngine;
using System.Collections.Generic;

public class EnemyFollowPath : MonoBehaviour
{
    public PlayerPath playerPath; // Riferimento allo script del personaggio
    public float speed = 2.0f; // Velocità di movimento del nemico
    public float waypointThreshold = 0.1f; // Distanza per considerare raggiunto un punto
    public Vector3 initialPosition; // Posizione iniziale del nemico
    private bool isFollowing = false; // Determina se il nemico sta seguendo il percorso

    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private bool isFacingRight = true; // Stato corrente della direzione del nemico

    private void Start()
    {
        initialPosition = transform.position; // Salva la posizione iniziale del nemico
    }

    private void Update()
    {
        if (isFollowing && playerPath != null)
        {
            while (playerPath.pathPoints.Count > 0)
            {
                pathQueue.Enqueue(playerPath.pathPoints[0]);
                playerPath.pathPoints.RemoveAt(0);
            }

            if (pathQueue.Count > 0)
            {
                Vector3 currentWaypoint = pathQueue.Peek();
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, step);

                if (Vector3.Distance(transform.position, currentWaypoint) < waypointThreshold)
                {
                    pathQueue.Dequeue();
                }

                // Logica di flipping
                if (currentWaypoint.x > transform.position.x && !isFacingRight)
                {
                    Flip();
                }
                else if (currentWaypoint.x < transform.position.x && isFacingRight)
                {
                    Flip();
                }
            }
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1; // Inverte la scala sull'asse X per flippare
        transform.localScale = theScale;
        Debug.Log("Enemy flipped. Now facing " + (isFacingRight ? "right" : "left"));
    }

    public void StartFollowing()
    {
        isFollowing = true;
        Debug.Log("Following started!");
    }

    public void StopFollowing()
    {
        isFollowing = false;
        pathQueue.Clear();
        Debug.Log("Following stopped!");
    }

    public void ResetEnemy()
    {
        isFollowing = false;
        pathQueue.Clear();
        transform.position = initialPosition;
        Debug.Log("Enemy reset to initial position: " + initialPosition);
    }
}
