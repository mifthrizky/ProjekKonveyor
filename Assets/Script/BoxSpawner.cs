using UnityEngine;
using System.Collections;

public class BoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab;
    public Transform spawnPoint;
    public float spawnForce = 5f;

    public int jumlahbox = 5;
    public float delay = 3.0f;

    public void StartSpawning()
    {
        StartCoroutine(SpawnMultipleBoxesWithDelay());
    }

    public void SpawnSingleBox()
    {
        if (boxPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Prefab atau Spawn Point belum di-set.");
            return;
        }

        GameObject newBox = Instantiate(boxPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody rb = newBox.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(spawnPoint.forward * spawnForce, ForceMode.Impulse);
        }
    }

    IEnumerator SpawnMultipleBoxesWithDelay()   
    {
        Debug.Log("Memulai proses spawn multiple boxes...");
        for (int i = 0; i < jumlahbox; i++)
        {
            SpawnSingleBox();
            if (i < jumlahbox - 1)
                yield return new WaitForSeconds(delay);
        }
        Debug.Log("Selesai spawn semua boxes.");
    }
}
