using UnityEngine;

// Prints a debug log when a code is scanned.
public class BarcodeScannerDebug : MonoBehaviour
{
    private void Start()
    {
        BarcodeScanner scanner = GetComponent<BarcodeScanner>();
        if(scanner != null)
        {
            scanner.CodeScanned.AddListener(OnCodeScanned);
        }
    }

    private void OnCodeScanned(string scannedCode)
    {
        Debug.Log($"The following code has been scanned: {scannedCode}");
    }
}
