using UnityEngine;
using System.Collections.Generic; // Untuk menggunakan List

public class ContainerController : MonoBehaviour
{
    public int maxBoxCount = 5;
    private int currentBoxCount = 0;
    private List<GameObject> boxesInContainer = new List<GameObject>(); // Untuk melacak box

    public ForkliftController forklift; // Referensi ke skrip forklift
    public bool isFull = false;
    public bool isBeingCarried = false;

    // Opsional: Untuk mereset setelah forklift selesai
    public Transform boxSpawnPoint; // Jika box ingin di-spawn ulang atau dipindahkan
    public GameObject boxPrefab;    // Jika box ingin di-spawn ulang

    void OnTriggerEnter(Collider other)
    {
        if (isFull || isBeingCarried) return; // Jangan hitung jika sudah penuh atau sedang diangkut

        if (other.CompareTag("Box") && !boxesInContainer.Contains(other.gameObject))
        {
            boxesInContainer.Add(other.gameObject);
            currentBoxCount = boxesInContainer.Count;
            Debug.Log("Box masuk: " + other.gameObject.name + ". Total: " + currentBoxCount);

            // Hancurkan box yang masuk atau nonaktifkan agar tidak dihitung lagi
            // other.gameObject.SetActive(false);
            // Atau jika box punya Rigidbody dan harus diam di wadah:
            // Rigidbody boxRb = other.GetComponent<Rigidbody>();
            // if (boxRb != null)
            // {
            //     boxRb.isKinematic = true; // Hentikan gerakan fisika box di dalam wadah
            // }
            // Pindahkan box sedikit ke dalam wadah agar terlihat rapi
            // other.transform.SetParent(transform); // Opsional: jadikan box child dari wadah


            if (currentBoxCount >= maxBoxCount)
            {
                isFull = true;
                Debug.Log("Wadah Penuh! Memanggil Forklift.");
                if (forklift != null)
                {
                    forklift.GoToContainer(this);
                }
                else
                {
                    Debug.LogError("Referensi ForkliftController belum di-assign di ContainerController!");
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Jika Anda ingin box bisa keluar dan mengurangi hitungan (mungkin tidak relevan untuk kasus ini)
        if (other.CompareTag("Box") && boxesInContainer.Contains(other.gameObject))
        {
            // Hanya hapus jika tidak sedang diangkut atau jika logika Anda mengizinkan
            if (!isBeingCarried)
            {
                boxesInContainer.Remove(other.gameObject);
                currentBoxCount = boxesInContainer.Count;
                Debug.Log("Box keluar: " + other.gameObject.name + ". Total: " + currentBoxCount);
                isFull = false; // Wadah tidak lagi penuh
            }
        }
    }

    public void ResetContainer()
    {
        Debug.Log("Mereset Wadah.");
        // Hancurkan atau pindahkan semua box yang ada di wadah
        foreach (GameObject box in boxesInContainer)
        {
            // Contoh: Hancurkan box
            // Destroy(box);

            // Contoh: Pindahkan box ke suatu tempat (jika Anda punya pool atau ingin digunakan lagi)
            if (boxSpawnPoint != null && boxPrefab != null) // Atau nonaktifkan saja
            {
                // box.transform.position = boxSpawnPoint.position;
                // box.SetActive(true); // Jika dinonaktifkan sebelumnya
                Destroy(box); // Paling mudah untuk demo
            } else {
                Destroy(box);
            }
        }
        boxesInContainer.Clear();
        currentBoxCount = 0;
        isFull = false;
        isBeingCarried = false;
        // Mungkin Anda ingin memindahkan wadah kembali ke posisi semula jika ia dipindahkan oleh forklift
    }
}