using UnityEngine;
using Cinemachine;

public class crate_penampungan : MonoBehaviour
{
    public CinemachineDollyCart dollyCart;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Masuk trigger dengan tag: " + other.tag);

        if (other.CompareTag("Box"))
        {
            Debug.Log(">> Box terdeteksi, forklift jalan!");
            dollyCart.m_Speed = 5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Box"))
        {
            Debug.Log(">> Box keluar, forklift stop.");
            dollyCart.m_Speed = 0f;
        }
    }
}
