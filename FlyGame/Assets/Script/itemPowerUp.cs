using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPowerUp : MonoBehaviour
{
    Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Logics.Instance.AddPowerUp();
            Destroy(gameObject);
        }

        if (collision.CompareTag("BulletKiller"))
            Destroy(gameObject);
    }
}
