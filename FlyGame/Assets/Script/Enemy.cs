using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    float speed;
    int HP;
    
    public int score;
    public GameObject DieFx;
    GameObject bullets;
    int movePatternID;
    int positionID = 1;

    Rigidbody2D rig2d;
    SpriteRenderer spriteRenderer;
    bool isDead = false;
    string size;
    string MyShotType;
    List<Transform> MovePoints = new List<Transform>();
    Vector3 curPos, nextPos;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rig2d = GetComponent<Rigidbody2D>();
        movePatternID = 0;
        curPos = nextPos = transform.position;

    }

    void OnHit(int dmg)
    {
        HP -= dmg;

        if (HP <= 0)
        {
            Logics.Instance.EnemyDead(gameObject, isDead, score);
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

    private void Update()
    {
        if ((Mathf.Abs(curPos.x - nextPos.x) <= 0.01f) && (Mathf.Abs(curPos.y - nextPos.y) <= 0.01f))
        {
            if (MovePoints.Count > positionID)
            {
                nextPos = MovePoints[positionID++].position;
            }
        }
    }


    private void FixedUpdate()
    {
        MoveEnemy();
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

    public void Init(string _size, string _shotType, int _movePattern, float _speed)
    {

        size = _size;
        HP = Logics.Instance.GetEnemyHP(size);
        MyShotType = _shotType;
        speed = _speed;

        MovePoints = new List<Transform>();

        GameObject c;
        c = GetComponent<BoxCollider2D>() == null ? GetComponent<CircleCollider2D>().gameObject : GetComponent<BoxCollider2D>().gameObject;
        c.SetActive(true);
        isDead = false;
        spriteRenderer.color = new Color(1, 1, 1, 1);
        movePatternID = _movePattern;
        MovePoints = Logics.Instance.GetEnemyMovePoints(movePatternID);
        transform.position = MovePoints[0].position;
        nextPos = MovePoints[1].position;
        positionID = 1;

        Invoke("FireBullet", 0.4f);


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


    private void MoveEnemy()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.fixedDeltaTime);
        curPos = transform.position;
    }
}
