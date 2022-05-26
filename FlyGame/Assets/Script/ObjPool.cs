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

    // Prefabs for PowerUps and Etc.
    public GameObject prefab_powerUps_NormalPower;
    public GameObject prefab_powerUps_FullPower;
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

    // PowerUps and Etc.
    GameObject[] powerUps_NormalPower;
    GameObject[] powerUps_FullPower;
    GameObject[] bonuses;
    GameObject[] explosion_Medium;
    GameObject[] explosion_Small;


    GameObject[] targetPool;

    private void Awake()
    {
        enemySmall = new GameObject[10];
        enemyMedium = new GameObject[10];
        enemyLarge = new GameObject[1];

        playerBulletsA = new GameObject[50];
        playerBulletsB = new GameObject[50];
        enemyBulletsA = new GameObject[50];
        enemyBulletsB = new GameObject[50];
        bossBulletsA = new GameObject[50];
        bossBulletsB = new GameObject[50];
        bossBulletsC = new GameObject[50];

        powerUps_NormalPower = new GameObject[5];
        powerUps_FullPower = new GameObject[5];
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
        }

        for (int i = 0; i < enemyMedium.Length; i++)
        {
            enemyMedium[i] = Instantiate(prefab_enemyMedium);
            enemyMedium[i].SetActive(false);
        }

        for (int i = 0; i < enemyLarge.Length; i++)
        {
            enemyLarge[i] = Instantiate(prefab_enemyLarge);
            enemyLarge[i].SetActive(false);
        }

        for (int i = 0; i < playerBulletsA.Length; i++)
        {
            playerBulletsA[i] = Instantiate(prefab_playerBulletsA);
            playerBulletsA[i].SetActive(false);
        }

        for (int i = 0; i < playerBulletsB.Length; i++)
        {
            playerBulletsB[i] = Instantiate(prefab_playerBulletsB);
            playerBulletsB[i].SetActive(false);
        }

        for (int i = 0; i < enemyBulletsA.Length; i++)
        {
            enemyBulletsA[i] = Instantiate(prefab_enemyBulletsA);
            enemyBulletsA[i].SetActive(false);
        }

        for (int i = 0; i < enemyBulletsB.Length; i++)
        {
            enemyBulletsB[i] = Instantiate(prefab_enemyBulletsB);
            enemyBulletsB[i].SetActive(false);
        }

        for (int i = 0; i < bossBulletsA.Length; i++)
        {
            bossBulletsA[i] = Instantiate(prefab_bossBulletsA);
            bossBulletsA[i].SetActive(false);
        }

        for (int i = 0; i < bossBulletsB.Length; i++)
        {
            bossBulletsB[i] = Instantiate(prefab_bossBulletsB);
            bossBulletsB[i].SetActive(false);

        }

        for (int i = 0; i < bossBulletsC.Length; i++)
        {
            bossBulletsC[i] = Instantiate(prefab_bossBulletsC);
            bossBulletsC[i].SetActive(false);
        }

        for (int i = 0; i < powerUps_NormalPower.Length; i++)
        {
            powerUps_NormalPower[i] = Instantiate(prefab_powerUps_NormalPower);
            powerUps_NormalPower[i].SetActive(false);
        }

        for (int i = 0; i < powerUps_FullPower.Length; i++)
        {
            powerUps_FullPower[i] = Instantiate(prefab_powerUps_FullPower);
            powerUps_FullPower[i].SetActive(false);
        }

        for (int i = 0; i < bonuses.Length; i++)
        {
            bonuses[i] = Instantiate(prefab_bonuses);
            bonuses[i].SetActive(false);
        }

        for (int i = 0; i < explosion_Medium.Length; i++)
        {
            explosion_Medium[i] = Instantiate(prefab_explosion_Big);
            explosion_Medium[i].SetActive(false);
        }

        for (int i = 0; i < explosion_Small.Length; i++)
        {
            explosion_Small[i] = Instantiate(prefab_explosion_Small);
            explosion_Small[i].SetActive(false);
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
            case "enemyLarge":
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
            case "powerUps_NormalPower":
                targetPool = powerUps_NormalPower;
                break;
            case "powerUps_FullPower":
                targetPool = powerUps_FullPower;
                break;
            case "bonuses":
                targetPool = bonuses;
                break;
            case "explosion_Small":
                targetPool = explosion_Small;
                break;
            case "explosion_Big":
                targetPool = explosion_Medium;
                break;
        }

        for (int i = 0; i < targetPool.Length; i++)
        {
            if(!targetPool[i].activeSelf)
            {
                targetPool[i].SetActive(true);
                return targetPool[i];
            }
        }

        return null;
    }
}
