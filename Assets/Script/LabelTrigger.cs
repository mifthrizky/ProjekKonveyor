using UnityEngine;
using System.Collections;

public class LabelTrigger : MonoBehaviour
{
    [Header("Prefabs untuk Di-spawn")]
    public GameObject barang1;   // Prefab untuk benda urutan ganjil (1, 3, 5, ...)
    public GameObject barang2;  // Prefab untuk benda urutan genap (2, 4, 6, ...)

    [Header("Pengaturan Spawn")]
    public Vector3 spawnOffset = new Vector3(0, 0.1f, 0); // Offset agar tidak nabrak collider

    private int detectedItemCount = 0; // Penghitung berapa banyak item "BoxMentah" yang sudah terdeteksi

    private void OnTriggerEnter(Collider other)
    {
        // Hanya proses jika objek yang masuk memiliki tag "BoxMentah"
        if (other.CompareTag("BoxMentah"))
        {
            detectedItemCount++; // Tambah penghitung setiap kali "BoxMentah" terdeteksi

            GameObject prefabToInstantiate = null;
            string originalItemName = other.gameObject.name; // Simpan nama original untuk logging/penamaan

            Debug.Log($"'{originalItemName}' (Tag: {other.tag}) memasuki trigger. Ini adalah item ke-{detectedItemCount}.");

            // Tentukan prefab mana yang akan digunakan berdasarkan urutan kedatangan (ganjil/genap)
            if (detectedItemCount % 2 == 1) // Jika detectedItemCount adalah ganjil (1, 3, 5, ...)
            {
                prefabToInstantiate = barang1;
                if (prefabToInstantiate == null)
                {
                    Debug.LogError("barang1 belum di-assign di Inspector!");
                    Destroy(other.gameObject); // Hancurkan item asli
                    return;
                }
                Debug.Log($"Menggunakan barang1 untuk item ke-{detectedItemCount} ('{originalItemName}').");
            }
            else // Jika detectedItemCount adalah genap (2, 4, 6, ...)
            {
                prefabToInstantiate = barang2;
                if (prefabToInstantiate == null)
                {
                    Debug.LogError("barang2 belum di-assign di Inspector!");
                    Destroy(other.gameObject); // Hancurkan item asli
                    return;
                }
                Debug.Log($"Menggunakan barang2 untuk item ke-{detectedItemCount} ('{originalItemName}').");
            }

            // Ambil posisi dan rotasi dari objek yang masuk sebelum dihancurkan
            Vector3 position = other.transform.position + spawnOffset;
            Quaternion rotation = other.transform.rotation;

            // Hancurkan objek "BoxMentah" yang lama
            Destroy(other.gameObject);
            Debug.Log($"Objek asli '{originalItemName}' dihancurkan.");

            // Instantiate prefab yang sudah ditentukan
            // Pengecekan null sudah dilakukan di atas, jadi kita bisa langsung instantiate
            GameObject newBox = Instantiate(prefabToInstantiate, position, rotation);
            newBox.name = $"{originalItemName}_changed_to_{prefabToInstantiate.name}_item{detectedItemCount}";
            Debug.Log($"Objek baru '{newBox.name}' di-Instantiate dari prefab '{prefabToInstantiate.name}'.");

            // Aktifkan collider setelah delay singkat untuk menghindari konflik langsung
            StartCoroutine(EnableColliderAfterDelay(newBox, 0.1f));
        }
    }

    private IEnumerator EnableColliderAfterDelay(GameObject obj, float delay)
    {
        if (obj == null)
        {
            Debug.LogWarning("Mencoba EnableColliderAfterDelay pada objek null.");
            yield break;
        }

        if (obj.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
            Debug.Log($"Collider pada '{obj.name}' dinonaktifkan sementara.");
            yield return new WaitForSeconds(delay);
            col.enabled = true;
            Debug.Log($"Collider pada '{obj.name}' diaktifkan kembali.");
        }
        else
        {
            Debug.LogWarning($"Tidak ditemukan komponen Collider pada '{obj.name}' untuk diaktifkan setelah delay.");
        }
    }
}