using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private List<DiaryPage> diaryPages = new List<DiaryPage>();
    public int maxPages = 15;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPage(DiaryPage page)
    {
        if (diaryPages.Count < maxPages)
        {
            diaryPages.Add(page);
            // Aggiorna UI
            UIManager.Instance.UpdateDiarySlots();
        }
    }

    public DiaryPage GetPage(int index)
    {
        if (index < diaryPages.Count)
        {
            return diaryPages[index];
        }
        return null;
    }

    public List<DiaryPage> GetPages()
    {
        return diaryPages;
    }
}
