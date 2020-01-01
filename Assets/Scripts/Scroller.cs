using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    public bool isEnabled = true;
    public float speed;

    private Renderer rend;
    private float offset = 0;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (isEnabled)
        {
            offset += speed * Time.deltaTime;
            rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }
}
