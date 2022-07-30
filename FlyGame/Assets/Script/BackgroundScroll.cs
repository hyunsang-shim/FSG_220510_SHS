using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    MeshRenderer render;
    public float scrollSpeedY;
    public float scrollSpeedX;
    float offsetX, offsetY;
    public Texture BossBG;
    public Texture BossBGPhase2;
    bool changedToBossStageBG = false;
    bool changedToBossPhase2 = false;

    private void Start()
    {
        render = GetComponent<MeshRenderer>();
        offsetX = 0;
        offsetY = 0;

    }
    private void Update()
    {
        if (!Logics.Instance.GetSlowState())
        {
            offsetY += scrollSpeedY * Time.deltaTime;
            offsetX += scrollSpeedX * Time.deltaTime;
        }
        else
        {
            offsetY += scrollSpeedY * Time.deltaTime * 0.2f;
            offsetX += scrollSpeedX * Time.deltaTime * 0.2f;
        }


        render.material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }

    private void FixedUpdate()
    {
        if (Logics.Instance.GetBossAppearState() && !changedToBossStageBG)
        {
            render.material.mainTexture = BossBG;
            render.material.mainTextureScale = new Vector2(12,22);
            changedToBossStageBG = true;
            scrollSpeedX = 5;
            scrollSpeedY = -scrollSpeedX;
        }

        if(Logics.Instance.GetBossPhase2() && !changedToBossPhase2)
        {
            render.material.mainTexture = BossBGPhase2;
            render.material.mainTextureScale = new Vector2(6, 11);
            changedToBossPhase2 = true;
            scrollSpeedX = 5;
            scrollSpeedY = -scrollSpeedX;

        }
    }
}
