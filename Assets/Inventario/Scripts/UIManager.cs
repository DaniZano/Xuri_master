using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject diaryUI; // Pannello del diario
    public Image[] slotImages; // Array di immagini per gli slot
    public Image pageContentImage; // Immagine per visualizzare il contenuto della pagina
    public GameObject singlePageView; // Overlay per visualizzare la singola pagina
    public GameObject blurBackground; // Sfondo sfocato
    public GameObject blurBackgroundSinglePage; // Sfondo sfocato
    public RectTransform selector; // Selettore degli slot

    public AudioClip moveSound; // Suono per il movimento
    public AudioClip openSound; // Suono per l'apertura dell'inventario
    public AudioClip selectSound; // Suono per la selezione di uno slot

    private AudioSource audioSource;

    private int currentPageIndex = -1; // Indice della pagina attualmente visualizzata
    private int currentSlotIndex = 0; // Indice dello slot attualmente selezionato

    // Variabili di stato per tracciare lo stato corrente degli input del controller
    private bool horizontalMoved = false;
    private bool verticalMoved = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "test_tutorial")
        {
            UIMainMenu mainMenuManager = FindObjectOfType<UIMainMenu>();
            if (mainMenuManager != null)
            {
                Destroy(mainMenuManager.gameObject);
            }
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        // Nascondi il diario all'inizio
        if (diaryUI != null)
        {
            diaryUI.SetActive(false);
        }

        // Inizializza gli slot come vuoti, trasparenti e non cliccabili
        for (int i = 0; i < slotImages.Length; i++)
        {
            slotImages[i].color = new Color(1, 1, 1, 0); // Trasparente
        }

        // Nascondi SinglePageView e BlurBackground all'inizio
        if (singlePageView != null)
        {
            singlePageView.SetActive(false);
        }

        if (blurBackground != null)
        {
            blurBackground.SetActive(false);
        }
        if (blurBackgroundSinglePage != null)
        {
            blurBackgroundSinglePage.SetActive(false);
        }

        // Nascondi il selettore all'inizio
        if (selector != null)
        {
            selector.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (MenuManagerINPLAY.Instance != null && MenuManagerINPLAY.Instance.IsMenuActive())
    {
        // Se il menu è aperto, non aprire l'inventario
        return;
    }
        // Controlla l'input della tastiera per aprire e chiudere l'inventario
        if (Input.GetKeyDown(KeyCode.I) || Input.GetButtonDown("Fire3"))
        {
            ToggleDiaryUI();
        }

        // Controlla l'input per selezionare gli oggetti solo quando l'inventario è aperto
        if (diaryUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Jump"))
            {
                if (singlePageView.activeSelf)
                {
                    HideDiaryPage();
                }
                else
                {
                    SelectSlot(currentSlotIndex);
                }
            }

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            float dpadHorizontalInput = Input.GetAxis("DPadHorizontal");
            float dpadVerticalInput = Input.GetAxis("DPadVertical");

            // Input della tastiera
            if (Input.GetKeyDown(KeyCode.LeftArrow) || (!horizontalMoved && horizontalInput < -0.5f) || (!horizontalMoved && dpadHorizontalInput < -0.5f))
            {
                MoveSelector(-1);
                horizontalMoved = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || (!horizontalMoved && horizontalInput > 0.5f) || (!horizontalMoved && dpadHorizontalInput > 0.5f))
            {
                MoveSelector(1);
                horizontalMoved = true;
            }
            else if (horizontalInput > -0.5f && horizontalInput < 0.5f && dpadHorizontalInput > -0.5f && dpadHorizontalInput < 0.5f)
            {
                horizontalMoved = false;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || (!verticalMoved && verticalInput > 0.5f) || (!verticalMoved && dpadVerticalInput > 0.5f))
            {
                MoveSelector(-4); // Supponendo che ci siano 4 slot per riga
                verticalMoved = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || (!verticalMoved && verticalInput < -0.5f) || (!verticalMoved && dpadVerticalInput < -0.5f))
            {
                MoveSelector(4); // Supponendo che ci siano 4 slot per riga
                verticalMoved = true;
            }
            else if (verticalInput > -0.5f && verticalInput < 0.5f && dpadVerticalInput > -0.5f && dpadVerticalInput < 0.5f)
            {
                verticalMoved = false;
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                SelectSlot(currentSlotIndex);
            }
        }
    }

    public void UpdateDiarySlots()
    {
        var pages = Inventory.Instance.GetPages();
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < pages.Count)
            {
                slotImages[i].sprite = pages[i].icon;
                slotImages[i].color = Color.white; // Rende visibile l'icona
            }
            else
            {
                slotImages[i].sprite = null;
                slotImages[i].color = new Color(1, 1, 1, 0); // Rende trasparente lo slot
            }
        }

        // Mostra il selettore e impostalo sul primo slot non vuoto
        if (pages.Count > 0)
        {
            currentSlotIndex = FindNextNonEmptySlot(0, 1);
            UpdateSelectorPosition();
            selector.gameObject.SetActive(true);
        }
        else
        {
            selector.gameObject.SetActive(false);
        }
    }

    public void ShowDiaryPage(int index)
    {
        if (index == currentPageIndex)
            return; // Evita di ricaricare la stessa pagina

        currentPageIndex = index;
        var page = Inventory.Instance.GetPage(index);
        if (page != null)
        {
            // Imposta l'immagine della pagina su SinglePageView
            singlePageView.GetComponent<Image>().sprite = page.contentImage;
            singlePageView.GetComponent<Image>().preserveAspect = true; // Mantiene le proporzioni
            singlePageView.transform.SetAsLastSibling(); // Porta l'immagine in primo piano
            singlePageView.SetActive(true);
            blurBackgroundSinglePage.SetActive(true);

            // Mostra il BlurBackground
            blurBackground.SetActive(true);

            PlaySound(selectSound);
        }
    }

    public void HideDiaryPage()
    {
        singlePageView.SetActive(false);
        blurBackgroundSinglePage.SetActive(false);
        currentPageIndex = -1; // Resetta l'indice della pagina corrente

        // Non nascondere il BlurBackground perché il diario è ancora attivo
    }

    public void ToggleDiaryUI()
    {
        if (diaryUI != null)
        {
            bool isActive = diaryUI.activeSelf;
            diaryUI.SetActive(!isActive);

            if (!diaryUI.activeSelf)
            {
                // Nascondi SinglePageView e BlurBackground quando chiudi l'inventario
                HideDiaryPage();
                blurBackground.SetActive(false);
                Time.timeScale = 1; // Riprendi il gioco
                selector.gameObject.SetActive(false); // Nascondi il selettore
            }
            else
            {
                // Assicurati che il BlurBackground sia visibile quando il diario è aperto
                blurBackground.SetActive(true);
                Time.timeScale = 0; // Metti in pausa il gioco

                // Mostra il selettore e impostalo sul primo slot non vuoto
                if (Inventory.Instance.GetPages().Count > 0)
                {
                    currentSlotIndex = FindNextNonEmptySlot(0, 1);
                    UpdateSelectorPosition();
                    selector.gameObject.SetActive(true);
                }
                PlaySound(openSound);
            }
        }
    }

    private void MoveSelector(int direction)
    {
        int newSlotIndex = FindNextNonEmptySlot(currentSlotIndex, direction);
        if (newSlotIndex != currentSlotIndex)
        {
            currentSlotIndex = newSlotIndex;
            UpdateSelectorPosition();

            PlaySound(moveSound);
        }
    }

    private void UpdateSelectorPosition()
    {
        selector.position = slotImages[currentSlotIndex].transform.position;
    }

    private void SelectSlot(int slotIndex)
    {
        ShowDiaryPage(slotIndex);
    }

    private int FindNextNonEmptySlot(int startIndex, int direction)
    {
        int newIndex = startIndex;
        do
        {
            newIndex = (newIndex + direction + slotImages.Length) % slotImages.Length;
        } while (slotImages[newIndex].sprite == null && newIndex != startIndex);
        return newIndex;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
