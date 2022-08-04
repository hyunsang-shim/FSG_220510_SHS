using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    float speed = 4;
    int HP = 100;
    int curHP;
    float coolTime = 3f;
    float shotDelay = 0.5f;
    public float moveDelay = 3;
    Vector2 TopLeft = new Vector2(-5, 8);
    Vector2 BottomRight = new Vector2(5, -6);

    int score = 5000;
    public GameObject dieFx;
    bool isDead = false;
    Rigidbody2D rig;
    Collider2D col;
    Animator anim;
    SpriteRenderer spr;
    Vector3 curPos, nextPos;

    // 패턴 관련
    public int patternIdx;
    public int curPatternCount;
    public int[] maxPatternCount;

    // 사망 이벤트
    AudioSource aud;
    public AudioClip explosionBoss;
    public AudioClip[] explosionSounds;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        curHP = HP;
        aud = GetComponent<AudioSource>();

    }

    private void Start()
    {

        curPos = transform.position;
        nextPos = new Vector2(0, 5);
        curHP = HP = Logics.Instance.GetEnemyHP("Boss");
        Logics.Instance.SetBossHPUI(HP);
        Invoke("SelectPattern", coolTime);
    }

    private void FixedUpdate()
    {
        Move();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            HP = 0;

        }
#endif
    }

    void Move()
    {
        if (!isDead)
        {
            float h = Mathf.Abs(curPos.x - nextPos.x);
            float v = Mathf.Abs(curPos.y - nextPos.y);
            float movespeed;
            if (h > v)
                movespeed = speed + h / 2f;
            else
                movespeed = speed + v / 2f;
            if(!Logics.Instance.GetSlowState())
                transform.position = Vector2.MoveTowards(curPos, nextPos, movespeed * Time.fixedDeltaTime);
            else
                transform.position = Vector2.MoveTowards(curPos, nextPos, Logics.Instance.GetSlowedSpeed() * 0.5f * Time.fixedDeltaTime);
            curPos = transform.position;

            if (curPos == nextPos) Invoke("SelectNextMovePosition", moveDelay);

            if (curPos.x > nextPos.x) anim.SetInteger("Direction", -1);
            else if (curPos.x < nextPos.x) anim.SetInteger("Direction", 1);
            else anim.SetInteger("Direction", 0);
        }
    }

    void SelectNextMovePosition()
    {
        int cnt = 0;
        while (Mathf.Abs(nextPos.x - curPos.x) < 2 && Mathf.Abs(nextPos.y - curPos.y) < 2)
        {
            nextPos = new Vector2(
                   Random.Range(TopLeft.x, BottomRight.x),
                   Random.Range(TopLeft.y, BottomRight.y)
                   );

            cnt++;

            if (cnt > 100) break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            if (!isDead)
            {
                Hit(collision.gameObject.GetComponent<Bullet>().GetBulletDamage());
                collision.gameObject.SetActive(false);
            }
        }
    }

    private void Hit(int _dmg)
    {
        curHP -= _dmg;
        Logics.Instance.UpdateBossHP(curHP);
        if (curHP <= HP / 2 && !Logics.Instance.GetBossPhase2())
        {
            Logics.Instance.SetBossPhase2();
            SetBossPhase2();
        }

        if (curHP < 1)
            Die();

        if (curHP > 0)
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
        if (!isDead)
        {
            patternIdx = patternIdx == 3 ? 1 : patternIdx + 1;
            curPatternCount = 0;

            switch (patternIdx)
            {
                case 1:
                    FireForward();
                    break;
                case 2:
                    FireShotgun();
                    break;
                case 3:
                    {
                        if (!Logics.Instance.GetBossPhase2())
                        {
                            SelectPattern();
                        }
                        else
                            FireRound();
                    }
                    break;
            }
        }
        else
            anim.StopPlayback();
    }

    void FireForward()
    {

        Vector2 targetPos = Logics.Instance.player.transform.position;

        GameObject bulletLL = Logics.Instance.objPool.GetObject("bossBulletsA");
        bulletLL.transform.position = transform.position + Vector3.right * 0.15f;
        GameObject bulletL = Logics.Instance.objPool.GetObject("bossBulletsA");
        bulletL.transform.position = transform.position + Vector3.right * 0.4f;
        GameObject bulletR = Logics.Instance.objPool.GetObject("bossBulletsA");
        bulletR.transform.position = transform.position + Vector3.left * 0.15f;
        GameObject bulletRR = Logics.Instance.objPool.GetObject("bossBulletsA");
        bulletRR.transform.position = transform.position + Vector3.left * 0.4f;

        Vector2 BulletSpeed = (targetPos - (Vector2)transform.position).normalized * (speed * 2f);
        bulletLL.GetComponent<Bullet>().SetBullet(BulletSpeed, 1, false, true);
        bulletL.GetComponent<Bullet>().SetBullet(BulletSpeed, 1, false, true);
        bulletR.GetComponent<Bullet>().SetBullet(BulletSpeed, 1, false, true);
        bulletRR.GetComponent<Bullet>().SetBullet(BulletSpeed, 1, false, true);


        curPatternCount++;

        if (!Logics.Instance.GetLogicTimeFlag())
        {
            if (curPatternCount < (Logics.Instance.GetBossPhase2() ? maxPatternCount[patternIdx] * 2 : maxPatternCount[patternIdx]))
            {
                if (!isDead)
                {
                    if (!Logics.Instance.GetSlowState())
                    {
                        Invoke("FireForward", shotDelay);
                    }
                    else
                        Invoke("FireForward", shotDelay + Logics.Instance.GetSlowedSpeed());
                }
                else
                {
                    if (!Logics.Instance.GetSlowState())
                    {
                        SelectPattern();
                    }
                    else
                        Invoke("SelectPattern", Logics.Instance.GetSlowedSpeed());
                }
            }
            else
                Invoke("SelectPattern", coolTime);
        }
    }
    void FireShotgun()
    {

        curPatternCount++;

        if (curPatternCount <= maxPatternCount[patternIdx])
        {
            Vector2 targetPos = Logics.Instance.player.transform.position;

            GameObject bullet1, bullet2, bullet3;
            bullet1 = Logics.Instance.objPool.GetObject("bossBulletsB"); 
            bullet2 = Logics.Instance.objPool.GetObject("bossBulletsB");
            bullet3 = Logics.Instance.objPool.GetObject("bossBulletsB");

            Vector2 dirVec1 = (targetPos - (Vector2)transform.position).normalized * speed;
            if (bullet1 != null)
            {
                bullet1.transform.position = transform.position;
                bullet1.GetComponent<Bullet>().SetBullet(dirVec1.normalized * speed, 1, true, true);
            }

            Vector2 dirVec2 = (targetPos - (Vector2)transform.position).normalized * speed;

            if (bullet2 != null)
            {
                bullet2.transform.position = transform.position;
                dirVec2 = new Vector2(Mathf.Sin(Mathf.PI * 0.15f) + dirVec2.x, -1).normalized * speed;
                bullet2.GetComponent<Bullet>().SetBullet(dirVec2.normalized * speed, 1, true, true);
            }

            Vector2 dirVec3 = (targetPos - (Vector2)transform.position).normalized * speed;
            if (bullet3 != null)
            {
                bullet3.transform.position = transform.position;
                dirVec3 = new Vector2(Mathf.Sin(Mathf.PI * (-0.15f)) + dirVec3.x, -1).normalized * speed;
                bullet3.GetComponent<Bullet>().SetBullet(dirVec3.normalized * speed, 1, true, true);
            }
        }

        if (!Logics.Instance.GetLogicTimeFlag())
        {
            if (curPatternCount < (Logics.Instance.GetBossPhase2() ? maxPatternCount[patternIdx] * 2 : maxPatternCount[patternIdx]))
            {
                if (!isDead)
                {
                    if (!Logics.Instance.GetSlowState())
                    {
                        Invoke("FireShotgun", shotDelay);
                    }
                    else
                        Invoke("FireShotgun", shotDelay + Logics.Instance.GetSlowedSpeed());
                }
                else
                {
                    if (!Logics.Instance.GetSlowState())
                    {
                        SelectPattern();
                    }
                    else
                        Invoke("SelectPattern", Logics.Instance.GetSlowedSpeed());
                }
            }
            else
                Invoke("SelectPattern", coolTime);
        }

}
    void FireRound()
    {
        
        for (int i = 0; i < 6; i++)
        {
            GameObject bullet = Logics.Instance.objPool.GetObject("bossBulletsD");
            bullet.transform.position = transform.position;

            Vector2 dirVec = Logics.Instance.player.transform.position - transform.position;
            Vector2 randVec = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(0f, 2.5f));
            dirVec += randVec;
            bullet.GetComponent<Bullet>().SetBullet(dirVec.normalized * speed, 1, true);
        }

        curPatternCount++;

        if (!Logics.Instance.GetLogicTimeFlag())
        {
            if (curPatternCount < (Logics.Instance.GetBossPhase2() ? maxPatternCount[patternIdx] * 2 : maxPatternCount[patternIdx]))
            {
                if (!isDead)
                {
                    if (!Logics.Instance.GetSlowState())
                    {
                        Invoke("FireRound", shotDelay);
                    }
                    else
                        Invoke("FireRound", shotDelay + Logics.Instance.GetSlowedSpeed());
                }
                else
                {
                    if (!Logics.Instance.GetSlowState())
                    {
                        SelectPattern();
                    }
                    else
                        Invoke("SelectPattern", Logics.Instance.GetSlowedSpeed());
                }
            }
            else
                Invoke("SelectPattern", coolTime);
        }
    }
    void Die()
    {
        isDead = true;        
        Logics.Instance.AddScore(score);
        GameObject fx = Instantiate(dieFx);
        fx.transform.SetParent(transform);
        fx.name = "dieFx";
        StartCoroutine("DieSoundPlay");
        AudioManager.Instance.PlaySFX(explosionBoss);
        Logics.Instance.ClearAllEnemyBullets();
        Logics.Instance.StopLogicTime();
    }

    public void SetBossPhase2()
    {
        AudioManager.Instance.ChangeBGM(2);
        coolTime = coolTime * 0.75f;
        speed = speed * 1.35f;
        shotDelay = shotDelay * 0.4f;

        for (int i = 0; i < maxPatternCount.Length; i++)
        {
            maxPatternCount[i] = (int)(maxPatternCount[i] * 1.5);
        }


    }

    IEnumerator DieSoundPlay()
    {
        int flip = 0;
        int cnt = 0;
        while (cnt < 5)
        {
            aud.PlayOneShot(explosionSounds[flip == 0 ? 1 : 0]);
            cnt++;
            
            yield return new WaitForSecondsRealtime(0.8f);

            if(cnt == 3)
            {
                GameObject fx2 = Instantiate(dieFx);
                fx2.transform.SetParent(transform);
                fx2.name = "dieFx2";
            }
        }


        GameObject fx3 = Instantiate(dieFx);
        fx3.transform.SetParent(transform);
        fx3.name = "dieFx3";

        Logics.Instance.GameClear();
        Debug.Log("Game Clear!");
        gameObject.SetActive(false);
    }
}
