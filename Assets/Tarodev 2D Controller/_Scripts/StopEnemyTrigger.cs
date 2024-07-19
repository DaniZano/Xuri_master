using UnityEngine;

public class StopEnemyTrigger : MonoBehaviour
{
    public EnemyFollowPath enemyFollowPath; // Riferimento allo script del nemico

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyFollowPath != null)
            {
                enemyFollowPath.StopFollowing();
                Debug.Log("Enemy stopped following because player entered the trigger.");
            }
            else
            {
                Debug.LogError("EnemyFollowPath non assegnato in StopEnemyTrigger!");
            }
        }
    }
}
