using UnityEngine;
using System.Collections;

public class LightObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // L'oggetto che desideri spawnare
    public Transform spawnPoint; // Il punto in cui spawnare l'oggetto
    public float spawnInterval = 3f; // L'intervallo di tempo tra gli spawn
    public float despawnTime = 2f; // Il tempo dopo cui l'oggetto scompare

    private void Start()
    {
        StartCoroutine(SpawnDespawnCycle());
    }

    private IEnumerator SpawnDespawnCycle()
    {
        while (true)
        {
            // Spawn dell'oggetto
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPoint.position, spawnPoint.rotation);

            // Aspetta per il tempo di despawn
            yield return new WaitForSeconds(despawnTime);

            // Distruggi l'oggetto
            Destroy(spawnedObject);

            // Aspetta l'intervallo di spawn
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
