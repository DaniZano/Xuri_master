using System.Collections;
using UnityEngine;

public class BossCollectibleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab; // Il prefab dell'oggetto da spawnare
    [SerializeField] private BoxCollider2D spawnArea; // Area di spawn
    [SerializeField] private float respawnDelay = 5f; // Tempo di attesa prima del respawn del prossimo oggetto
    [SerializeField] private Transform[] spawnPoints; // Punti di spawn degli oggetti

    private GameObject currentObject;
    private Coroutine spawnCoroutine;

    private void Start()
    {
        // Avvia il ciclo di spawn solo una volta inizialmente
        SpawnObject();
    }

    private void OnEnable()
    {
        Collectible.OnCollected += HandleObjectCollected; // Iscriviti all'evento statico
    }

    private void OnDisable()
    {
        Collectible.OnCollected -= HandleObjectCollected; // Rimuovi l'iscrizione all'evento statico
    }

    private void SpawnObject()
    {
        Vector3 spawnPosition = GetRandomPositionInArea();
        currentObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
    }

    private void HandleObjectCollected(Collectible collectedObject)
    {
        collectedObject.gameObject.SetActive(false);
        currentObject = null; // Imposta l'oggetto corrente a null quando viene collezionato

        // Avvia il respawn dopo un periodo di tempo
        StartCoroutine(RespawnObject());
    }

    private IEnumerator RespawnObject()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnObject();
    }

    private Vector3 GetRandomPositionInArea()
    {
        if (spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex].position;
        }
        else
        {
            // Se non ci sono punti specificati, spawn nell'area di default
            Bounds bounds = spawnArea.bounds;
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            return new Vector3(x, y, spawnArea.transform.position.z);
        }
    }
}
