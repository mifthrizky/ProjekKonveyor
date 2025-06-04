using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrnjgSpawner : MonoBehaviour
{
     public GameObject krnjgPrefab;
    public Transform spawnPoint;
    public float spawnForce = 5f;

    public int jmlhkeranjang = 1;
    public float delay = 3.0f;

    public void StartSpawning()
    {
        StartCoroutine(SpawnMultipleBoxesWithDelay());
    }

    public void SpawnSingleBox()
    {
        if (krnjgPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Prefab atau Spawn Point belum di-set.");
            return;
        }

        GameObject newBox = Instantiate(krnjgPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody rb = newBox.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(spawnPoint.forward * spawnForce, ForceMode.Impulse);
        }
    }

    IEnumerator SpawnMultipleBoxesWithDelay()   
    {
        Debug.Log("Memulai proses spawn keranjang...");
        for (int i = 0; i < jmlhkeranjang; i++)
        {
            SpawnSingleBox();
            if (i < jmlhkeranjang - 1)
                yield return new WaitForSeconds(delay);
        }
        Debug.Log("Selesai spawn keranjang.");
    }
}
