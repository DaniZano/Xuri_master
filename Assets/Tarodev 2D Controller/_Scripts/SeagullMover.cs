using UnityEngine;

public class SeagullMover : MonoBehaviour
{
    public Transform[] waypoints; // Array di waypoint
    public bool[] stopAtWaypoints; // Array che indica i waypoint in cui fermarsi
    public float speed = 2f; // Velocità di movimento
    public float stoppingDistance = 0.1f; // Distanza minima per considerare il waypoint raggiunto

    private int currentWaypointIndex = 0; // Indice del waypoint attuale
    private bool isMoving = false; // Controlla se l'entità si sta muovendo
    private bool waitingForPlayer = false; // Controlla se l'entità sta aspettando l'input del giocatore

    public bool IsMoving
    {
        get { return isMoving; }
    }

    void Update()
    {
        if (isMoving && waypoints.Length > 0 && !waitingForPlayer)
        {
            Move();
        }
    }

    void Move()
    {
        // Controlla che l'indice sia valido prima di accedere agli array
        if (currentWaypointIndex < waypoints.Length)
        {
            // Ottieni il waypoint corrente
            Transform targetWaypoint = waypoints[currentWaypointIndex];

            // Muovi l'entità verso il waypoint con una velocità costante
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

            // Se l'entità ha raggiunto il waypoint
            if (Vector3.Distance(transform.position, targetWaypoint.position) < stoppingDistance)
            {
                // Se è un waypoint di stop, aspetta l'input del giocatore
                if (stopAtWaypoints.Length > currentWaypointIndex && stopAtWaypoints[currentWaypointIndex])
                {
                    waitingForPlayer = true;
                    isMoving = false;
                }
                else
                {
                    // Passa al prossimo waypoint
                    currentWaypointIndex++;
                    if (currentWaypointIndex >= waypoints.Length)
                    {
                        isMoving = false;
                    }
                }
            }
        }
        else
        {
            isMoving = false;
        }
    }

    public void StartMovement()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            isMoving = true;
            waitingForPlayer = false;
        }
    }

    public void StopMovement()
    {
        isMoving = false;
    }

    public void ContinueMovement()
    {
        if (currentWaypointIndex < waypoints.Length - 1)
        {
            waitingForPlayer = false;
            isMoving = true;
            currentWaypointIndex++;
        }
    }

    public bool IsWaitingForPlayer()
    {
        return waitingForPlayer;
    }
}
