using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool : MonoBehaviour
{
    // Prefabs for Enemies 
    public GameObject prefab_enemySmall;
    public GameObject prefab_enemyMedium;
    public GameObject prefab_enemyLarge;

    // Prefabs for Bullets
    public GameObject prefab_playerBulletsA;
    public GameObject prefab_playerBulletsB;
    public GameObject prefab_enemyBulletsA;
    public GameObject prefab_enemyBulletsB;
    public GameObject prefab_bossBulletsA;
    public GameObject prefab_bossBulletsB;
    public GameObject prefab_bossBulletsC;
    public GameObject prefab_bossBulletsD;

    // Prefabs for PowerUps and Etc.
    public GameObject prefab_PowerUp;
    public GameObject prefab_SpeedUp;
    public GameObject prefab_bonuses;
    public GameObject prefab_explosion_Big;
    public GameObject prefab_explosion_Small;



    // Enemies
    GameObject[] enemySmall;
    GameObject[] enemyMedium;
    GameObject[] enemyLarge;
    
    // Bullets
    GameObject[] playerBulletsA;
    GameObject[] playerBulletsB;
    GameObject[] enemyBulletsA;
    GameObject[] enemyBulletsB;
    GameObject[] bossBulletsA;
    GameObject[] bossBulletsB;
    GameObject[] bossBulletsC;
    GameObject[] bossBulletsD;

    // PowerUps and Etc.
    GameObject[] powerUp;
    GameObject[] speedUp;
    GameObject[] bonuses;
    GameObject[] explosion_Medium;
    GameObject[] explosion_Small;


    GameObject[] targetPool;

    private void Awake()
    {
        enemySmall = new GameObject[30];
        enemyMedium = new GameObject[10];
        enemyLarge = new GameObject[5];

        playerBulletsA = new GameObject[250];
        playerBulletsB = new GameObject[250];
        enemyBulletsA = new GameObject[70];
        enemyBulletsB = new GameObject[70];
        bossBulletsA = new GameObject[200];
        bossBulletsB = new GameObject[200];
        bossBulletsC = new GameObject[200];
        bossBulletsD = new GameObject[200];

        powerUp = new GameObject[5];
        speedUp = new GameObject[5];
        bonuses = new GameObject[5];
        explosion_Medium = new GameObject[30];
        explosion_Small= new GameObject[30];

        Generate();
    }

    void Generate()
    {
        for (int i = 0; i < enemySmall.Length; i++)
        {
            enemySmall[i] = Instantiate(prefab_enemySmall);
            enemySmall[i].SetActive(false);
            enemySmall[i].transform.SetParent(transform);
        }

        for (int i = 0; i < enemyMedium.Length; i++)
        {
            enemyMedium[i] = Instantiate(prefab_enemyMedium);
            enemyMedium[i].SetActive(false);
            enemyMedium[i].transform.SetParent(transform);
        }

        for (int i = 0; i < enemyLarge.Length; i++)
        {
            enemyLarge[i] = Instantiate(prefab_enemyLarge);
            enemyLarge[i].SetActive(false);
            enemyLarge[i].transform.SetParent(transform);
        }

        for (int i = 0; i < playerBulletsA.Length; i++)
        {
            playerBulletsA[i] = Instantiate(prefab_playerBulletsA);
            playerBulletsA[i].SetActive(false);
            playerBulletsA[i].transform.SetParent(transform);
        }

        for (int i = 0; i < playerBulletsB.Length; i++)
        {
            playerBulletsB[i] = Instantiate(prefab_playerBulletsB);
            playerBulletsB[i].SetActive(false);
            playerBulletsB[i].transform.SetParent(transform);
        }

        for (int i = 0; i < enemyBulletsA.Length; i++)
        {
            enemyBulletsA[i] = Instantiate(prefab_enemyBulletsA);
            enemyBulletsA[i].SetActive(false);
            enemyBulletsA[i].transform.SetParent(transform);
        }

        for (int i = 0; i < enemyBulletsB.Length; i++)
        {
            enemyBulletsB[i] = Instantiate(prefab_enemyBulletsB);
            enemyBulletsB[i].SetActive(false);
            enemyBulletsB[i].transform.SetParent(transform);
        }

        for (int i = 0; i < bossBulletsA.Length; i++)
        {
            bossBulletsA[i] = Instantiate(prefab_bossBulletsA);
            bossBulletsA[i].SetActive(false);
            bossBulletsA[i].transform.SetParent(transform);
        }

        for (int i = 0; i < bossBulletsB.Length; i++)
        {
            bossBulletsB[i] = Instantiate(prefab_bossBulletsB);
            bossBulletsB[i].SetActive(false);
            bossBulletsB[i].transform.SetParent(transform);

        }

        for (int i = 0; i < bossBulletsC.Length; i++)
        {
            bossBulletsC[i] = Instantiate(prefab_bossBulletsC);
            bossBulletsC[i].SetActive(false);
            bossBulletsC[i].transform.SetParent(transform);
        }

        for (int i = 0; i < bossBulletsD.Length; i++)
        {
            bossBulletsD[i] = Instantiate(prefab_bossBulletsD);
            bossBulletsD[i].SetActive(false);
            bossBulletsD[i].transform.SetParent(transform);
        }

        for (int i = 0; i < powerUp.Length; i++)
        {
            powerUp[i] = Instantiate(prefab_PowerUp);
            powerUp[i].SetActive(false);
            powerUp[i].transform.SetParent(transform);
        }

        for (int i = 0; i < speedUp.Length; i++)
        {
            speedUp[i] = Instantiate(prefab_SpeedUp);
            speedUp[i].SetActive(false);
            speedUp[i].transform.SetParent(transform);
        }

        for (int i = 0; i < bonuses.Length; i++)
        {
            bonuses[i] = Instantiate(prefab_bonuses);
            bonuses[i].SetActive(false);
            bonuses[i].transform.SetParent(transform);
        }

        for (int i = 0; i < explosion_Medium.Length; i++)
        {
            explosion_Medium[i] = Instantiate(prefab_explosion_Big);
            explosion_Medium[i].SetActive(false);
            explosion_Medium[i].transform.SetParent(transform);
        }

        for (int i = 0; i < explosion_Small.Length; i++)
        {
            explosion_Small[i] = Instantiate(prefab_explosion_Small);
            explosion_Small[i].SetActive(false);
            explosion_Small[i].transform.SetParent(transform);
        }

    }

    public GameObject GetObject(string type)
    {

        switch(type)
        {
            case "enemySmall":
                targetPool = enemySmall;
                break;
            case "enemyMedium":
                targetPool = enemyMedium;
                break;
            case "boss":
                targetPool = enemyLarge;
                break;
            case "playerBulletsA":
                targetPool = playerBulletsA;
                break;
            case "playerBulletsB":
                targetPool = playerBulletsB;
                break;
            case "enemyBulletsA":
                targetPool = enemyBulletsA;
                break;
            case "enemyBulletsB":
                targetPool = enemyBulletsB;
                break;
            case "bossBulletsA":
                targetPool = bossBulletsA;
                break;
            case "bossBulletsB":
                targetPool = bossBulletsB;
                break;
            case "bossBulletsC":
                targetPool = bossBulletsC;
                break;
            case "bossBulletsD":
                targetPool = bossBulletsD;
                break;
            case "PowerUp":
                targetPool = powerUp;
                break;
            case "SpeedUp":
                targetPool = speedUp;
                break;
            case "bonuses":
                targetPool = bonuses;
                break;
            case "explosion_Small":
                targetPool = explosion_Small;
                break;
            case "explosion_Medium":
                targetPool = explosion_Medium;
                break;
        }

        for (int i = 0; i < targetPool.Length; i++)
        {
            if (!targetPool[i].activeSelf)
            {
                targetPool[i].SetActive(true);
                return targetPool[i];
            }
        }

        return null;
    }

    public GameObject[] GetEnemyBulletsA()
    {
        return enemyBulletsA;
    }

    public GameObject[] GetEnemyBulletsB()
    {
        return enemyBulletsB;
    }
}
