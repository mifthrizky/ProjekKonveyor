using UnityEngine;
using System.Collections.Generic;

public class ContainerController : MonoBehaviour
{
    public int maxBoxCount = 5;
    private int currentBoxCount = 0;
    private List<GameObject> boxesInContainer = new List<GameObject>();

    public ForkliftController forklift;
    public bool isFull = false;
    public bool isBeingCarried = false; // Akan dikontrol oleh metode SecureBoxesForTransport

    public Transform boxSpawnPoint;
    public GameObject boxPrefab;

    void OnTriggerEnter(Collider other)
    {
        if (isFull || isBeingCarried) return;

        if (other.CompareTag("Box") && !boxesInContainer.Contains(other.gameObject))
        {
            boxesInContainer.Add(other.gameObject);
            currentBoxCount = boxesInContainer.Count;
            Debug.Log("Box masuk: " + other.gameObject.name + ". Total: " + currentBoxCount);

            // 1. Jadikan box sebagai child dari wadah ini
            other.transform.SetParent(this.transform);
            // Opsional: Reset posisi lokal box jika perlu agar rapi di dalam wadah
            // other.transform.localPosition = new Vector3(Random.Range(-0.2f, 0.2f), other.transform.localPosition.y, Random.Range(-0.2f, 0.2f));
            // other.transform.localRotation = Quaternion.identity;

            // Rigidbody boxRb = other.GetComponent<Rigidbody>();
            // if (boxRb != null)
            // {
            //     // Biarkan fisika box berjalan normal sampai forklift mengangkatnya
            //     // boxRb.isKinematic = false; // Pastikan tidak kinematic awalnya
            // }

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

    // Metode baru untuk mengamankan atau melepaskan box
    public void SecureBoxesForTransport(bool secure)
    {
        this.isBeingCarried = secure; // Tandai bahwa wadah sedang/tidak sedang diangkut
        // Debug.Log("Container " + this.gameObject.name + " isBeingCarried: " + this.isBeingCarried);

        foreach (GameObject boxGO in boxesInContainer)
        {
            if (boxGO == null) continue; // Jaga-jaga jika box sudah hancur

            Rigidbody boxRb = boxGO.GetComponent<Rigidbody>();
            if (boxRb != null)
            {
                boxRb.isKinematic = secure; // Kunci atau lepaskan fisika box
                // Jika melepaskan (secure == false) dan box masih child,
                // dan Anda tidak menghancurkannya, ia akan tetap di wadah
                // sampai ada gaya yang mengeluarkannya.
            }
        }
        // Debug.Log("Boxes in " + this.gameObject.name + " set to kinematic: " + secure);
    }

    public void ResetContainer()
    {
        Debug.Log("Mereset Wadah: " + this.gameObject.name);

        // Sebelum menghancurkan, pastikan mereka tidak lagi kinematic jika akan di-pool,
        // tapi karena kita destroy, tidak terlalu masalah.
        // SecureBoxesForTransport(false); // Opsional jika box tidak langsung dihancurkan

        foreach (GameObject box in boxesInContainer)
        {
            if (box != null) // Selalu cek null sebelum destroy
            {
                // Jika box di-parent, tidak perlu un-parent jika langsung di-destroy.
                // Jika tidak di-destroy dan ingin box keluar dari parent:
                // box.transform.SetParent(null);
                Destroy(box);
            }
        }
        boxesInContainer.Clear();
        currentBoxCount = 0;
        isFull = false;
        // isBeingCarried akan diatur oleh SecureBoxesForTransport(false) saat dilepas
        // Namun karena ResetContainer dipanggil setelah PerformDrop,
        // pastikan isBeingCarried juga false di sini untuk kejelasan.
        isBeingCarried = false;
    }

    // (Sisa OnTriggerExit jika ada...)
}