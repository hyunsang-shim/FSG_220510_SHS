using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int HP;
    public string size;
    
    Rigidbody2D rig2d;
    Animator animator;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rig2d = GetComponent<Rigidbody2D>();
        rig2d.velocity = Vector2.down * speed;
        HP = Logics.Instance.GetEnemyHP(size);
    }

    void OnHit(int dmg)
    {
        HP -= dmg;

        if (HP <= 0)
        {
            animator.SetBool("isDead", true);
            Invoke("Die", 0.5f);
        }
        else
        {
            spriteRenderer.color = new Color(1, 0.8f, 0.8f, 1);
            Invoke("SetDefaultSpriteColor", 0.15f);
        }

    }

    void SetDefaultSpriteColor()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BulletKiller")
        {
            gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "PlayerBullet")
        {            
            OnHit(collision.gameObject.GetComponent<Bullet>().GetBulletDamage());
            collision.gameObject.SetActive(false);

        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }

}
