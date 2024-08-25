using UnityEngine;

public class DebugTeleport : MonoBehaviour
{
    public Transform[] teleportPoints; // Array di posizioni di teletrasporto
    private int currentTeleportIndex = 0; // Indice del punto di teletrasporto corrente

    // Impostazioni della combinazione di tasti
    public string leftBumperButton = "Skip1"; // Configurazione predefinita nel vecchio Input Manager (esempio: LB)
    public string rightBumperButton = "Skip2"; // Configurazione predefinita nel vecchio Input Manager (esempio: RB)
    public string leftTriggerButton = "Skip3"; // Configurazione predefinita nel vecchio Input Manager (esempio: RB)
    public string rightTriggerButton = "Skip4"; // Configurazione predefinita nel vecchio Input Manager (esempio: RB)




    void Update()
    {


        // Controlla se entrambe le levette del gamepad sono premute
        if (Input.GetButton(leftBumperButton) &&
           Input.GetButton(rightBumperButton) &&
           Input.GetButton(leftTriggerButton) &&
           Input.GetButtonDown(rightTriggerButton))
        {
            Debug.Log("Tutti e quattro i tasti (Skip1 + Skip2 + Skip3 + Skip4) sono stati premuti. Avvio del teletrasporto.");
            TeleportToNextValidPoint();
        }


    }

    void TeleportToNextValidPoint()
    {
        if (teleportPoints.Length == 0)
        {
            Debug.LogWarning("Nessun punto di teletrasporto assegnato!");
            return;
        }

        // Cerca il prossimo punto valido che il personaggio non ha ancora superato
        while (currentTeleportIndex < teleportPoints.Length &&
               transform.position.x > teleportPoints[currentTeleportIndex].position.x)
        {
            currentTeleportIndex++;
        }

        // Se tutti i punti sono stati superati, resetta all'inizio
        if (currentTeleportIndex >= teleportPoints.Length)
        {
            currentTeleportIndex = 0;
        }

        // Teletrasporta il personaggio al punto valido
        transform.position = teleportPoints[currentTeleportIndex].position;
        currentTeleportIndex++;
    }
}
