using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int HP;
    
    public int score;
    public GameObject DieFx;

    Rigidbody2D rig2d;
    SpriteRenderer spriteRenderer;
    bool isDead = false;
    string size;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rig2d = GetComponent<Rigidbody2D>();

    }

    void OnHit(int dmg)
    {
        HP -= dmg;

        if (HP <= 0)
        {
            Logics.Instance.EnemyDead(gameObject, isDead);
            isDead = true;
        }
        else if( HP > 0)
        {
            spriteRenderer.color = new Color(1, 0.8f, 0.8f, 1);
            Invoke("SetDefaultSpriteColor", 0.25f);
        }
        
        if (isDead)
        {
            
        }
    }

    void SetDefaultSpriteColor()
    {
        if(!isDead)
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

    public string GetSize()
    {
        return size;
    }

    public void SetSize(string s)
    {
        size = s;
    }

    public void Init(string _size)
    {

        rig2d.velocity = Vector2.down * speed;
        HP = Logics.Instance.GetEnemyHP(size);
        size = _size;
        GameObject c;
        c = GetComponent<BoxCollider2D>() == null ? GetComponent<CircleCollider2D>().gameObject : GetComponent<BoxCollider2D>().gameObject;
        c.SetActive(true);
        isDead = false;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

}
