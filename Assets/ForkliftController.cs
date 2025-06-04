using UnityEngine;
using UnityEngine.AI; // Penting untuk NavMeshAgent

public class ForkliftController : MonoBehaviour
{
    public static ForkliftController instances;
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

    void Awake()
    {
        instances = this;
    }

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
            // currentTargetContainer.isBeingCarried = true; // Ini akan dihandle oleh SecureBoxesForTransport

            Rigidbody containerRb = currentTargetContainer.GetComponent<Rigidbody>();
            if (containerRb != null)
            {
                containerRb.isKinematic = true;
            }

            currentTargetContainer.transform.SetParent(forkAttachmentPoint);
            currentTargetContainer.transform.localPosition = Vector3.zero + Vector3.up * liftHeight;
            currentTargetContainer.transform.localRotation = Quaternion.identity;

            // 2. Beri tahu wadah untuk mengamankan box-box di dalamnya
            currentTargetContainer.SecureBoxesForTransport(true);

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

            // 3. Beri tahu wadah untuk "melepaskan" box-box (mengembalikan fisikanya jika perlu)
            // Ini dipanggil sebelum ResetContainer, yang mana akan menghancurkan box.
            // Jadi, efek utamanya adalah mengembalikan isKinematic ke false jika box tidak langsung dihancurkan.
            currentTargetContainer.SecureBoxesForTransport(false);

            currentTargetContainer.transform.SetParent(null);
            // ... (Pengaturan posisi drop wadah) ...
            Vector3 dropPosition = forkliftStartPosition.position + forkliftStartPosition.forward * 2f;
            dropPosition.y = currentTargetContainer.transform.position.y - liftHeight; // Asumsi ketinggian lantai
            // Raycast ke bawah untuk posisi Y yang lebih akurat jika lantai tidak rata:
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(dropPosition.x, transform.position.y + 1f, dropPosition.z), Vector3.down, out hit, 5f)) {
                dropPosition.y = hit.point.y + (currentTargetContainer.GetComponent<Collider>() != null ? currentTargetContainer.GetComponent<Collider>().bounds.extents.y : 0);
            } else {
                 // Jika tidak ada hit, gunakan posisi yang sudah dihitung (dikurangi liftHeight)
                // atau set ke Y=0 jika itu lantai Anda
                dropPosition.y = currentTargetContainer.transform.position.y - liftHeight;
            }
            currentTargetContainer.transform.position = dropPosition;


            Rigidbody containerRb = currentTargetContainer.GetComponent<Rigidbody>();
            if (containerRb != null)
            {
                containerRb.isKinematic = false;
            }

            currentTargetContainer.ResetContainer(); // Ini akan menghancurkan box-box
            currentTargetContainer = null;
            containerPickupTransform = null;
        }

        transform.rotation = forkliftStartPosition.rotation;
        agent.isStopped = false;
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