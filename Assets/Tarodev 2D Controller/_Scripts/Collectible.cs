using UnityEngine;
using System;
using TarodevController;
using System.Collections.Generic;

public class Collectible : MonoBehaviour
{
    private static List<Collectible> allCollectibles = new List<Collectible>();

    public static event Action<Collectible> OnCollected; // Evento statico per segnalare il collezionabile raccolto

    private void Awake()
    {
        allCollectibles.Add(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CollectibleCollected();
                gameObject.SetActive(false); // Disabilita il collezionabile
                allCollectibles.Remove(this); // Rimuovi il collezionabile dalla lista
                OnCollected?.Invoke(this); // Invoca l'evento OnCollected
            }
        }
    }

    public static void CollectiblesReappear()
    {
        foreach (Collectible collectible in allCollectibles)
        {
            collectible.gameObject.SetActive(true); // Riattiva il collezionabile
        }
    }
}
