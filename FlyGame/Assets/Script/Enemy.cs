using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    float speed;
    int HP;
    bool valunable;
    public int score;
    public GameObject DieFx;
    int movePatternID;
    int positionID = 1;
    float curShotDelay, maxShotDelay;
    bool shotOpen = false;
    public Slider hpbar;


    Rigidbody2D rig2d;
    SpriteRenderer spriteRenderer;
    bool isDead = false;
    string size;
    string MyShotType;
    List<Transform> MovePoints = new List<Transform>();
    Vector3 curPos, nextPos;
    Vector3 hpbarOffset;
    string dropType;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rig2d = GetComponent<Rigidbody2D>();
        movePatternID = 0;
        curPos = nextPos = transform.position;
        hpbar = GetComponentInChildren<Slider>();
        hpbar.maxValue = HP;
        hpbarOffset = new Vector3(0, -0.5f, 0);
        
    }

    private void Start()
    {
        hpbar.maxValue = HP;
        hpbar.value = HP;
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

        FireBullet();
        Reload();

        //hpbar.transform.parent.position = transform.position + hpbarOffset;
    }
    private void OnEnable()
    {
        Invoke("SetValunable", 2);
    }

    void SetValunable()
    {
        valunable = true;
    }
    private void FixedUpdate()
    {
        MoveEnemy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("BulletKiller"))
        {
            gameObject.SetActive(false);
        }
        else if(collision.gameObject.CompareTag("PlayerBullet"))
        {            
            OnHit(collision.gameObject.GetComponent<Bullet>().GetBulletDamage());
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.CompareTag("Border"))
        {
            shotOpen = !shotOpen;;
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

    public void Init(string _size, string _shotType, int _movePattern, float _speed, string _dropType = "None", float _shotDelay = 9999)
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
        dropType = _dropType;

        curShotDelay  = 0;
        maxShotDelay = _shotDelay;

        hpbar.maxValue = HP;
        hpbar.value = HP;

        shotOpen = false;

        InvokeRepeating("FireBullet", 2, maxShotDelay);

    }

    public void FireBullet()
    {

        if (!shotOpen || curShotDelay < maxShotDelay) return;

        Vector2 targetPos = Logics.Instance.player.transform.position;

        switch (MyShotType)
        {
            case "OneShotToTarget":
                {
                    GameObject bullet;
                    switch (size)
                    {
                        case "Small":
                            bullet = Logics.Instance.objPool.GetObject("enemyBulletsA");
                            break;
                        case "Medium":
                            bullet = Logics.Instance.objPool.GetObject("enemyBulletsB");
                            break;
                        default:
                            bullet = Logics.Instance.objPool.GetObject("enemyBulletsA");
                            break;
                    }

                    if (bullet != null)
                    {
                        bullet.transform.position = transform.position;
                        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                        Rigidbody2D rigidLv_1 = bullet.GetComponent<Rigidbody2D>();

                        Vector2 BulletSpeed = (targetPos - (Vector2)transform.position).normalized * 3;
                        rigidLv_1.AddForce(BulletSpeed, ForceMode2D.Impulse);
                        bullet.GetComponent<Bullet>().SetBullet(BulletSpeed, 1, false);                     
                    }
                    break;
                }
            case "3-Way":
                {

                    GameObject bullet1, bullet2, bullet3;
                    Vector2 dirVec1 = (targetPos - (Vector2)transform.position).normalized * 3;
                    bullet1 = Logics.Instance.objPool.GetObject("bossBulletsD");
                    if (bullet1 != null)
                    {
                        bullet1.transform.position = transform.position;
                        bullet1.GetComponent<Bullet>().SetBullet(dirVec1, 1, true);
                        Rigidbody2D rig1 = bullet1.GetComponent<Rigidbody2D>();
                    }

                    Vector2 dirVec2 = (targetPos - (Vector2)transform.position).normalized * 3;
                    bullet2 = Logics.Instance.objPool.GetObject("bossBulletsD");

                    if (bullet2 != null)
                    {
                        bullet2.transform.position = transform.position;
                        dirVec2 = new Vector2(Mathf.Sin(Mathf.PI * 0.15f) + dirVec2.x, -1).normalized * 3;
                        bullet2.GetComponent<Bullet>().SetBullet(dirVec2, 1, true);
                        Rigidbody2D rig2 = bullet2.GetComponent<Rigidbody2D>();
                    }

                    Vector2 dirVec3 = (targetPos - (Vector2)transform.position).normalized * 3;
                    bullet3 = Logics.Instance.objPool.GetObject("bossBulletsD");
                    if (bullet3 != null)
                    {
                        bullet3.transform.position = transform.position;
                        dirVec3 = new Vector2(Mathf.Sin(Mathf.PI * (-0.15f)) + dirVec3.x, -1).normalized * 3;
                        bullet3.GetComponent<Bullet>().SetBullet(dirVec3, 1, true);
                        Rigidbody2D rig3 = bullet3.GetComponent<Rigidbody2D>();
                    }
                    break; 
                }
        }

            curShotDelay = 0;
    }

    public void Reload()
    {
        if (maxShotDelay != 9999 || shotOpen)
            curShotDelay += Time.fixedDeltaTime;
    }

    private void MoveEnemy()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.fixedDeltaTime);
        curPos = transform.position;
    }

    void OnHit(int dmg)
    {
        if(valunable)
            HP -= dmg;

        if (HP <= 0)
        {
            Logics.Instance.EnemyDead(gameObject, isDead, score);
            isDead = true;
        }
        else if (HP > 0)
        {
            spriteRenderer.color = new Color(1, 0.8f, 0.8f, 1);
            hpbar.value = HP;
            Invoke("SetDefaultSpriteColor", 0.25f);
        }

        if (isDead)
        {
            if (dropType != "None")
            {
                Logics.Instance.DropItem(transform.position, dropType);
            }
        }
    }

    void SetDefaultSpriteColor()
    {
        if (!isDead)
            spriteRenderer.color = new Color(1, 1, 1, 1);
    }

}
