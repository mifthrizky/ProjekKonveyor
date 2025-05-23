using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab;         // Assign prefab kotak di inspector
    public Transform spawnPoint;         // Titik spawn (misal di tengah objek spawn)
    public float spawnForce = 5f;        // Arahkan ke konveyor

    public void SpawnBox()
    {
        GameObject newBox = Instantiate(boxPrefab, spawnPoint.position, Quaternion.identity);

        // Tambahkan gaya agar kotak bergerak ke arah konveyor
        Rigidbody rb = newBox.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(spawnPoint.forward * spawnForce, ForceMode.Impulse);
        }
    }

    void Start()
    {
        SpawnBox(); 
    }
}
