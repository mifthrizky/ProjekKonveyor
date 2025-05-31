using UnityEngine;
using UnityEngine.AI; // Penting untuk NavMeshAgent

public class ForkliftController : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform forkliftStartPosition; // Titik awal dan kembali forklift
    public Transform forkAttachmentPoint;   // Titik di garpu forklift tempat wadah menempel

    private ContainerController currentTargetContainer;
    private Transform containerPickupTransform; // Posisi actual wadah yang akan diambil

    private enum ForkliftState
    {
        Idle,
        MovingToContainer,
        PickingUpContainer,
        MovingToStartPosition,
        DroppingContainer
    }
    private ForkliftState currentState;

    public float pickupDistanceThreshold = 1.5f; // Jarak untuk dianggap sampai dan mengambil
    public float dropDistanceThreshold = 1.5f;   // Jarak untuk dianggap sampai dan meletakkan
    public float liftHeight = 0.5f; // Seberapa tinggi wadah diangkat (relatif terhadap attachment point)

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent tidak ditemukan pada Forklift!");
            enabled = false;
            return;
        }

        if (forkliftStartPosition == null)
        {
            // Jika tidak diset, gunakan posisi awal objek forklift itu sendiri
            GameObject startPosObj = new GameObject("ForkliftActualStartPosition");
            startPosObj.transform.position = transform.position;
            startPosObj.transform.rotation = transform.rotation;
            forkliftStartPosition = startPosObj.transform;
            Debug.LogWarning("ForkliftStartPosition tidak di-assign, menggunakan posisi awal saat Start.");
        }
        else
        {
            // Pindahkan forklift ke posisi awal saat game dimulai
            transform.position = forkliftStartPosition.position;
            transform.rotation = forkliftStartPosition.rotation;
        }


        if (forkAttachmentPoint == null)
        {
            Debug.LogError("ForkAttachmentPoint belum di-assign di ForkliftController!");
            enabled = false;
            return;
        }

        currentState = ForkliftState.Idle;
        agent.stoppingDistance = pickupDistanceThreshold * 0.8f; // Sedikit lebih kecil agar lebih dekat
    }

    void Update()
    {
        switch (currentState)
        {
            case ForkliftState.Idle:
                // Menunggu panggilan
                break;

            case ForkliftState.MovingToContainer:
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    if (currentTargetContainer != null)
                    {
                        currentState = ForkliftState.PickingUpContainer;
                        Debug.Log("Sampai di Wadah. Memulai pengambilan.");
                        // Hentikan agent sementara agar tidak "jitter" saat pickup
                        agent.isStopped = true;
                        PerformPickup();
                    }
                }
                break;

            case ForkliftState.PickingUpContainer:
                // Proses pickup sudah dihandle di PerformPickup, state ini bisa singkat
                // atau bisa ada animasi, lalu lanjut ke MovingToStartPosition
                // Untuk simplisitas, kita langsung pindah state setelah PerformPickup
                break;

            case ForkliftState.MovingToStartPosition:
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    currentState = ForkliftState.DroppingContainer;
                    Debug.Log("Sampai di Posisi Awal. Memulai pelepasan.");
                    agent.isStopped = true;
                    PerformDrop();
                }
                break;

            case ForkliftState.DroppingContainer:
                // Proses drop sudah dihandle di PerformDrop
                break;
        }
    }

    public void GoToContainer(ContainerController container)
    {
        if (currentState == ForkliftState.Idle)
        {
            currentTargetContainer = container;
            containerPickupTransform = container.transform; // Simpan transform wadah
            agent.isStopped = false;
            agent.SetDestination(containerPickupTransform.position);
            currentState = ForkliftState.MovingToContainer;
            Debug.Log("Forklift bergerak menuju: " + container.name);
        }
        else
        {
            Debug.LogWarning("Forklift sedang sibuk, tidak bisa menerima perintah baru.");
        }
    }

    void PerformPickup()
    {
        if (currentTargetContainer != null && containerPickupTransform != null)
        {
            Debug.Log("Mengambil " + currentTargetContainer.name);
            currentTargetContainer.isBeingCarried = true;

            // Matikan fisika wadah jika ada, agar tidak bentrok
            Rigidbody containerRb = currentTargetContainer.GetComponent<Rigidbody>();
            if (containerRb != null)
            {
                containerRb.isKinematic = true;
            }

            // Smoothly move container to attachment point (Optional, bisa langsung parent)
            // Untuk animasi lebih smooth, bisa pakai Lerp atau iTween/DOTween
            currentTargetContainer.transform.SetParent(forkAttachmentPoint);
            currentTargetContainer.transform.localPosition = Vector3.zero + Vector3.up * liftHeight; // Naikkan sedikit
            currentTargetContainer.transform.localRotation = Quaternion.identity; // Reset rotasi relatif

            // Setelah pickup, kembali ke start position
            agent.isStopped = false;
            agent.SetDestination(forkliftStartPosition.position);
            currentState = ForkliftState.MovingToStartPosition;
            Debug.Log("Wadah diambil. Kembali ke posisi awal.");
        }
    }

    void PerformDrop()
    {
        if (currentTargetContainer != null)
        {
            Debug.Log("Meletakkan " + currentTargetContainer.name);
            currentTargetContainer.transform.SetParent(null); // Lepas dari forklift
            currentTargetContainer.isBeingCarried = false;

            // Posisikan wadah dengan rapi di lantai (sesuaikan Y jika perlu)
            Vector3 dropPosition = new Vector3(transform.position.x, 0, transform.position.z); // Asumsi lantai di Y=0
             // Coba offset agar tidak pas di bawah forklift
            dropPosition += forkliftStartPosition.forward * 2f; // Letakkan 2 unit di depan posisi start forklift
            dropPosition.y = currentTargetContainer.transform.position.y - liftHeight; // Kembalikan ke ketinggian semula relatif

            currentTargetContainer.transform.position = dropPosition;


            // Aktifkan kembali fisika jika perlu
            Rigidbody containerRb = currentTargetContainer.GetComponent<Rigidbody>();
            if (containerRb != null)
            {
                containerRb.isKinematic = false; // Atau true jika wadah harus diam setelah diletakkan
            }

            // Reset wadah untuk siklus berikutnya
            currentTargetContainer.ResetContainer();
            currentTargetContainer = null;
            containerPickupTransform = null;
        }

        // Kembali ke state Idle
        // Mungkin forklift perlu berputar kembali ke orientasi awalnya jika penting
        transform.rotation = forkliftStartPosition.rotation;
        agent.isStopped = false; // Siap untuk tugas berikutnya
        currentState = ForkliftState.Idle;
        Debug.Log("Wadah diletakkan. Forklift Idle.");
    }

    // Gizmos untuk visualisasi di editor
    void OnDrawGizmosSelected()
    {
        if (forkAttachmentPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(forkAttachmentPoint.position, 0.2f);
            Gizmos.DrawRay(forkAttachmentPoint.position, forkAttachmentPoint.forward * 1f);
        }
        if (forkliftStartPosition != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(forkliftStartPosition.position, 0.5f);
        }
    }
}