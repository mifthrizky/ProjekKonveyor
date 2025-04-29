using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private Renderer rend;
    private Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        offset = new Vector2(0, Time.time * scrollSpeed);
        rend.material.mainTextureOffset = offset;
        
    }
}
