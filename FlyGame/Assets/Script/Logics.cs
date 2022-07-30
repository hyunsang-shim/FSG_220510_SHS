using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Logics : MonoBehaviour
{

    List<Spawner> spawnList;
    int spawnIndex;
    bool spawnEnd;

    
    int direction;

    [Header("필수 객체")]
    public GameObject player;
    public Animator animator;
    public ObjPool objPool;
    public Transform[] enemyMovePoints;
    Vector3 playerShotPoint;
    public GameObject[] dropItems;
    public GameObject BossObject;

    // 밸런스 요소들
    [Header("플레이어 밸런스 요소")]
    public int life;
    public int powerLevel;
    public int BulletPower;        
    public float maxShotDelay;  // 설정된 총알 발사 딜레이
    public float baseSpeed;   // 기본 기체 이동 속도    
    bool isBossAppear = false;
    int highScore;

    [Header("밸런스 요소 (디버그용)")]
    public int defaultEnemyHP_Small;
    public int defaultEnemyHP_Medium;
    public int defaultEnemyHP_Boss;
    

    // 디버그용 요소
    [Header("플레이 정보 (디버그용)")]
    public int aliveEnemies = 0;
    public float forceSpawnDelay = 0.5f;
    public float curSpawnDelay;
    public float nextSpawnDelay;
    public float playerMove_H, playerMove_V;


    // UI 요소들
    [Header("UI 구성 요소")]
    public Text scoreText;
    public Text highScoreText;
    public Sprite lifeImage;
    public GameObject BossHPBar;

    [Header("UI Prefabs")]
    public GameObject gameOverSet;
    public GameObject gameClearSet;

    [Header("UI 내용 요소")]
    public Canvas UIRoot;
    public GameObject lifeIconsRoot;
    public int score;


    List<List<int>> movePattern = new List<List<int>>();
    

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


        spawnList = new List<Spawner>();
        playerShotPoint = player.transform.position + Vector3.up * 0.5f;
        BulletPower = 1;

        UIRoot = FindObjectOfType<Canvas>();

        for(int i = 0; i < life; i++)
        {
            GameObject l = new GameObject();
            l.AddComponent<Image>();
            l.GetComponent<Image>().sprite = lifeImage;
            l.transform.SetParent(lifeIconsRoot.transform);
            l.name = $"life_{i}";
        }
        BossHPBar.gameObject.SetActive(false);
        Debug.Log($"BossHPBar is {BossHPBar.activeSelf}");
        highScore = PlayerPrefs.GetInt("highScore", 5000);
        highScoreText.text = $"HighScore: {highScore}";
        ReadSpawnData();
        ReadEnemyMovePatterns();
    }

    private void Update()
    {
        

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            AddPowerUp();

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            --BulletPower;

        if (Input.GetKeyDown(KeyCode.KeypadDivide))
            maxShotDelay += 0.1f;

        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            maxShotDelay -= 0.1f;

        // Debug : Spawn Enemies from the top of the list
        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
        {
            curSpawnDelay = 0;
            spawnIndex = 0;
            spawnEnd = false;

        }
#endif

       

        scoreText.text = string.Format("Score: {0:n0}", score);
        highScoreText.text = string.Format("High Score: {0:n0}", highScore);
    }

    private void FixedUpdate()
    {
        Fire();
        Reload();
        UpdatePlayerShotPoint();

        curSpawnDelay += Time.fixedDeltaTime;

        if ((curSpawnDelay > forceSpawnDelay) && (aliveEnemies == 0) && !spawnEnd)
            nextSpawnDelay = forceSpawnDelay;

        if ((curSpawnDelay > nextSpawnDelay) && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }

        SetAnimationDir();
        if (isSlowed)
            totalSpeed = baseSpeed * slowModifyer;
        else
            totalSpeed = baseSpeed;

        player.transform.position += new Vector3(playerMove_H, playerMove_V, 0) * totalSpeed * Time.fixedDeltaTime;

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

        string line = stringReader.ReadLine();
        while (stringReader != null)
        {
            line = stringReader.ReadLine();
            if (line == null) break;
            
            Spawner spawnData = new Spawner();
            spawnData.spawnDelay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.movePatternID = int.Parse(line.Split(',')[2]);
            spawnData.speed = int.Parse(line.Split(',')[3]);
            spawnData.dropType = line.Split(',')[4];
            spawnData.shotDelay = float.Parse(line.Split(',')[5]);

            spawnList.Add(spawnData);
        }

        // 파일 닫기
        stringReader.Close();

    }

    void ReadEnemyMovePatterns()
    {

        TextAsset txtFile = Resources.Load("EnemyMovePoints") as TextAsset;
        StringReader stringReader = new StringReader(txtFile.text);
        int patternCount = 0;
        while (stringReader != null)
        {
            string line = stringReader.ReadLine();

            if (line == null) break;

            movePattern.Add(new List<int>());
            int count = int.Parse(line.Split(',')[0]);

            for (int i = 1; i <= count; i++)
            {
                movePattern[patternCount].Add(int.Parse(line.Split(',')[i]));
            }

            patternCount++;

        }
    }

    void SpawnEnemy()
    {

        GameObject tmp;
        switch (spawnList[spawnIndex].type)
        {            
            case "S":
                {
                    tmp = objPool.GetObject("enemySmall");
                    if (tmp != null)
                    {
                        //tmp.GetComponent<Enemy>().Init("Small", "OneShotToTarget", spawnList[spawnIndex].movePatternID, spawnList[spawnIndex].speed, spawnList[spawnIndex].dropType, spawnList[spawnIndex].shotDelay);
                        tmp.GetComponent<Enemy>().Init("Small", "OneShotToTarget", spawnList[spawnIndex].movePatternID, spawnList[spawnIndex].speed, spawnList[spawnIndex].dropType, spawnList[spawnIndex].shotDelay);
                        aliveEnemies++;
                    }
                    break;
                }
            case "M":
                {
                    tmp = objPool.GetObject("enemyMedium");
                    if (tmp != null)
                    {
                        tmp.GetComponent<Enemy>().Init("Medium", "3-Way", spawnList[spawnIndex].movePatternID, spawnList[spawnIndex].speed, spawnList[spawnIndex].dropType, spawnList[spawnIndex].shotDelay);
                        aliveEnemies++;
                    }

                    break;
                }
            default:
                {
                    Debug.LogWarning("Enemy Spawn Failed! -> Spawned small sized one instead.");
                    tmp = objPool.GetObject("enemySmall");
                    if (tmp != null)
                    {
                        tmp.GetComponent<Enemy>().Init("Small", "OneShotToTarget", spawnList[spawnIndex].movePatternID, spawnList[spawnIndex].speed, spawnList[spawnIndex].dropType, spawnList[spawnIndex].shotDelay);
                        aliveEnemies++;
                    }
                    break;
                }
        }

        // 다음 스폰 딜레이로 갱신
        nextSpawnDelay = spawnList[spawnIndex++].spawnDelay;
        
        // 리스트 끝이면 스폰 종료
        // 보스전 시작 준비
        if (spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            Invoke("StartBossStage", 3f);
            AudioManager.Instance.StopBGM();
        }

    }
    
    public List<Transform> GetEnemyMovePoints(int _patternNumber)
    {
        int cnt = movePattern[_patternNumber].Count;
        List<Transform> ret = new List<Transform>();
        for (int i =0; i < cnt; i++)
        {
            ret.Add(enemyMovePoints[movePattern[_patternNumber][i]]);
        }

        return ret;
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
                case 1:     //      |
                    GameObject bulletLv_1 = objPool.GetObject("playerBulletsA");
                    bulletLv_1.transform.position = playerShotPoint;
                    bulletLv_1.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 1, false);
                    bulletLv_1.GetComponent<Bullet>().SetBulletDamage(1);
                    break;
                case 2:     //       ||
                    GameObject bulletLLv_2 = objPool.GetObject("playerBulletsA");
                    GameObject bulletRLv_2 = objPool.GetObject("playerBulletsA");
                    bulletLLv_2.transform.position = playerShotPoint + Vector3.left * 0.1f;
                    bulletRLv_2.transform.position = playerShotPoint + Vector3.right * 0.1f;
                    bulletLLv_2.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 1, false);
                    bulletLLv_2.GetComponent<Bullet>().SetBulletDamage(1);
                    bulletRLv_2.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 1, false);
                    bulletRLv_2.GetComponent<Bullet>().SetBulletDamage(1);
                    break;
                case 3:     //      \ | /
                    GameObject bulletLLv_3 = objPool.GetObject("playerBulletsA");
                    GameObject bulletMLv_3 = objPool.GetObject("playerBulletsA");
                    GameObject bulletRLv_3 = objPool.GetObject("playerBulletsA");
                    bulletLLv_3.transform.position = playerShotPoint + Vector3.left * 0.1f;
                    bulletMLv_3.transform.position = playerShotPoint;
                    bulletRLv_3.transform.position = playerShotPoint + Vector3.right * 0.1f;
                    bulletLLv_3.transform.Rotate(Vector3.forward, Mathf.Sin(Mathf.PI * (-0.18f)));
                    bulletRLv_3.transform.Rotate(Vector3.forward, Mathf.Sin(Mathf.PI * 0.18f));
                    bulletLLv_3.GetComponent<Bullet>().SetBullet(new Vector2(Mathf.Sin(Mathf.PI * (-0.18f)), 1).normalized * 10, 1, false);
                    bulletMLv_3.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 1, false);
                    bulletRLv_3.GetComponent<Bullet>().SetBullet(new Vector2(Mathf.Sin(Mathf.PI * 0.18f), 1).normalized * 10, 1, false);
                    break; 
                    
                    //GameObject bulletLv_3 = objPool.GetObject("playerBulletsB");
                    //bulletLv_3.transform.position = playerShotPoint;
                    //Rigidbody2D rigidLv_3 = bulletLv_3.GetComponent<Rigidbody2D>();
                    //rigidLv_3.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    //bulletLv_3.GetComponent<Bullet>().SetBullet(Vector2.up * 10, false);
                    //bulletLv_3.GetComponent<Bullet>().SetBulletDamage(3);
                    //break;
                case 4:     //      \\ || //
                    GameObject bulletLLv_4_1 = objPool.GetObject("playerBulletsA");
                    GameObject bulletLLv_4_2 = objPool.GetObject("playerBulletsA");
                    GameObject bulletMLv_4_1 = objPool.GetObject("playerBulletsA");
                    GameObject bulletMLv_4_2 = objPool.GetObject("playerBulletsA");
                    GameObject bulletRLv_4_1 = objPool.GetObject("playerBulletsA");
                    GameObject bulletRLv_4_2 = objPool.GetObject("playerBulletsA");
                    bulletLLv_4_1.transform.position = playerShotPoint + Vector3.left * 0.1f;
                    bulletLLv_4_2.transform.position = playerShotPoint + Vector3.left * 0.2f;
                    bulletMLv_4_1.transform.position = playerShotPoint + Vector3.left * 0.1f;
                    bulletMLv_4_2.transform.position = playerShotPoint + Vector3.right * 0.1f;
                    bulletRLv_4_1.transform.position = playerShotPoint + Vector3.right * 0.1f;
                    bulletRLv_4_2.transform.position = playerShotPoint + Vector3.right * 0.2f;
                    bulletLLv_4_1.transform.Rotate(Vector3.forward, Mathf.Sin(Mathf.PI * (-0.15f)));
                    bulletLLv_4_2.transform.Rotate(Vector3.forward, Mathf.Sin(Mathf.PI * (-0.18f)));
                    bulletRLv_4_1.transform.Rotate(Vector3.forward, Mathf.Sin(Mathf.PI * 0.15f));
                    bulletRLv_4_2.transform.Rotate(Vector3.forward, Mathf.Sin(Mathf.PI * 0.18f));
                    bulletLLv_4_1.GetComponent<Bullet>().SetBullet(new Vector2(Mathf.Sin(Mathf.PI * (-0.15f)), 1).normalized * 10, 1, false);
                    bulletLLv_4_2.GetComponent<Bullet>().SetBullet(new Vector2(Mathf.Sin(Mathf.PI * (-0.18f)), 1).normalized * 10, 1, false);
                    bulletMLv_4_1.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 1, false);
                    bulletMLv_4_2.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 1, false);
                    bulletRLv_4_1.GetComponent<Bullet>().SetBullet(new Vector2(Mathf.Sin(Mathf.PI * 0.15f), 1).normalized * 10, 1, false);
                    bulletRLv_4_2.GetComponent<Bullet>().SetBullet(new Vector2(Mathf.Sin(Mathf.PI * 0.18f), 1).normalized * 10, 1, false);
                    break;

                //GameObject bulletLv_4 = objPool.GetObject("playerBulletsB");
                //GameObject bulletLLv_4 = objPool.GetObject("playerBulletsA");
                //GameObject bulletRLv_4 = objPool.GetObject("playerBulletsA");
                //bulletLv_4.transform.position = playerShotPoint;
                //bulletLLv_4.transform.position = playerShotPoint + Vector3.left * 0.15f;
                //bulletRLv_4.transform.position = playerShotPoint + Vector3.right * 0.15f;
                //bulletLv_4.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 1, false);
                //bulletLLv_4.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 3, false);
                //bulletRLv_4.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 1, false);
                //break;
                case 5:     //      \\ ㅁ //
                    GameObject bulletLLv_5_1 = objPool.GetObject("playerBulletsA");
                    GameObject bulletLLv_5_2 = objPool.GetObject("playerBulletsA");
                    GameObject bulletMLv_5 = objPool.GetObject("playerBulletsB");
                    GameObject bulletRLv_5_1 = objPool.GetObject("playerBulletsA");
                    GameObject bulletRLv_5_2 = objPool.GetObject("playerBulletsA");
                    bulletLLv_5_1.transform.position = playerShotPoint + Vector3.left * 0.1f;
                    bulletLLv_5_2.transform.position = playerShotPoint + Vector3.left * 0.2f;
                    bulletMLv_5.transform.position = playerShotPoint;
                    bulletRLv_5_1.transform.position = playerShotPoint + Vector3.right * 0.1f;
                    bulletRLv_5_2.transform.position = playerShotPoint + Vector3.right * 0.2f;
                    bulletLLv_5_1.transform.Rotate(Vector3.forward, Mathf.Sin(Mathf.PI * (-0.15f)));
                    bulletLLv_5_2.transform.Rotate(Vector3.forward, Mathf.Sin(Mathf.PI * (-0.18f)));
                    bulletRLv_5_1.transform.Rotate(Vector3.forward, Mathf.Sin(Mathf.PI * 0.15f));
                    bulletRLv_5_2.transform.Rotate(Vector3.forward, Mathf.Sin(Mathf.PI * 0.18f));
                    bulletLLv_5_1.GetComponent<Bullet>().SetBullet(new Vector2(Mathf.Sin(Mathf.PI * (-0.15f)), 1).normalized * 10, 1, false);
                    bulletLLv_5_2.GetComponent<Bullet>().SetBullet(new Vector2(Mathf.Sin(Mathf.PI * (-0.18f)), 1).normalized * 10, 1, false);
                    bulletMLv_5.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 3, false);
                    bulletRLv_5_1.GetComponent<Bullet>().SetBullet(new Vector2(Mathf.Sin(Mathf.PI * 0.15f), 1).normalized * 10, 1, false);
                    bulletRLv_5_2.GetComponent<Bullet>().SetBullet(new Vector2(Mathf.Sin(Mathf.PI * 0.18f), 1).normalized * 10, 1, false);

                    break;
                    //GameObject bulletLLv_5 = objPool.GetObject("playerBulletsB");
                    //GameObject bulletRLv_5 = objPool.GetObject("playerBulletsB");
                    //bulletLLv_5.transform.position = playerShotPoint + Vector3.left * 0.15f;
                    //bulletRLv_5.transform.position = playerShotPoint + Vector3.right * 0.15f;
                    //bulletLLv_5.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 3, false);
                    //bulletRLv_5.GetComponent<Bullet>().SetBullet(Vector2.up * 10, 3, false);
                    //break;
            }

            curShotDelay = 0;
            AudioManager.Instance.PlaySFX("PLAYER_ATTACK");

        }
    }

    public void EnemyDead(GameObject _enemy, bool _isDead, int _score)
    {
        Enemy e = _enemy.GetComponent<Enemy>();
        if (!_isDead)
        {
            _enemy.gameObject.SetActive(false);
            string enemySize = e.GetSize();
            AudioManager.Instance.PlaySFX("EnemyDeath_" + enemySize);
            AddScore(_score);
            GameObject fx = objPool.GetObject("explosion_" + enemySize);
            fx.transform.position = e.transform.position;
            aliveEnemies--;                    
        }
    }

    void Reload()
    {
        curShotDelay += Time.fixedDeltaTime;
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

        if (score >= highScore)
        {
            highScore = score;
        }
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

        for (int i = 0; i < lifeIconsRoot.transform.childCount; i++)
        {
            if (i < life)
            {
                lifeIconsRoot.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
                lifeIconsRoot.transform.GetChild(i).gameObject.SetActive(false);
        }
    }


    public void GameOver()
    {
        gameOverSet.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GameClear()
    {
        gameClearSet.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Retry()
    {
        SceneManager.LoadScene(0);
    }
    
    public float GetSlowModifier()
    {
        return slowModifyer;
    }

    public void DropItem(Vector3 _position, string _itemName)
    {
        if(_itemName == "Speed")
        {
            GameObject item = Instantiate(dropItems[0]);
            item.transform.position = _position;
        }
        else if (_itemName == "Power")
        {
            GameObject item = Instantiate(dropItems[1]);
            item.transform.position = _position;
        }
    }


    public void AddSpeedUp()
    {
        ++baseSpeed;
        maxShotDelay -= 0.125f;

        if (baseSpeed > 8)
            baseSpeed = 8;

        if (maxShotDelay < 0.1f)
            maxShotDelay = 0.1f;
    }

    public void AddPowerUp()
    {
        ++BulletPower;
        if (BulletPower > 5)
            BulletPower = 5;
    }

    public bool GetBossAppearState()
    {
        return isBossAppear;
    }
    void StartBossStage()
    {
        Debug.LogWarning("Boss Stage Start!!!");
        isBossAppear = true;
        AudioManager.Instance.ChangeBGM(1);
        GameObject boss = Instantiate(BossObject);
    }


    public void UpdateBossHP(int _curHP)
    {
        BossHPBar.GetComponentInChildren<Slider>().value = _curHP;

    }
    public void SetBossHPUI(int _maxHP)
    {         
        BossHPBar.GetComponentInChildren<Slider>().maxValue = _maxHP;
        BossHPBar.GetComponentInChildren<Slider>().value = _maxHP;
        BossHPBar.SetActive(true);
    }

    bool isBossPhase2 = false;
    public bool GetBossPhase2()    {        return isBossPhase2;    }
    public void SetBossPhase2()    {        isBossPhase2 = true;    }
}
