using UnityEngine;

public class Player : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DiaryPage"))
        {
            DiaryPage page = other.GetComponent<DiaryPagePickup>().page;
            Inventory.Instance.AddPage(page);
            Destroy(other.gameObject);
        }
    }
}
