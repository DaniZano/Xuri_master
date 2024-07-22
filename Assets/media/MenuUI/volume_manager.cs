using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    private bool horizontalMoved = false;


    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    void Update()
    {

        float horizontalInput = Input.GetAxis("Horizontal");
        float dpadHorizontalInput = Input.GetAxis("DPadHorizontal");

        // Gestione delle frecce per modificare il volume
        float volumeChange = 0;

        if (Input.GetKey(KeyCode.LeftArrow) || (!horizontalMoved && horizontalInput < -0.5f) || (!horizontalMoved && dpadHorizontalInput < -0.5f))
        {
            volumeChange = -0.01f; // Diminuire il volume
        }
        else if (Input.GetKey(KeyCode.RightArrow) || (!horizontalMoved && horizontalInput > 0.5f) || (!horizontalMoved && dpadHorizontalInput > 0.5f))
        {
            volumeChange = 0.01f; // Aumentare il volume
        }

        if (volumeChange != 0)
        {
            volumeSlider.value = Mathf.Clamp(volumeSlider.value + volumeChange, 0, 1);
            ChangeVolume(); // Aggiorna il volume
            Save(); // Salva il volume
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}
