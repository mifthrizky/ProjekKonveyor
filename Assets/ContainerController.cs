using UnityEngine;
using System.Collections.Generic;

public class ContainerController : MonoBehaviour
{
    public static int maxBoxCount = 3;
    private List<GameObject> boxesInContainer = new List<GameObject>();

    public bool isFull = false;
    public bool isBeingCarried = false;

    // Tag baru untuk menandai box yang sudah diproses
    public string processedBoxTag = "ProcessedBox"; // Anda bisa atur ini di Inspector jika mau

    void OnTriggerEnter(Collider other)
    {
        if (isFull || isBeingCarried) return;

        // Hanya proses jika tag-nya "Box" (box baru)
        if (other.CompareTag("Box") && !boxesInContainer.Contains(other.gameObject))
        {
            boxesInContainer.Add(other.gameObject);
            Debug.Log("Box masuk: " + other.gameObject.name + ". Total di '" + this.name + "': " + boxesInContainer.Count);

            if (boxesInContainer.Count >= maxBoxCount)
            {
                isFull = true;
                Debug.Log("Wadah '" + this.name + "' Penuh! Memanggil Forklift.");
                
                foreach (GameObject g in boxesInContainer)
                    g.transform.SetParent(this.transform);

                ForkliftController.instances.GoToContainer(this);
                /*
                else
                {
                    Debug.LogError("Referensi ForkliftController belum di-assign pada ContainerController di '" + this.name + "'!");
                }
                */
            }
        }
    }

    public void SecureBoxesForTransport(bool secure)
    {
        this.isBeingCarried = secure;
        foreach (GameObject boxGO in boxesInContainer)
        {
            if (boxGO == null) continue;
            Rigidbody boxRb = boxGO.GetComponent<Rigidbody>();
            if (boxRb != null)
            {
                boxRb.isKinematic = secure;
            }
        }
        Debug.Log("Box di dalam '" + this.name + "' telah diatur kinematic: " + secure);
    }

    public void ResetContainer()
    {
        Debug.Log("Mereset Wadah secara logika: '" + this.name + "'. Box fisik lama akan diubah tag-nya.");

        // Penting: SecureBoxesForTransport(false) sudah dipanggil oleh ForkliftController sebelumnya,
        // jadi box-box di `boxesInContainer` (sebelum di-clear) sudah non-kinematic.

        // Iterate SEMUA child transform dari wadah ini.
        // Box-box dari muatan sebelumnya adalah child dari wadah ini.
        List<Transform> childrenToProcess = new List<Transform>();
        foreach (Transform child in transform) // 'transform' adalah transform dari GameObject wadah ini
        {
            childrenToProcess.Add(child); // Tambahkan ke list sementara untuk menghindari masalah modifikasi saat iterasi
        }

        foreach (Transform childBoxTransform in childrenToProcess)
        {
            // Hanya ubah tag jika memang itu box yang perlu diproses (misalnya, masih bertag "Box")
            // atau jika Anda ingin menandai semua child yang merupakan box.
            if (childBoxTransform.CompareTag("Box")) // Hanya ubah tag jika masih "Box"
            {
                childBoxTransform.tag = processedBoxTag; // Ganti tag-nya
                Debug.Log("Mengubah tag untuk box lama: '" + childBoxTransform.name + "' menjadi '" + processedBoxTag + "'");

                // Pastikan juga fisika aktif jika belum (walaupun SecureBoxesForTransport(false) seharusnya sudah)
                Rigidbody rb = childBoxTransform.GetComponent<Rigidbody>();
                if (rb != null && rb.isKinematic)
                {
                    rb.isKinematic = false;
                }
            }
        }

        // Kosongkan list referensi box internal dan reset status
        boxesInContainer.Clear();
        isFull = false;
        isBeingCarried = false; // Pastikan status ini juga false
    }
}