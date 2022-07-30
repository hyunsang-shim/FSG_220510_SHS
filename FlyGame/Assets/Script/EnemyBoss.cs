using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    float speed = 4;
    int HP = 1000;
    float coolTime = 3f;
    float curDelay = 0;
    Vector2 TopLeft = new Vector2(-5, 8);
    Vector2 BottomRight = new Vector2(5, -6);

    int score = 5000;
    public GameObject[] dieFx;
    bool shotOpen;
    Rigidbody2D rig;
    Collider2D col;
    Animator anim;
    Vector3 curPos, nextPos;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        curPos = transform.position;
        nextPos = new Vector2(0, 5);

        Logics.Instance.SetBossHPUI(HP);
    }
    private void Update()
    {
        Move();
        Attack();
        Reload();
    }
    private void Start()
    {
    }

    void Move()
    {
        transform.position = Vector2.MoveTowards(curPos, nextPos, speed * Time.deltaTime);
        curPos = transform.position;

        if (shotOpen)
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
        }


        Debug.Log($"Move() false: CurPos = {curPos}, nextPos = {nextPos}");
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
    }

    void Die()
    {
        Logics.Instance.GameClear();
    }

    void Reload()
    {
        curDelay += Time.fixedDeltaTime;

        if (curDelay >= coolTime)
            shotOpen = true;
    }
    void Attack()
    {
        if (!shotOpen) return;

        int shotType = 0;


        Vector2 targetPos = Logics.Instance.player.transform.position;

        switch (shotType)
        {
            case 0:     // 직선 5연사
                {

                    GameObject bullet;
                    bullet = Logics.Instance.objPool.GetObject("BossBulletsA");
                    if (bullet != null)
                    {
                        bullet.transform.position = transform.position;
                        Vector2 BulletSpeed = (targetPos - (Vector2)transform.position).normalized * 3;
                        bullet.GetComponent<Bullet>().SetBullet(BulletSpeed, 1, false);
                    }

                    break;
                }
            case 1:     // 3방향 5연사
                {

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
                    break;
                }
        }

        curDelay = 0;
        shotType = shotType == 0 ? 1 : 0;
    }
}
