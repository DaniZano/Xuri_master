using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIMainMenu : MonoBehaviour
{
    public static UIMainMenu Instance;

    public GameObject MainMenuUI; // Pannello del menu
    public Image[] MainSlotImages; // Array di immagini per gli slot
    public RectTransform MainSelector; // Selettore degli slot

    public AudioClip moveSound; // Suono per il movimento
    public AudioClip selectSound; // Suono per la selezione di uno slot

    public GameObject settingsOverlay; // Overlay delle impostazioni
    public Slider volumeSlider; // Slider per il volume

    public GameObject creditsCanvas; // Canvas dei crediti

    private AudioSource audioSource;
    private int currentSlotIndex = 0; // Indice dello slot attualmente selezionato
    private bool horizontalMoved = false;
    private bool verticalMoved = false;
    private bool isOverlayActive = false; // Flag per controllare se un overlay è attivo
    private bool isMenuActive = true; // Flag per controllare se il menu è attivo

    public float fadeDuration = 0.25f; // Durata del fade
    private CanvasGroup mainMenuCanvasGroup;

    public AudioSource sceneAudioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantieni l'istanza tra le scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // Assicurati che il menu sia visibile all'inizio
        if (MainMenuUI != null)
        {
            mainMenuCanvasGroup = MainMenuUI.GetComponent<CanvasGroup>();
            if (mainMenuCanvasGroup == null)
            {
                mainMenuCanvasGroup = MainMenuUI.AddComponent<CanvasGroup>();
            }

            // Nascondi inizialmente
            mainMenuCanvasGroup.alpha = 0;
            MainMenuUI.SetActive(true);

            // Avvia il fade-in
            StartCoroutine(Fade(0, 1, fadeDuration));
        }

        // Inizializza gli slot come trasparenti
        foreach (var slot in MainSlotImages)
        {
            if (slot != null)
            {
                slot.color = new Color(1, 1, 1, 0); // Trasparente
            }
        }

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            MenuManagerINPLAY menuINPLAY = FindObjectOfType<MenuManagerINPLAY>();
            if (menuINPLAY != null)
            {
                Destroy(menuINPLAY.gameObject);
            }
        }

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            UIManager inventario = FindObjectOfType<UIManager>();
            if (inventario != null)
            {
                Destroy(inventario.gameObject);
            }
        }


        // Mostra il selettore all'inizio e imposta il colore con la trasparenza desiderata
        if (MainSelector != null)
        {
            Image MainSelectorImage = MainSelector.GetComponent<Image>();
            if (MainSelectorImage != null)
            {
                MainSelectorImage.color = new Color(1, 1, 1, 0.2f); // Bianco con 20% di trasparenza
            }
            MainSelector.gameObject.SetActive(true);
        }

        // Nascondi le impostazioni all'inizio
        if (settingsOverlay != null)
        {
            settingsOverlay.SetActive(false);
        }

        // Nascondi il canvas dei crediti all'inizio
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(false);
        }

        // Imposta il valore iniziale del volume
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            mainMenuCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        mainMenuCanvasGroup.alpha = endAlpha;
    }

    void Update()
    {
        // Gestisce l'input del controller per chiudere gli overlay e tornare al menu
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton9)) 
        {
            CloseOverlay();
        }

        // Se un overlay è attivo, ignora gli altri input
        if (isOverlayActive)
        {
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float dpadHorizontalInput = Input.GetAxis("DPadHorizontal");
        float dpadVerticalInput = Input.GetAxis("DPadVertical");

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
            MoveSelector(-1); 
            verticalMoved = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || (!verticalMoved && verticalInput < -0.5f) || (!verticalMoved && dpadVerticalInput < -0.5f))
        {
            MoveSelector(1); 
            verticalMoved = true;
        }
        else if (verticalInput > -0.5f && verticalInput < 0.5f && dpadVerticalInput > -0.5f && dpadVerticalInput < 0.5f)
        {
            verticalMoved = false;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Jump"))
        {
            SelectSlot(currentSlotIndex);
        }
    }

    private void MoveSelector(int direction)
    {
        currentSlotIndex += direction;

        if (currentSlotIndex < 0)
        {
            currentSlotIndex = MainSlotImages.Length - 1;
        }
        else if (currentSlotIndex >= MainSlotImages.Length)
        {
            currentSlotIndex = 0;
        }

        // Muovi il selettore alla posizione del nuovo slot
        if (MainSelector != null && MainSlotImages[currentSlotIndex] != null)
        {
            MainSelector.position = MainSlotImages[currentSlotIndex].transform.position;
        }

        PlaySound(moveSound);
    }

    private void SelectSlot(int index)
    {
        switch (index)
        {
            case 0:
                OnSlot1Clicked();
                break;
            case 1:
                OnSlot3Clicked();
                break;
                
            case 2:
                OnSlot2Clicked();
                break;
                
            case 3:
                OnSlot4Clicked();
                break;
            default:
                break;
        }

        PlaySound(selectSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void OnSlot1Clicked()
    {

         if (sceneAudioSource != null)
            {
                StartCoroutine(FadeOutAudio(sceneAudioSource, 0.5f)); // 1.0f è la durata del fade out
            }

        // Chiama il metodo per caricare la scena dopo il fade out
        StartCoroutine(LoadSceneAfterFade());
        // Carica la nuova scena
        
        
        // Disattiva il menu principale
        if (MainMenuUI != null)
        {
            MainMenuUI.SetActive(false);
        }

       
    }

    private IEnumerator LoadSceneAfterFade()
    {
        // Attendi fino al completamento del fade out
        yield return new WaitForSeconds(1.0f); // La stessa durata del fade out

        SceneManager.LoadScene("video_intro");
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float fadeDuration)
{
    float startVolume = audioSource.volume;

    while (audioSource.volume > 0)
    {
        audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
        yield return null;
    }

    audioSource.Stop();
    audioSource.volume = startVolume; // Resetta il volume per futuri utilizzi
}


    public void OnSlot2Clicked()
    {
        ShowSettings();
    }

    public void OnSlot3Clicked()
    {
        Application.Quit();
    }

    public void OnSlot4Clicked()
    {
        ShowCredits();
    }

    private void ShowSettings()
    {
        if (settingsOverlay != null)
        {
            settingsOverlay.SetActive(true);
            isOverlayActive = true; 
        }
    }

    private void ShowCredits()
    {
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(true);
            isOverlayActive = true; 
        }
    }

    private void CloseOverlay()
    {
        if (settingsOverlay != null)
        {
            settingsOverlay.SetActive(false); 
        }

        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(false); 
        }

        if (MainMenuUI != null)
        {
            MainMenuUI.SetActive(true);
        }

        isOverlayActive = false; 
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }
}
