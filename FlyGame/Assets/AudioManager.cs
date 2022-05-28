using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] BGMs;


    AudioSource audSrc;
    AudioClip curClip;
    AudioClip nextClip;



    private void Awake()
    {
        audSrc = GetComponent<AudioSource>();

        audSrc.clip = BGMs[0];
        audSrc.volume = 0.8f;
        audSrc.loop = true;
    }

    private void Start()
    {
        audSrc.Play();
    }




}
