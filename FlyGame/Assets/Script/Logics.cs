using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logics : MonoBehaviour
{
    public int direction;

    public GameObject player;
    public Animator animator;
    public Animation anim;

    public float baseSpeed;   // 기본 기체 이동 속도
    float slowModifyer = 0.5f;      // 느려졌을 때 얼마나 느려질 것인지.
    bool isSlowed = false;          // 느려진 상태인지 확인


    private static Logics instance = null;
    public static Logics Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }


    private void FixedUpdate()
    {
        SetAnimationDir();
    }


    public void SetDirection(int d)
    {
        direction = d;
    }
    public void SetAnimationDir()
    {
        switch (direction)
        {            
            case 2:
            case 3:
            case 6:
                {
                    animator.SetInteger("Direction", 2);
                    break;
                }
            case 8:
            case 9:
            case 12:
                {
                    animator.SetInteger("Direction", 1);
                    break;
                }
            default:
                {
                    animator.SetInteger("Direction", 0);
                    break;
                }
        }
        
    }

    public void SetSlowState(bool v)
    {
        isSlowed = v;
    }

    public bool GetSlowState()
    {
        return isSlowed;
    }

    public float GetSpeed()
    {
        return baseSpeed;
    }

    public float GetSlowedSpeed()
    {
        return baseSpeed * slowModifyer;
    }
}
