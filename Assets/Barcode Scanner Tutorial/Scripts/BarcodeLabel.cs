using UnityEngine;

// Represents a barcode on a label.
public class BarcodeLabel : MonoBehaviour
{
    [SerializeField]
    private string barcodeValue = "";
    
    // Returns the string representing the barcode on this object.
    public string BarcodeValue => barcodeValue;

    // Sets the label to a new value.
    public void SetLabel(string value)
    {
        barcodeValue = value;
    }
}
