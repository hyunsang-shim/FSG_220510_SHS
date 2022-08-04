using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int dmg;
    Rigidbody2D rig;
    bool isSlowable = true;
    bool isRotate = false;
    Vector2 normalVelocity, slowedVelocity;


    private void Awake()
    {
        if (rig == null)
            rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Logics.Instance.GetSlowState())
            OnSlowEnabled();
        else
            OnSlowDisabled();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BulletKiller"))
        {
            gameObject.SetActive(false);
        }
    }

    public void SetBulletDamage(int d)
    {
        dmg = d;
    }

    public int GetBulletDamage()
    {
        return dmg;
    }

    public void OnSlowEnabled()
    {
        if(isSlowable)
            rig.velocity = slowedVelocity;
        else
            rig.velocity = normalVelocity;


        if(!Logics.Instance.GetLogicTimeFlag())
            if (isRotate)
                transform.Rotate(Vector3.forward * 2);

    }

    public void OnSlowDisabled()
    {
        rig.velocity = normalVelocity;

        if (!Logics.Instance.GetLogicTimeFlag()) 
            if (isRotate)
                transform.Rotate(Vector3.forward * 12);

    }


    public void SetBullet(Vector2 _speed, int _dmg, bool _rot = false, bool _slowable = false)
    {
        rig.velocity = normalVelocity = _speed;
        slowedVelocity = normalVelocity / Logics.Instance.GetSlowedSpeed() * 0.5f;

        isRotate = _rot;

        isSlowable = _slowable;

        dmg = _dmg;
    }

}