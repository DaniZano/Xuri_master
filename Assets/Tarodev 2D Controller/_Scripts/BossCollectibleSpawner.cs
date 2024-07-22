using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class BossCollectibleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab; // Il prefab dell'oggetto da spawnare
    [SerializeField] private BoxCollider2D spawnArea; // Area di spawn
    [SerializeField] private float respawnDelay = 5f; // Tempo di attesa prima del respawn del prossimo oggetto
    [SerializeField] private Transform[] spawnPoints; // Punti di spawn degli oggetti

    private List<GameObject> spawnedObjects = new List<GameObject>(); // Elenco degli oggetti spawnati
    private bool playerInArea = false;

    private void OnEnable()
    {
        PlayerController.OnCollected += HandleObjectCollected; // Iscriviti all'evento statico
    }

    private void OnDisable()
    {
        PlayerController.OnCollected -= HandleObjectCollected; // Rimuovi l'iscrizione all'evento statico
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = true;
            if (spawnedObjects.Count == 0)
            {
                SpawnObject();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = false;
            //DestroyAllSpawnedObjects();
            StopAllCoroutines(); // Ferma il respawn se il giocatore esce dall'area
        }
    }

    private void SpawnObject()
    {
        Vector3 spawnPosition = GetRandomSpawnPoint();
        GameObject newObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(newObject);
    }

    private void HandleObjectCollected()
    {
        if (playerInArea)
        {
            StartCoroutine(RespawnObject());
        }
    }

    private IEnumerator RespawnObject()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (playerInArea) // Verifica se il giocatore è ancora nell'area
        {
            SpawnObject();
        }
    }

    private Vector3 GetRandomSpawnPoint()
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

    private void DestroyAllSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
    }
}
