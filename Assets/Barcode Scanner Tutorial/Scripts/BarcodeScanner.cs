using UnityEngine;
using UnityEngine.Events;

public class BarcodeScanner : MonoBehaviour
{
    [SerializeField]
    private Transform rayTransform;

    [SerializeField]
    private float scanDistance = 0.1f;

    public string LastCodeScanned { get; private set; }

    public UnityEvent<string> CodeScanned;

    private Material emitterMaterial;

    private bool isScanSuccessful;
    private bool isScanning;

    //Tambahan: Referensi audio
    public AudioSource audioSource;
    public AudioClip kirimSekarangClip;
    public AudioClip kirimEsokHariClip;

    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            emitterMaterial = GetComponent<MeshRenderer>().materials[1];
        }

        UpdateEmitterColor();
    }

    private void FixedUpdate()
    {
        if (isScanning && !isScanSuccessful)
        {
            RaycastHit hit;
            Ray ray = new Ray(rayTransform.position, rayTransform.TransformDirection(Vector3.forward));
            if (Physics.Raycast(ray, out hit, scanDistance))
            {
                BarcodeLabel barcodeLabel = hit.collider.gameObject.GetComponent<BarcodeLabel>();

                if (barcodeLabel != null)
                {
                    isScanSuccessful = true;
                    LastCodeScanned = barcodeLabel.BarcodeValue;

                    //Putar audio berdasarkan barcode
                    PlayScanAudio(LastCodeScanned);

                    CodeScanned.Invoke(LastCodeScanned);
                }
            }
        }
    }

    public void StartScanning()
    {
        isScanSuccessful = false;
        isScanning = true;

        UpdateEmitterColor();
    }

    public void StopScanning()
    {
        isScanning = false;

        UpdateEmitterColor();
    }

    private void UpdateEmitterColor()
    {
        if (emitterMaterial != null)
        {
            emitterMaterial.color = isScanning ? Color.red : Color.black;
        }
    }

    //Fungsi tambahan untuk memainkan audio
    private void PlayScanAudio(string barcodeValue)
    {
        if (audioSource == null) return;

        switch (barcodeValue)
        {
            case "Produk1": //Sesuai BarcodeValue
                audioSource.clip = kirimSekarangClip;
                audioSource.Play();
                break;

            case "Produk2":
                audioSource.clip = kirimEsokHariClip;
                audioSource.Play();
                break;

            default:
                Debug.Log("Barcode tidak dikenali: " + barcodeValue);
                break;
        }
    }
}
