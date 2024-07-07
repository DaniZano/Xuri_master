using System.Collections;
using UnityEngine;
using TarodevController;


public class BossCollectibleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab; // Il prefab dell'oggetto da spawnare
    [SerializeField] private BoxCollider2D spawnArea; // Area di spawn

    private GameObject currentObject;
    private Coroutine spawnCoroutine;
    private PlayerController playerController;

    private void Start()
    {
        // Trova il PlayerController nella scena

        
            spawnCoroutine = StartCoroutine(SpawnObjectRoutine());
       
    }

    private IEnumerator SpawnObjectRoutine()
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitUntil(() => currentObject == null); // Attendi finché l'oggetto corrente non viene collezionato
            yield return new WaitForSeconds(5f); // Tempo di attesa prima del respawn del nuovo oggetto
        }
    }

    private void SpawnObject()
    {
        Vector2 spawnPosition2D = GetRandomPositionInArea();
        Vector3 spawnPosition = new Vector3(spawnPosition2D.x, spawnPosition2D.y, spawnArea.transform.position.z);
        currentObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
    }

    private void HandleObjectCollected(GameObject collectedObject)
    {
        if (collectedObject == currentObject)
        {
            currentObject.SetActive(false); // Disabilita l'oggetto collezionato
            currentObject = null; // Imposta l'oggetto corrente a null quando viene collezionato
            playerController.CollectibleCollected(); // Chiamata al metodo CollectibleCollected() del PlayerController
        }
    }

    private Vector2 GetRandomPositionInArea()
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            Debug.Log("Player entered spawn area");
            if (spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(SpawnObjectRoutine()); // Avvia il ciclo di spawn quando il giocatore entra nell'area di spawn
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited spawn area");
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine); // Interrompe il ciclo di spawn quando il giocatore esce dall'area di spawn
                spawnCoroutine = null;
            }
        }
    }
}
