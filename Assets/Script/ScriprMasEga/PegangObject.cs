using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class ThrowableInteractable : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Event: Saat objek dipegang
        grabInteractable.selectEntered.AddListener(OnGrab);
        // Event: Saat dilepas
        grabInteractable.selectExited.AddListener(OnRelease);

        // Event: Deteksi tombol grip (aktifasi)
        grabInteractable.activated.AddListener(OnGripPressed);
        grabInteractable.deactivated.AddListener(OnGripReleased);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Nonaktifkan physics saat dipegang
        rb.isKinematic = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Aktifkan physics saat dilepas
        rb.isKinematic = false;

        // Dapatkan interactor (tangan)
        var interactorObj = args.interactorObject as IXRInteractor;
        if (interactorObj != null)
        {
            var interactorGO = interactorObj.transform.gameObject;

            // Ambil XRVelocityTracker dari tangan
            var velocityTracker = interactorGO.GetComponent<XRVelocityTracker>();
            if (velocityTracker != null)
            {
                rb.velocity = velocityTracker.velocity;
                rb.angularVelocity = velocityTracker.angularVelocity;
            }
            else
            {
                Debug.LogWarning("XRVelocityTracker tidak ditemukan di tangan.");
            }
        }
    }

    // Event tombol grip ditekan
    private void OnGripPressed(ActivateEventArgs args)
    {
        Debug.Log("Grip ditekan!");
        // Tambahkan efek visual/suara jika perlu
    }

    // Event tombol grip dilepas
    private void OnGripReleased(DeactivateEventArgs args)
    {
        Debug.Log("Grip dilepas!");
    }
}
