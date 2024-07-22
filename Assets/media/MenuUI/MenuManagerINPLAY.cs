using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManagerINPLAY : MonoBehaviour
{
    public static MenuManagerINPLAY Instance;

    public GameObject MenuUI; // Pannello del menu
    public Image[] slotImages; // Array di immagini per gli slot
    public RectTransform selector; // Selettore degli slot

    public AudioClip moveSound; // Suono per il movimento
    public AudioClip selectSound; // Suono per la selezione di uno slot

    public GameObject settingsOverlay; // Overlay delle impostazioni
    public Slider volumeSlider; // Slider per il volume


    private AudioSource audioSource;
    private int currentSlotIndex = 0; // Indice dello slot attualmente selezionato
    private bool horizontalMoved = false;
    private bool verticalMoved = false;
    private bool isOverlayActive = false; // Flag per controllare se un overlay è attivo
    private bool isMenuActive = false; // Flag per controllare se il menu è attivo


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

        // Assicurati che il menu sia invisibile all'inizio
        if (MenuUI != null)
        {
            MenuUI.SetActive(false);
        }

        // Inizializza gli slot come trasparenti
        foreach (var slot in slotImages)
        {
            if (slot != null)
            {
                slot.color = new Color(1, 1, 1, 0); // Trasparente
            }
        }

        // Mostra il selettore all'inizio e imposta il colore con la trasparenza desiderata
        if (selector != null)
        {
            Image selectorImage = selector.GetComponent<Image>();
            if (selectorImage != null)
            {
                selectorImage.color = new Color(1, 1, 1, 0.2f); // Bianco con 20% di trasparenza
            }
            selector.gameObject.SetActive(true);
        }

        // Nascondi le impostazioni all'inizio
        if (settingsOverlay != null)
        {
            settingsOverlay.SetActive(false);
        }

       

        // Imposta il valore iniziale del volume
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    void Update()
    {

        // Controlla se il menu deve essere mostrato
        if (Input.GetKeyDown(KeyCode.M)  || Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            ToggleMenu();
        }

        // Gestisce l'input del controller per chiudere gli overlay e tornare al menu
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton9))
        {
            CloseOverlay();
        }

        // Se un overlay è attivo, ignora gli altri input
        if (isOverlayActive || !isMenuActive)
        {
            return;
        }

        //if (isOverlayActive)
        //{
        //    // Controlla se il pulsante Joystick 9 è premuto
        //    if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton9))
        //    {
        //        // Gestisci l'input del pulsante Joystick 9 qui
        //        CloseOverlay();
        //    }
        //    // Esce dalla funzione per ignorare altri input
        //    return;
        //}

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
            MoveSelector(-1); // Puoi cambiare il valore per adattarlo alla tua disposizione degli slot
            verticalMoved = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || (!verticalMoved && verticalInput < -0.5f) || (!verticalMoved && dpadVerticalInput < -0.5f))
        {
            MoveSelector(1); // Puoi cambiare il valore per adattarlo alla tua disposizione degli slot
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

    private void ToggleMenu()
    {
        isMenuActive = !isMenuActive;

        if (isMenuActive)
        {
            Time.timeScale = 0f; // Pausa il gioco
            MenuUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f; // Riprendi il gioco
            MenuUI.SetActive(false);
        }
    }

    private void MoveSelector(int direction)
    {
        currentSlotIndex += direction;

        if (currentSlotIndex < 0)
        {
            currentSlotIndex = slotImages.Length - 1;
        }
        else if (currentSlotIndex >= slotImages.Length)
        {
            currentSlotIndex = 0;
        }

        // Muovi il selettore alla posizione del nuovo slot
        if (selector != null && slotImages[currentSlotIndex] != null)
        {
            selector.position = slotImages[currentSlotIndex].transform.position;
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
        ToggleMenu(); // Nascondi il menu e riprendi il gioco

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
        SceneManager.LoadScene("Main Menu");
    }

    private void ShowSettings()
    {
        if (settingsOverlay != null)
        {
            settingsOverlay.SetActive(true);
            isOverlayActive = true; // Imposta il flag per indicare che un overlay è attivo
        }
    }

    

    private void CloseOverlay()
    {
        if (settingsOverlay != null)
        {
            settingsOverlay.SetActive(false); // Nascondi l'overlay delle impostazioni
        }

        

        // Mostra il menu principale se necessario
        if (MenuUI != null)
        {
            MenuUI.SetActive(true);
        }

        isOverlayActive = false; // Reimposta il flag per indicare che nessun overlay è attivo
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    public bool IsMenuActive()
{
    return isMenuActive;
}

}
