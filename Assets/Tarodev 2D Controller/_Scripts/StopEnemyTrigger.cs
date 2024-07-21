using UnityEngine;

public class StopEnemyTrigger : MonoBehaviour
{
    public EnemyFollowPath enemyFollowPath; // Riferimento allo script del nemico
    private bool isFollowing = true; // Stato corrente dell'inseguimento

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (enemyFollowPath != null)
            {
                if (isFollowing)
                {
                    enemyFollowPath.StopFollowing();
                    isFollowing = false;
                    Debug.Log("Enemy stopped following because player entered the trigger.");
                }
                else
                {
                    enemyFollowPath.StartFollowing();
                    isFollowing = true;
                    Debug.Log("Enemy started following because player entered the trigger.");
                }
            }
            else
            {
                Debug.LogError("EnemyFollowPath non assegnato in StopEnemyTrigger!");
            }
        }
    }
}
