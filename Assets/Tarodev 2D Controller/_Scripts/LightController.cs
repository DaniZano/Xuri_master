using UnityEngine;
using TarodevController;
using System;

public class LightController : MonoBehaviour
{
    public Light pointLight;
    public float decreaseRate = 0.1f;  // Velocità di diminuzione del range della luce per secondo
    public float resetRange = 40f;     // Range della luce quando viene resettata
    public float maxRange = 40f;       // Range massimo della luce
    public float minRange = 0f;        // Range minimo della luce
    public LayerMask enemyLayer;       // Layer dei nemici
    public float enemyRange = 20f;

    public Color colorLessThan10 = Color.red;
    public Color colorGreaterThan10 = Color.white;

    private PlayerController playerController;

    #region Interface

    
    public event Action LightUp;

    #endregion

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController non trovato nel GameObject o nei suoi genitori.");
        }
    }

    private void Update()
    {


        // Diminuzione del range della luce nel tempo
        if (pointLight.range > minRange)
        {
            pointLight.range -= decreaseRate * Time.deltaTime;
        }

        // Aumento del range della luce quando si preme "Ctrl" e uccisione dei nemici nel raggio
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetButtonDown("Fire1") && playerController.UseCollectible()))
        {
            ResetLightAndKillEnemies();
        }

        // Cambia colore se la luce è sotto un certo livello
        if (pointLight.range < 10f)
        {
            pointLight.color = colorLessThan10;
        }
        else
        {
            pointLight.color = colorGreaterThan10;
        }

        // Controllo per la morte del giocatore
        CheckForDeath();
    }

    
    private void ResetLightAndKillEnemies()
    {
        pointLight.range = resetRange;

        // Trova tutti i nemici nel raggio della luce e li uccide
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, enemyRange, enemyLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            Entity enemy = hitCollider.GetComponent<Entity>();
            if (enemy != null)
            {
                enemy.Stun(); // stunna direttamente il nemico
            }
        }
    }

    public void ResetLightAfterRespawn()
    {
        pointLight.range = resetRange;
    }

    // Controllo per la morte del giocatore
    private void CheckForDeath()
    {
        if (pointLight.range <= 0)
        {
            bool isInAnotherLightRange = false;

            // Cerca tutte le luci secondarie e controlla se il giocatore è nel raggio di una di esse
            SecondaryLightController[] allSecondaryLights = FindObjectsOfType<SecondaryLightController>();
            foreach (SecondaryLightController lightController in allSecondaryLights)
            {
                if (lightController.playerInRange)
                {
                    isInAnotherLightRange = true;
                    break;
                }
            }

            if (!isInAnotherLightRange)
            {
                playerController.Respawn();
            }
        }
    }

    
}
