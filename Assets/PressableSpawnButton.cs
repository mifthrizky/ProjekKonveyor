using UnityEngine;
using System.Collections; // Diperlukan untuk Coroutine jika Anda menggunakan animasi tombol

public class PressableSpawnButton : MonoBehaviour
{
    [Header("Konfigurasi Spawn")]
    public GameObject containerPrefab;     // Assign Prefab Wadah Anda di Inspector
    public Transform containerSpawnPoint;  // Assign Transform titik spawn Wadah di Inspector
    public ForkliftController forkliftController; // Assign instance ForkliftController dari scene Anda di Inspector

    [Header("Animasi Tombol (Opsional)")]
    public float pressAnimationDuration = 0.1f;
    public Vector3 pressOffset = new Vector3(0, -0.02f, 0); // Seberapa dalam tombol bergerak
    public float actionCooldown = 0.5f; // Cooldown untuk mencegah klik spam

    private Vector3 originalLocalPosition;
    private bool isAnimating = false;
    private bool onActionCooldown = false;

    void Start()
    {
        // Validasi referensi penting
        if (containerPrefab == null)
        {
            Debug.LogError("Tombol '" + gameObject.name + "': Container Prefab belum di-assign! Skrip dinonaktifkan.");
            enabled = false; return;
        }
        if (containerSpawnPoint == null)
        {
            Debug.LogError("Tombol '" + gameObject.name + "': Container Spawn Point belum di-assign! Skrip dinonaktifkan.");
            enabled = false; return;
        }
        if (forkliftController == null)
        {
            Debug.LogError("Tombol '" + gameObject.name + "': ForkliftController belum di-assign! Skrip dinonaktifkan.");
            enabled = false; return;
        }

        originalLocalPosition = transform.localPosition;

        // Cek apakah ada Collider untuk OnMouseDown
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning("Tombol '" + gameObject.name + "' tidak memiliki Collider. OnMouseDown mungkin tidak berfungsi. Anda bisa memanggil TriggerButtonPress() dari sistem input lain.");
        }
    }

    // Metode ini dipanggil jika tombol memiliki Collider dan diklik mouse
    void OnMouseDown()
    {
        HandlePress();
    }

    // Metode publik ini bisa dipanggil dari sistem interaksi lain (misalnya event dari XR Interaction Toolkit)
    public void TriggerButtonPress()
    {
        HandlePress();
    }

    private void HandlePress()
    {
        if (isAnimating || onActionCooldown)
        {
            // Debug.Log("Tombol sedang animasi atau cooldown, aksi diabaikan.");
            return;
        }

        Debug.Log("Tombol '" + gameObject.name + "' ditekan. Memulai proses spawn wadah.");
        SpawnContainerNow(); // Langsung panggil metode spawn
        StartCoroutine(AnimateAndCooldownCoroutine()); // Jalankan animasi dan cooldown
    }

    private void SpawnContainerNow()
    {
        // Pengecekan referensi sekali lagi untuk keamanan
        if (containerPrefab == null || containerSpawnPoint == null || forkliftController == null)
        {
            Debug.LogError("Tombol '" + gameObject.name + "' tidak bisa spawn wadah: referensi penting hilang.");
            return;
        }

        // Buat instance baru dari prefab wadah di titik spawn
        GameObject newContainerGO = Instantiate(containerPrefab, containerSpawnPoint.position, containerSpawnPoint.rotation);
        // Beri nama unik untuk memudahkan identifikasi di Hierarchy
        newContainerGO.name = containerPrefab.name + "_Instance_ByButton_" + Time.frameCount;

        // Dapatkan komponen ContainerController dari instance wadah baru
        ContainerController newCC = newContainerGO.GetComponent<ContainerController>();
        if (newCC != null)
        {
            // PENTING: Berikan referensi forklift ke wadah baru ini
            // agar wadah baru tahu siapa yang harus dipanggil saat penuh.
            newCC.forklift = this.forkliftController;
            Debug.Log("Wadah baru '" + newContainerGO.name + "' telah di-spawn oleh tombol dan terhubung ke Forklift '" + forkliftController.name + "'.");
        }
        else
        {
            Debug.LogError("Prefab Wadah '" + containerPrefab.name + "' yang di-spawn tidak memiliki komponen ContainerController! Menghancurkan instance yang salah.");
            Destroy(newContainerGO); // Hancurkan instance jika tidak ada controller-nya
        }
    }

    IEnumerator AnimateAndCooldownCoroutine()
    {
        isAnimating = true;
        onActionCooldown = true;

        // Animasi tekan sederhana
        transform.localPosition = originalLocalPosition + pressOffset;
        yield return new WaitForSeconds(pressAnimationDuration);
        transform.localPosition = originalLocalPosition;
        
        isAnimating = false;

        // Cooldown sebelum tombol bisa diaktifkan lagi
        // Pastikan durasi cooldown lebih besar dari durasi animasi jika perlu
        float remainingCooldown = actionCooldown - pressAnimationDuration;
        if (remainingCooldown > 0)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }
        onActionCooldown = false;
        // Debug.Log("Cooldown tombol '" + gameObject.name + "' selesai.");
    }
}