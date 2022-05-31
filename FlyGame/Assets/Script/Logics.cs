using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Logics : MonoBehaviour
{

    public List<Spawner> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

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


    public Text scoreText;
    public Image[] lifeImages;
    public GameObject gameOverSet;

    public int life;
    public int score;
    public int powerLevel;

    public GameObject[] enemyObj;
    public Transform[] EnemySpawnPoints;
    public float curSpawnDelay;
    public float nextSpawnDelay;

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
        spawnList = new List<Spawner>();

        if (instance == null) instance = this;

        playerShotPoint = player.transform.position + Vector3.up * 0.5f;
        BulletPower = 1;

        ReadSpawnData();
        ReadEnemyMovePattern();
    }

    private void Update()
    {
        curSpawnDelay += Time.deltaTime;
        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }

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
            tempEnemy.GetComponent<Enemy>().HP = 100;
            tempEnemy.GetComponent<Enemy>().score = 100;
        }
#endif

        Fire();
        Reload();
        UpdatePlayerShotPoint();

        scoreText.text = string.Format("{0:n0}", score);
    }

    void ReadSpawnData()
    {
        // 변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        // 파일 읽기
        TextAsset txtFile = Resources.Load("Stage_0") as TextAsset;

        StringReader stringReader = new StringReader(txtFile.text);

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();           

            if (line == null) break;
            
            Spawner spawnData = new Spawner();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnData.movePatternID = int.Parse(line.Split(',')[3]);
            spawnList.Add(spawnData);
        }

        // 파일 닫기
        stringReader.Close();

    }

    void SpawnEnemy()
    {

        GameObject tmp;

        switch (spawnList[spawnIndex].type)
        {
            case "S":
                {
                    tmp = objPool.GetObject("enemySmall");
                    tmp.GetComponent<Enemy>().Init("Small", spawnList[spawnIndex].movePatternID);

                    break;
                }
            case "M":
                {
                    tmp = objPool.GetObject("enemyMedium");
                    tmp.GetComponent<Enemy>().Init("Medium", spawnList[spawnIndex].movePatternID);

                    break;
                }
            default:
                {
                    Debug.LogWarning("Enemy Spawn Failed! -> Spawned small sized one instead.");
                    tmp = objPool.GetObject("enemySmall");
                    tmp.GetComponent<Enemy>().Init("Small", spawnList[spawnIndex].movePatternID);

                    break;
                }
        }
        
        tmp.transform.position = EnemySpawnPoints[spawnList[spawnIndex].point].position;
        tmp.transform.rotation = EnemySpawnPoints[spawnList[spawnIndex].point].rotation;

        // 다음 스폰 딜레이로 갱신
        nextSpawnDelay = spawnList[spawnIndex++].delay;
        
        // 리스트 끝이면 스폰 종료
        if (spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
        }

    }
    
    void ReadEnemyMovePattern()
    {

    }

    private void FixedUpdate()
    {
        SetAnimationDir();
        if (isSlowed)
            totalSpeed = baseSpeed * slowModifyer;
        else
            totalSpeed = baseSpeed;
        
        player.transform.position += new Vector3(playerMove_H, playerMove_V, 0) * totalSpeed * Time.deltaTime;

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
        if (!player.GetComponent<InputController>().isHit)
        {
            switch (BulletPower)
            {
                case 1:
                    GameObject bulletLv_1 = objPool.GetObject("playerBulletsA");
                    bulletLv_1.transform.position = playerShotPoint;
                    Rigidbody2D rigidLv_1 = bulletLv_1.GetComponent<Rigidbody2D>();
                    rigidLv_1.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    bulletLv_1.GetComponent<Bullet>().SetBulletDamage(1);

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
                    bulletLLv_2.GetComponent<Bullet>().SetBulletDamage(1);
                    bulletRLv_2.GetComponent<Bullet>().SetBulletDamage(1);

                    break;
                case 3:
                    GameObject bulletLv_3 = objPool.GetObject("playerBulletsB");
                    bulletLv_3.transform.position = playerShotPoint;
                    Rigidbody2D rigidLv_3 = bulletLv_3.GetComponent<Rigidbody2D>();
                    rigidLv_3.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    bulletLv_3.GetComponent<Bullet>().SetBulletDamage(3);

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
                    bulletLv_4.GetComponent<Bullet>().SetBulletDamage(3);
                    bulletLLv_4.GetComponent<Bullet>().SetBulletDamage(1);
                    bulletRLv_4.GetComponent<Bullet>().SetBulletDamage(1);
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
                    bulletLLv_5.GetComponent<Bullet>().SetBulletDamage(3);
                    bulletRLv_5.GetComponent<Bullet>().SetBulletDamage(3);
                    break;
            }

            curShotDelay = 0;
            AudioManager.Instance.PlaySFX("PLAYER_ATTACK");

        }
    }

    public void EnemyDead(GameObject e, bool f)
    {
        if (!f)
        {
            e.gameObject.SetActive(false);
            string enemySize = e.GetComponent<Enemy>().GetSize();
            AudioManager.Instance.PlaySFX("EnemyDeath_" + enemySize);
            AddScore(e.GetComponent<Enemy>().score);
            GameObject fx = objPool.GetObject("explosion_" + enemySize);
            fx.transform.position = e.transform.position;
        }
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

    public void AddScore(int p)
    {
        score += p;
    }

    public void RespawnPlayer()
    {       

        Invoke("RespawnPlayerExe", 2f);
    }

    public void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);
        player.GetComponent<InputController>().isHit = false;
    }


    public void PlayerHit()
    {
        AudioManager.Instance.PlaySFX("PLAYER_DEATH");

        GameObject playerDieFX_1 = objPool.GetObject("explosion_Medium");
        playerDieFX_1.transform.position = player.transform.position;
        playerDieFX_1.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * 2, ForceMode2D.Impulse);

        GameObject playerDieFX_2 = objPool.GetObject("explosion_Medium");
        playerDieFX_2.transform.position = player.transform.position;
        playerDieFX_2.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * 2, ForceMode2D.Impulse);

        GameObject playerDieFX_3 = objPool.GetObject("explosion_Medium");
        playerDieFX_3.transform.position = player.transform.position;
        playerDieFX_3.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, -1) * 2, ForceMode2D.Impulse);

        GameObject playerDieFX_4 = objPool.GetObject("explosion_Medium");
        playerDieFX_4.transform.position = player.transform.position;
        playerDieFX_4.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, -1) * 2, ForceMode2D.Impulse);

        life--;
        isSlowed = false;

        if (life == 0)
            GameOver();
        else
            RespawnPlayer();

        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].color = new Color(0,0,0, 0);
        }

        for (int i = 0; i < life; i++)
        {
            lifeImages[i].color = new Color(1, 1, 1, 1);
        }
    }


    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene(0);
    }
}
