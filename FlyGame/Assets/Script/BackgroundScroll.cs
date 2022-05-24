using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    MeshRenderer render;
    public float ScrollSpeed;
    float offset;

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        offset = 0;
    }
    private void Update()
    {
        if (!Logics.Instance.GetSlowState())
        {
            offset += ScrollSpeed * Time.deltaTime;
        }
        else
        {
            offset += ScrollSpeed * Time.deltaTime * 0.2f;
        }


        render.material.mainTextureOffset = new Vector2(0, offset);
    }
}
