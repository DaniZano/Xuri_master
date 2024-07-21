using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerActivationArea : MonoBehaviour
{
    public Miniboss miniboss; // Riferimento al componente Miniboss
    private bool playerInArea = false; // Stato del giocatore nell'area

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInArea = true;
            if (miniboss != null)
            {
                miniboss.PlayerEnteredPowerArea();
            }
            Debug.Log("Giocatore entrato nell'area di attivazione del potere.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInArea = false;
            if (miniboss != null)
            {
                miniboss.PlayerExitedPowerArea();
            }
            Debug.Log("Giocatore uscito dall'area di attivazione del potere.");
        }
    }
}
