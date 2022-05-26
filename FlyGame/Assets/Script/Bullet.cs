using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    int dmg;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BulletKiller")
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
}
