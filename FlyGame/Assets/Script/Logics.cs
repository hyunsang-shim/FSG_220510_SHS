using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logics : MonoBehaviour
{

    public ObjPool objPool;
    public int direction;

    public GameObject player;
    public Animator animator;
    public Animation anim;

    public float baseSpeed;   // 기본 기체 이동 속도    
    public float playerMove_H, playerMove_V;
    
    public float maxShotDelay;  // 설정된 총알 발사 딜레이
    public int BulletPower;
    public Vector3 playerShotPoint;

    public int defaultEnemyHP_Small;
    public int defaultEnemyHP_Medium;
    public int defaultEnemyHP_Boss;


    float slowModifyer = 0.5f;      // 느려졌을 때 얼마나 느려질 것인지.
    bool isSlowed = false;          // 느려진 상태인지 확인
    float totalSpeed;
    float curShotDelay;     // 총알 발사 딜레이 체크용


    private static Logics instance = null;
    public static Logics Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null) instance = this;

        playerShotPoint = player.transform.position + Vector3.up * 0.5f;
        BulletPower = 1;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            ++BulletPower;

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            --BulletPower;

        if (Input.GetKeyDown(KeyCode.KeypadDivide))
            maxShotDelay += 0.1f;

        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            maxShotDelay -= 0.1f;

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            GameObject tempEnemy = objPool.GetObject("enemySmall");
            tempEnemy.transform.position = new Vector3(0, 0, 0);
        }


#endif

            Fire();
        Reload();
        UpdatePlayerShotPoint();
    }

    private void FixedUpdate()
    {
        SetAnimationDir();
        if (isSlowed)
            player.transform.position += new Vector3(playerMove_H, playerMove_V, 0) * baseSpeed * Time.deltaTime;
        else
            player.transform.position += new Vector3(playerMove_H, playerMove_V, 0) * baseSpeed * Time.deltaTime;

    }


    public void SetDirection(int d)
    {
        direction = d;
    }
    public void SetAnimationDir()
    {
        switch (direction)
        {            
            case 2:
            case 3:
            case 6:
                {
                    animator.SetInteger("Direction", 2);
                    break;
                }
            case 8:
            case 9:
            case 12:
                {
                    animator.SetInteger("Direction", 1);
                    break;
                }
            default:
                {
                    animator.SetInteger("Direction", 0);
                    break;
                }
        }
        
    }

    public void SetSlowState(bool v)
    {
        isSlowed = v;
    }

    public bool GetSlowState()
    {
        return isSlowed;
    }

    public float GetSpeed()
    {
        return baseSpeed;
    }

    public float GetSlowedSpeed()
    {
        return baseSpeed * slowModifyer;
    }

    public void SetPlayerMovement(float h, float v)
    {
        playerMove_H = h;
        playerMove_V = v;
    }


    void Fire()
    {
        if (!Input.GetButton("Fire1") && !Input.GetKey(KeyCode.Space))
            return;

        if (curShotDelay < maxShotDelay)
            return;

        // 플레이어 총알 패턴
        // Power에 따라 총알 변경
        switch (BulletPower)
        {
            case 1:
                GameObject bulletLv_1 = objPool.GetObject("playerBulletsA");
                bulletLv_1.transform.position = playerShotPoint;
                Rigidbody2D rigidLv_1 = bulletLv_1.GetComponent<Rigidbody2D>();
                rigidLv_1.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                bulletLv_1.GetComponent<Bullet>().SetBulletDamage(BulletPower * 10);

                break;
            case 2:
                GameObject bulletLLv_2 = objPool.GetObject("playerBulletsA");
                GameObject bulletRLv_2 = objPool.GetObject("playerBulletsA");
                bulletLLv_2.transform.position = playerShotPoint + Vector3.left * 0.1f;
                bulletRLv_2.transform.position = playerShotPoint + Vector3.right * 0.1f;
                Rigidbody2D rigidLLv_2 = bulletLLv_2.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidRLv_2 = bulletRLv_2.GetComponent<Rigidbody2D>();
                rigidLLv_2.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRLv_2.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                bulletLLv_2.GetComponent<Bullet>().SetBulletDamage(BulletPower * 10);
                bulletRLv_2.GetComponent<Bullet>().SetBulletDamage(BulletPower * 10);

                break;
            case 3:
                GameObject bulletLv_3 = objPool.GetObject("playerBulletsB");
                bulletLv_3.transform.position = playerShotPoint;
                Rigidbody2D rigidLv_3 = bulletLv_3.GetComponent<Rigidbody2D>();
                rigidLv_3.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                bulletLv_3.GetComponent<Bullet>().SetBulletDamage(BulletPower * 15);

                break;
            case 4:
                GameObject bulletLv_4 = objPool.GetObject("playerBulletsB");
                GameObject bulletLLv_4 = objPool.GetObject("playerBulletsA");
                GameObject bulletRLv_4 = objPool.GetObject("playerBulletsA");
                bulletLv_4.transform.position = playerShotPoint;
                bulletLLv_4.transform.position = playerShotPoint + Vector3.left * 0.15f;
                bulletRLv_4.transform.position = playerShotPoint + Vector3.right * 0.15f;
                Rigidbody2D rigidLv_4 = bulletLv_4.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLLv_4 = bulletLLv_4.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidRLv_4 = bulletRLv_4.GetComponent<Rigidbody2D>();
                rigidLv_4.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLLv_4.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRLv_4.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                bulletLv_4.GetComponent<Bullet>().SetBulletDamage(BulletPower * 15);
                bulletLLv_4.GetComponent<Bullet>().SetBulletDamage(BulletPower * 10);
                bulletRLv_4.GetComponent<Bullet>().SetBulletDamage(BulletPower * 10);
                break;
            case 5:
                GameObject bulletLLv_5 = objPool.GetObject("playerBulletsB");
                GameObject bulletRLv_5 = objPool.GetObject("playerBulletsB");
                bulletLLv_5.transform.position = playerShotPoint + Vector3.left * 0.15f;
                bulletRLv_5.transform.position = playerShotPoint + Vector3.right * 0.15f;
                Rigidbody2D rigidLLv_5 = bulletLLv_5.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidRLv_5 = bulletRLv_5.GetComponent<Rigidbody2D>();
                rigidLLv_5.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidRLv_5.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                bulletLLv_5.GetComponent<Bullet>().SetBulletDamage(BulletPower * 15);
                bulletRLv_5.GetComponent<Bullet>().SetBulletDamage(BulletPower * 15);
                break;
        }

        curShotDelay = 0;
        // 총알 발사

    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void UpdatePlayerShotPoint()
    {
        playerShotPoint = player.transform.position + Vector3.up * 0.5f;
    }

    public int GetEnemyHP(string size)
    {
        switch(size)
        {
            case "Small":
                return defaultEnemyHP_Small;
            case "Medium":
                return defaultEnemyHP_Medium;
            case "Boss":
                return defaultEnemyHP_Boss;
        }

        return defaultEnemyHP_Small;
    }
}
