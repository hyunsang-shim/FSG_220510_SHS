using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBG : MonoBehaviour
{
    MeshRenderer render;
    public float scrollSpeedY;
    public float scrollSpeedX;
    float offsetX, offsetY;

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        offsetX = 0;
        offsetY = 0;

    }

    private void Update()
    {
        offsetY += scrollSpeedY * Time.deltaTime;
        offsetX += scrollSpeedX * Time.deltaTime;

        render.material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
