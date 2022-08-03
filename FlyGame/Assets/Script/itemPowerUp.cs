using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPowerUp : MonoBehaviour
{
    Collider2D col;
    AudioSource aud;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        aud = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Logics.Instance.AddPowerUp();
            AudioManager.Instance.PlaySFX("PowerUp");
            Destroy(gameObject);
        }

        if (collision.CompareTag("BulletKiller"))
            Destroy(gameObject);
    }
}
