using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPowerUp : MonoBehaviour
{
    Collider2D col;
    float normalDropSpeed, slowedDropSpeed;
    Vector3 curPos;

    private void Awake()
    {
        col = GetComponent<Collider2D>();        
        normalDropSpeed = 6;
        slowedDropSpeed = normalDropSpeed / Logics.Instance.GetSlowedSpeed();
        curPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Logics.Instance.AddPowerUp();
            AudioManager.Instance.PlaySFX("PowerUp");
            gameObject.SetActive(false);
        }

        if (collision.CompareTag("BulletKiller"))
            gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        curPos = transform.position;
        Vector3 nextPos = new Vector3(curPos.x, -20, curPos.z);
        if (!Logics.Instance.GetSlowState())
        {
            
            transform.position = Vector3.MoveTowards(transform.position, nextPos, normalDropSpeed * Time.fixedDeltaTime);
        }
        else
            transform.position = Vector3.MoveTowards(transform.position, nextPos, slowedDropSpeed * Time.fixedDeltaTime);

    }
}
