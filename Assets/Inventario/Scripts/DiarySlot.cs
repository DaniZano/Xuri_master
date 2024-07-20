using UnityEngine;
using UnityEngine.UI;

public class DiarySlot : MonoBehaviour
{
    public int slotIndex; // L'indice dello slot

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => OnClick());
    }

    void OnClick()
    {
        UIManager.Instance.ShowDiaryPage(slotIndex);
    }
}
