using UnityEngine;
using VRBuilder.XRInteraction.Properties; // Untuk UsableProperty
using VRBuilder.BasicInteraction.Properties; // Untuk GrabbableProperty
// LockableProperty ada di VRBuilder.Core.Properties berdasarkan skrip yang Anda berikan
using VRBuilder.Core.Properties;

public class ForceScannerActive : MonoBehaviour
{
    private GrabbableProperty grabbableProperty;
    private UsableProperty usableProperty; // UsableProperty mewarisi LockableProperty

    void Awake()
    {
        // Dapatkan referensi ke komponen yang dibutuhkan saat Awake
        grabbableProperty = GetComponent<GrabbableProperty>();
        usableProperty = GetComponent<UsableProperty>();

        // Validasi apakah semua komponen ada
        if (grabbableProperty == null)
        {
            Debug.LogError($"'{gameObject.name}' - ForceScannerActive: Komponen GrabbableProperty tidak ditemukan!", this.gameObject);
        }

        if (usableProperty == null)
        {
            Debug.LogError($"'{gameObject.name}' - ForceScannerActive: Komponen UsableProperty tidak ditemukan!", this.gameObject);
        }

        // Nonaktifkan skrip ini jika komponen penting tidak ada untuk menghindari error berkelanjutan
        if (grabbableProperty == null || usableProperty == null)
        {
            Debug.LogError($"'{gameObject.name}' - ForceScannerActive: Menonaktifkan skrip karena komponen penting tidak ada.");
            enabled = false;
        }
    }

    void LateUpdate()
    {
        // Jika komponen usableProperty ada dan saat ini terkunci
        if (usableProperty != null && usableProperty.IsLocked)
        {
            // Panggil metode publik SetLocked(false) untuk membuka kuncinya.
            // Ini akan menjalankan logika di LockableProperty.SetLocked dan kemudian
            // UsableProperty.InternalSetLocked(false)
            usableProperty.SetLocked(false);
            // Untuk debugging, Anda bisa uncomment baris di bawah ini:
            // Debug.Log($"'{gameObject.name}' - ForceScannerActive: Memaksa UsableProperty.SetLocked(false) karena IsLocked true.");
        }

        // Pastikan komponen GrabbableProperty selalu aktif (enabled)
        if (grabbableProperty != null && !grabbableProperty.enabled)
        {
            grabbableProperty.enabled = true;
            // Untuk debugging, Anda bisa uncomment baris di bawah ini:
            // Debug.Log($"'{gameObject.name}' - ForceScannerActive: Memaksa GrabbableProperty.enabled = true.");
        }

        // Pastikan komponen UsableProperty selalu aktif (enabled)
        // Meskipun jika IsLocked adalah false, komponen UsableProperty itu sendiri mungkin dinonaktifkan.
        if (usableProperty != null && !usableProperty.enabled)
        {
            usableProperty.enabled = true;
            // Untuk debugging, Anda bisa uncomment baris di bawah ini:
            // Debug.Log($"'{gameObject.name}' - ForceScannerActive: Memaksa UsableProperty.enabled = true.");
        }

        // Catatan: Mungkin juga ada kebutuhan untuk berinteraksi dengan komponen
        // InteractableObject (VR Builder) secara langsung jika masalahnya lebih kompleks,
        // tapi biasanya mengelola LockableProperty dan status enabled dari Grabbable/Usable
        // sudah mencakup banyak kasus.
    }
}