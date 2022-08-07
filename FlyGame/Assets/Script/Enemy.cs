using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    float speed;
    float bulletSpeed;
    int HP;
    bool valunable;
    public int score;
    public GameObject DieFx;
    Collider2D col;
    int movePatternID;
    int positionID = 1;
    float maxShotDelay;
    float curDelay;    
    bool shotOpen = false;
    public Slider hpbar;
    float invalunableDelay = 1.8f;


    Rigidbody2D rig2d;
    SpriteRenderer spriteRenderer;
    bool isDead = false;
    string size;
    string MyShotType;
    List<Transform> MovePoints = new List<Transform>();
    Vector3 curPos, nextPos;
    string dropType;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rig2d = GetComponent<Rigidbody2D>();
        movePatternID = 0;
        curPos = nextPos = transform.position;
        hpbar = GetComponentInChildren<Slider>();
        hpbar.maxValue = HP;
        col = GetComponent<Collider2D>();        
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

        if(valunable)
            FireBullet();

    }
    private void OnEnable()
    {
        Invoke("SetValunable", invalunableDelay);
        curDelay = 0;
        SetDefaultSpriteColor();


    }

    void SetValunable()
    {
        valunable = true;
    }
    private void FixedUpdate()
    {
        MoveEnemy();

        if(!Logics.Instance.GetLogicTimeFlag())
            curDelay += Time.fixedDeltaTime;

        FireBullet();
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

    public void Init(string _size, string _shotType, int _movePattern, float _speed, float _bulletSpeed, string _dropType = "None", float _shotDelay = 9999)
    {

        size = _size;
        HP = Logics.Instance.GetEnemyHP(size);
        MyShotType = _shotType;
        speed = _speed;
        bulletSpeed = _bulletSpeed;

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

        maxShotDelay = _shotDelay;

        hpbar.maxValue = HP;
        hpbar.value = HP;

        shotOpen = false;

        Invoke("FireBullet", maxShotDelay + 2);

    }

    public void FireBullet()
    {

        Vector2 targetPos = Logics.Instance.player.transform.position;

        if (curDelay < maxShotDelay) return;


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
                        Vector2 BulletSpeed = (targetPos - (Vector2)transform.position).normalized * 3;
                        bullet.GetComponent<Bullet>().SetBullet(BulletSpeed, 1, false, true);                     
                    }
                    break;
                }
            case "3-Way":
                {

                    GameObject bullet1, bullet2, bullet3;
                    bullet1 = Logics.Instance.objPool.GetObject("bossBulletsD");
                    bullet2 = Logics.Instance.objPool.GetObject("bossBulletsD");
                    bullet3 = Logics.Instance.objPool.GetObject("bossBulletsD");

                    bullet1.transform.position = transform.position;
                    bullet2.transform.position = transform.position;
                    bullet3.transform.position = transform.position;

                    Vector2 dirVec1 = (targetPos - (Vector2)transform.position).normalized * 3;
                    Vector2 dirVec2 = (targetPos - (Vector2)transform.position).normalized * 3;
                    Vector2 dirVec3 = (targetPos - (Vector2)transform.position).normalized * 3;
                    dirVec2 = new Vector2(Mathf.Sin(Mathf.PI * 0.2f) + dirVec2.x, -1).normalized * 3;
                    dirVec3 = new Vector2(Mathf.Sin(Mathf.PI * (-0.2f)) + dirVec3.x, -1).normalized * 3;
                    bullet1.GetComponent<Bullet>().SetBullet(dirVec1, 1, true, true);
                    bullet2.GetComponent<Bullet>().SetBullet(dirVec2, 1, true, true);
                    bullet3.GetComponent<Bullet>().SetBullet(dirVec3, 1, true, true);

                    break;
                }
        }

        curDelay = 0;
    }


    private void MoveEnemy()
    {
        if (Logics.Instance.GetLogicTimeFlag())
            return;

        if(!Logics.Instance.GetSlowState())
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.fixedDeltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, nextPos, Logics.Instance.GetSlowedSpeed() * 0.5f * Time.fixedDeltaTime);

        curPos = transform.position;
    }


    public void OnHit(int dmg)
    {
        if(valunable)
            HP -= dmg;

        if (HP <= 0)
        {
            Logics.Instance.EnemyDead(gameObject, isDead, score);
            isDead = true;
            valunable = false;
        }
        else if (HP > 0)
        {
            spriteRenderer.color = new Color(1, 0.8f, 0.8f, 1);
            hpbar.value = HP;
            Invoke("SetDefaultSpriteColor", 0.25f);
        }

        if (isDead)
        {
            valunable = false;
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
