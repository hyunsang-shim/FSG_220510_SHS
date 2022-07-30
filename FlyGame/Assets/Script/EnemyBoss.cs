using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    float speed = 4;
    int HP = 1000;
    float coolTime = 3f;
    float shotDelay = 0.5f;
    float curDelay = 0;
    float moveDelay = 0;
    bool isCanMove = false;
    Vector2 TopLeft = new Vector2(-5, 8);
    Vector2 BottomRight = new Vector2(5, -6);

    int score = 5000;
    public GameObject[] dieFx;
    bool shotOpen;
    bool isDead = false;
    Rigidbody2D rig;
    Collider2D col;
    Animator anim;
    SpriteRenderer spr;
    Vector3 curPos, nextPos;

    // ���� ����
    public int patternIdx;
    public int curPatternCount;
    public int[] maxPatternCount;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

    }

    private void Start()
    {

        curPos = transform.position;
        nextPos = new Vector2(0, 5);
        Logics.Instance.SetBossHPUI(HP);
        isCanMove = false;
        Move();
    }

    private void FixedUpdate()
    {
    }

    void Move()
    {
        transform.position = Vector2.MoveTowards(curPos, nextPos, speed * Time.fixedDeltaTime);
        curPos = transform.position;

        if (curPos == nextPos)
            isCanMove = true;

        if (isCanMove)
        {
            curPos = transform.position;
            nextPos = new Vector2(
                Random.Range(TopLeft.x, BottomRight.x),
                Random.Range(TopLeft.y, BottomRight.y)
                );

            if (curPos.x >= nextPos.x)
            {
                anim.SetInteger("Direction", -1);
            }
            else if (curPos.x < nextPos.x)
                anim.SetInteger("Direction", 1);
            else
                anim.SetInteger("Direction", 0);

            isCanMove = false;
        }

        if (curPos != nextPos)
            Move();
        else
            Invoke("SelectPattern", coolTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            Hit(collision.gameObject.GetComponent<Bullet>().GetBulletDamage());
            collision.gameObject.SetActive(false);
        }
    }

    private void Hit(int _dmg)
    {
        HP -= _dmg;
        Logics.Instance.UpdateBossHP(HP);
        if(HP <= 500 && !Logics.Instance.GetBossPhase2())
        {
            Logics.Instance.SetBossPhase2();
            AudioManager.Instance.ChangeBGM(2);
            coolTime /= 2f;
            speed *= 1.5f;
        }

        if (HP < 1)
            Die();

        if (HP > 0)
        {
            spr.color = new Color(1, 0.8f, 0.8f, 1);
            Invoke("SetDefaultSpriteColor", 0.35f);
        }

    }

    void SetDefaultSpriteColor()
    {
        if (!isDead)
            spr.color = new Color(1, 1, 1, 1);
    }

    void SelectPattern()
    {
        patternIdx = patternIdx == 3 ? 0 : patternIdx + 1;
        curPatternCount = 0;

        switch(patternIdx)
        {
            case 0:
                Move();
                break;
            case 1:
                FireForward();
                break;
            case 2:
                FireShotgun();
                break;
            case 3:
                FireRound();
                break;
        }
    }

    void FireForward() {
        Debug.Log($"���� 5����");

        curPatternCount++;

        Vector2 targetPos = Logics.Instance.player.transform.position;

        GameObject bullet;
        bullet = Logics.Instance.objPool.GetObject("BossBulletsA");
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            Vector2 BulletSpeed = (targetPos - (Vector2)transform.position).normalized * 3;
            bullet.GetComponent<Bullet>().SetBullet(BulletSpeed, 1, false);
        }



        if (curPatternCount < maxPatternCount[patternIdx])
            Invoke("FireForward", shotDelay);
        else
            Invoke("SelectPattern", coolTime);

    }
    void FireShotgun() {

        Debug.Log($"���� ���� 3����");
        curPatternCount++;

        Vector2 targetPos = Logics.Instance.player.transform.position;

        GameObject bullet1, bullet2, bullet3;
        Vector2 dirVec1 = (targetPos - (Vector2)transform.position).normalized * 3;
        bullet1 = Logics.Instance.objPool.GetObject("bossBulletsB");
        if (bullet1 != null)
        {
            bullet1.transform.position = transform.position;
            bullet1.GetComponent<Bullet>().SetBullet(dirVec1, 1, true);
            Rigidbody2D rig1 = bullet1.GetComponent<Rigidbody2D>();
        }

        Vector2 dirVec2 = (targetPos - (Vector2)transform.position).normalized * 3;
        bullet2 = Logics.Instance.objPool.GetObject("bossBulletsB");

        if (bullet2 != null)
        {
            bullet2.transform.position = transform.position;
            dirVec2 = new Vector2(Mathf.Sin(Mathf.PI * 0.15f) + dirVec2.x, -1).normalized * 3;
            bullet2.GetComponent<Bullet>().SetBullet(dirVec2, 1, true);
            Rigidbody2D rig2 = bullet2.GetComponent<Rigidbody2D>();
        }

        Vector2 dirVec3 = (targetPos - (Vector2)transform.position).normalized * 3;
        bullet3 = Logics.Instance.objPool.GetObject("bossBulletsB");
        if (bullet3 != null)
        {
            bullet3.transform.position = transform.position;
            dirVec3 = new Vector2(Mathf.Sin(Mathf.PI * (-0.15f)) + dirVec3.x, -1).normalized * 3;
            bullet3.GetComponent<Bullet>().SetBullet(dirVec3, 1, true);
            Rigidbody2D rig3 = bullet3.GetComponent<Rigidbody2D>();
        }
        if (curPatternCount < maxPatternCount[patternIdx])
            Invoke("FireShotgun", shotDelay);
        else
            Invoke("SelectPattern", coolTime);
    }
    void FireRound() {
        Debug.Log($"�ݿ����� 3��"); 
        curPatternCount++;
        
        if (curPatternCount < maxPatternCount[patternIdx])
            Invoke("FireRound", shotDelay);
        else
            Invoke("SelectPattern", coolTime);
    }
    void Die()
    {
        Logics.Instance.GameClear();
    }

    void LateUpdate()
    {
        curDelay += Time.fixedDeltaTime;

        if (curDelay >= coolTime)
            shotOpen = true;

        moveDelay += Time.fixedDeltaTime;
        if (moveDelay >= coolTime)
            isCanMove = true;
        
    }
   
}