using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    [Header("Object References")]
    public Transform container;             // Wadah tempat box ditampung
    public GameObject forklift;             // Forklift yang bergerak
    public Transform targetPosition;        // Tujuan pengantaran (misalnya garis kuning)
    public Transform initialPosition;       // Posisi awal forklift

    [Header("Settings")]
    public float moveSpeed = 2f;            // Kecepatan gerak forklift
    public float liftHeight = 1.5f;         // Ketinggian pengangkatan container
    public float liftSpeed = 1f;            // Kecepatan pengangkatan/turunkan

    private bool forkliftIsMoving = false;

    /// <summary>
    /// Dipanggil dari ContainerTrigger saat box telah mencapai jumlah yang ditentukan.
    /// </summary>
    public void TriggerForklift()
    {
        if (!forkliftIsMoving)
        {
            StartCoroutine(MoveForkliftSequence());
        }
    }

    /// <summary>
    /// Urutan aksi forklift: maju -> angkat -> pindah -> turun -> kembali.
    /// </summary>
    IEnumerator MoveForkliftSequence()
    {
        forkliftIsMoving = true;

        // 1. Gerak ke depan container
        yield return MoveToPosition(forklift.transform, container.position);

        // 2. Angkat container
        yield return Lift(container, liftHeight);

        // 3. Parent container ke forklift agar ikut bergerak
        container.SetParent(forklift.transform);

        // 4. Gerak ke posisi target (garis kuning)
        yield return MoveToPosition(forklift.transform, targetPosition.position);

        // 5. Turunkan container
        yield return Lift(container, -liftHeight);

        // 6. Unparent container agar tetap di tempat
        container.SetParent(null);

        // 7. Forklift kembali ke posisi awal
        yield return MoveToPosition(forklift.transform, initialPosition.position);

        forkliftIsMoving = false;
    }

    /// <summary>
    /// Menggerakkan objek ke target dengan kecepatan tertentu.
    /// </summary>
    IEnumerator MoveToPosition(Transform obj, Vector3 targetPos)
    {
        while (Vector3.Distance(obj.position, targetPos) > 0.1f)
        {
            obj.position = Vector3.MoveTowards(obj.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Mengangkat atau menurunkan objek secara vertikal.
    /// </summary>
    IEnumerator Lift(Transform obj, float height)
    {
        Vector3 startPos = obj.position;
        Vector3 endPos = startPos + new Vector3(0, height, 0);

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            obj.position = Vector3.Lerp(startPos, endPos, elapsed);
            elapsed += Time.deltaTime * liftSpeed;
            yield return null;
        }

        obj.position = endPos;
    }
}
