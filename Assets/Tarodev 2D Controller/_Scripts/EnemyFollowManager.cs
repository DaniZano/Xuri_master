using UnityEngine;

public class EnemyFollowManager : MonoBehaviour
{
    public EnemyFollowPath enemyFollowPath;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyFollowPath.StartFollowing();
        }
    }
}
