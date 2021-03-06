using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] BGMs;


    AudioSource audSrc;
    AudioClip curClip;
    AudioClip nextClip;


    public AudioClip clipPlayerAttack;
    
    public AudioClip clipPlayerDeath;
    public AudioClip clipEnemySmallDeath;
    public AudioClip clipEnemyMediumDeath;

    private static AudioManager instance = null;
    public static AudioManager Instance
    {
        get
        {
            return instance;
        }
    }


    private void Awake()
    {
        if (instance == null) instance = this;


        audSrc = GetComponent<AudioSource>();

        audSrc.clip = BGMs[0];
        audSrc.volume = 1f;
        audSrc.loop = true;
    }

    private void Start()
    {
        audSrc.Play();
    }

    public void PlaySFX(string sfxName)
    {
        switch (sfxName)
        {
            case "PLAYER_DEATH":
                audSrc.PlayOneShot(clipPlayerDeath);
                break;
            case "EnemyDeath_Small":
                audSrc.PlayOneShot(clipEnemySmallDeath);
                break;
            case "EnemyDeath_Medium":
                audSrc.PlayOneShot(clipEnemyMediumDeath);
                break;
            case "PLAYER_ATTACK":
                audSrc.PlayOneShot(clipPlayerAttack);
                break;
        }
    }

    public void ChangeBGM(int _idx)
    {
        audSrc.clip = BGMs[_idx];
        audSrc.volume = 1f;
        audSrc.loop = true;
        audSrc.Play();

    }

    public void StopBGM()
    {
        StartCoroutine("MuteBgm");
    }

    IEnumerator MuteBgm()
    {
        while(audSrc.volume > 0.05f)
        {
            audSrc.volume -= 0.1f;

            yield return new WaitForSeconds(0.15f);
            if(audSrc.volume <= 0)
            {
                audSrc.volume = 0;
                break;
            }
        }
    }

}
