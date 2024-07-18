using UnityEngine;
using System.Collections.Generic;

public class EnemyFollowPath : MonoBehaviour
{
    public PlayerPath playerPath;
    public float speed = 2.0f;
    public float waypointThreshold = 0.1f;
    public bool isFollowing = false;

    private Queue<Vector3> pathQueue = new Queue<Vector3>();

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
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("StartFollowingZone"))
        {
            Debug.Log("Segui segui");
            isFollowing = true;
        }
    }
}
