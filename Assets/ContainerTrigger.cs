using System.Collections.Generic;
using UnityEngine;

public class ContainerTrigger : MonoBehaviour
{
    public int requiredBoxCount = 5;
    public BoxManager boxManager; // Drag script BoxManager ke sini

    private List<GameObject> boxes = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Box") && !boxes.Contains(other.gameObject))
        {
            boxes.Add(other.gameObject);
            Debug.Log($"Box masuk: {boxes.Count}");

            if (boxes.Count >= requiredBoxCount)
            {
                boxManager.TriggerForklift();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Box") && boxes.Contains(other.gameObject))
        {
            boxes.Remove(other.gameObject);
            Debug.Log($"Box keluar: {boxes.Count}");
        }
    }
}
