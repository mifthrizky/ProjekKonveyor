using UnityEngine;
using System.Collections;

public class LabelTrigger : MonoBehaviour
{
    public GameObject boxPrefab; // Prefab Box kayu
    public Vector3 spawnOffset = new Vector3(0, 0.1f, 0); // Offset agar tidak nabrak collider

    private void OnTriggerEnter(Collider other)
    {
        // Gunakan Tag agar lebih aman dan cepat
        if (other.CompareTag("BoxMentah"))
        {
            Vector3 position = other.transform.position + spawnOffset;
            Quaternion rotation = other.transform.rotation;

            Destroy(other.gameObject); // Hancurkan yang lama

            GameObject newBox = Instantiate(boxPrefab, position, rotation);

            // Aktifkan collider setelah delay singkat
            StartCoroutine(EnableColliderAfterDelay(newBox, 0.1f));
        }
    }

    private IEnumerator EnableColliderAfterDelay(GameObject obj, float delay)
    {
        if (obj.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
            yield return new WaitForSeconds(delay);
            col.enabled = true;
        }
    }
}
