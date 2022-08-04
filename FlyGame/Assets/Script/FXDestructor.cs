using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXDestructor : MonoBehaviour
{
    public float destructionDelay;
    public float curTimerCount;
    Animator animator;

    private void Start()
    {
        curTimerCount = 0;
        transform.position = transform.parent.transform.position;
    }

    private void Update()
    {
        curTimerCount += Time.deltaTime;
        if (curTimerCount >= destructionDelay)
            DestructSelf();
    }

    void DestructSelf()
    {
        gameObject.SetActive(false);
        curTimerCount = 0;
    }

}
