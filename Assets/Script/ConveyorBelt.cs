using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed;
    public Vector3 direction;
    public List<GameObject> onBelt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = onBelt.Count - 1; i >= 0; i--) // Loop mundur agar aman saat remove
        {
            if (onBelt[i] == null) // Jika object sudah dihancurkan, hapus dari list
            {
                onBelt.RemoveAt(i);
                continue;
            }

            Rigidbody rb = onBelt[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = speed * direction;
            }
        }
    }


    //When something collider with the belt
    private void OnCollisionEnter(Collision collision)
    {
        onBelt.Add(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        onBelt.Remove(collision.gameObject);
    }
}
