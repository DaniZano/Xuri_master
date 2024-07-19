using System.Collections.Generic;
using UnityEngine;

public class PlayerPath : MonoBehaviour
{
    public float pathUpdateInterval = 0.1f;
    public List<Vector3> pathPoints = new List<Vector3>();
    public bool isTracking = false;

    private float timeSinceLastUpdate = 0f;

    private void Update()
    {
        if (isTracking)
        {
            timeSinceLastUpdate += Time.deltaTime;

            if (timeSinceLastUpdate >= pathUpdateInterval)
            {
                pathPoints.Add(transform.position);
                timeSinceLastUpdate = 0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("StartTrackingZone"))
        {
            isTracking = true;
            Debug.Log("Tracking started!");
        }
    }

    public void ResetPath()
    {
        isTracking = false;
        pathPoints.Clear();
        Debug.Log("Path reset!");
    }
}
