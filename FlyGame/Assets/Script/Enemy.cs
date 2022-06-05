using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int HP;
    
    public int score;
    public GameObject DieFx;
    GameObject bullets;

    Rigidbody2D rig2d;
    SpriteRenderer spriteRenderer;
    bool isDead = false;
    string size;
    string MyShotType;

    int movePoints;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rig2d = GetComponent<Rigidbody2D>();
        movePoints = 0;
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

    public void Init(string _size, string _shotType, int points)
    {

       
        HP = Logics.Instance.GetEnemyHP(size);
        size = _size;
        MyShotType = _shotType;
        GameObject c;
        c = GetComponent<BoxCollider2D>() == null ? GetComponent<CircleCollider2D>().gameObject : GetComponent<BoxCollider2D>().gameObject;
        c.SetActive(true);
        isDead = false;
        spriteRenderer.color = new Color(1, 1, 1, 1);


        movePoints = points;
        if (movePoints == 0)
            rig2d.velocity = Vector2.down * speed;
        else
            StartCoroutine("MoveToPoints",points);

        Invoke("FireBullet", 0.4f);


    }

    IEnumerable MoveToPoints(List<Vector3> points)
    {
        int pointIdx = 0;
        while (points[pointIdx] != null)
        {
            Vector3.Slerp(transform.position, points[pointIdx++], speed);

        yield return new WaitForSeconds(0.2f);
        }
    }


    public void FireBullet()
    {
        switch (MyShotType)
        {
            case "OneShotToTarget":
                {
                    Vector3 targetPos = Logics.Instance.player.transform.position;
                    GameObject bullet = Logics.Instance.objPool.GetObject("enemyBulletsA");
                    bullet.transform.position = transform.position;
                    bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    Rigidbody2D rigidLv_1 = bullet.GetComponent<Rigidbody2D>();

                    rigidLv_1.AddForce((Vector2)(targetPos - transform.position).normalized * 3, ForceMode2D.Impulse);
                    break;
                }
        }
    }
}
