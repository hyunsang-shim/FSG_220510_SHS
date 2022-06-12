using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    int dmg;
    Rigidbody2D rig;
    bool isSlowable = true;
    bool isRotate = false;
    Vector2 oldVelocity, newVelocity;


    private void Awake()
    {
        if (rig == null)
            rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Logics.Instance.GetSlowState())
        {
            OnSlowEnabled();
        }
        else
            OnSlowDisabled();

        if(isRotate)
        {
            transform.Rotate(Vector3.forward * 8);
        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BulletKiller")
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
        if (gameObject.name == "EnemyBullet")
        {
            rig.velocity = newVelocity;
        }
    }

    public void OnSlowDisabled()
    {
        rig.velocity = oldVelocity;
    }


    public void SetBullet(Vector2 _speed, bool _rot = false, bool _slowable = false)
    {
        rig.velocity = oldVelocity = _speed;
        newVelocity = oldVelocity / Logics.Instance.GetSlowModifier();

        isRotate = _rot;

        isSlowable = _slowable;
    }

}