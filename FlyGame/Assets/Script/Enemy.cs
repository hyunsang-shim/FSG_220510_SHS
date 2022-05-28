using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int HP;
    public string size;
    public int score;
    GameObject collider;
    public GameObject DieFx;

    Rigidbody2D rig2d;
    Animator animator;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rig2d = GetComponent<Rigidbody2D>();

        if (GetComponent<BoxCollider2D>() == null)
            collider = GetComponent<CircleCollider2D>().gameObject;
        else
            collider = GetComponent<BoxCollider2D>().gameObject;
        
    }

    void OnHit(int dmg)
    {
        HP -= dmg;

        if (HP <= 0)
        {
            Logics.Instance.AddScore(100);
            collider.SetActive(false);
            GameObject fx = Instantiate(DieFx);
            fx.transform.position = transform.position;
            animator.runtimeAnimatorController = DieFx.GetComponent<Animator>().runtimeAnimatorController;
            Invoke("Die", 0.8f);
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

    public void Init()
    {

        rig2d.velocity = Vector2.down * speed;
        HP = Logics.Instance.GetEnemyHP(size);

        GameObject c;
        c = GetComponent<BoxCollider2D>() == null ? GetComponent<CircleCollider2D>().gameObject : GetComponent<BoxCollider2D>().gameObject;
        c.SetActive(true);
    }

}
