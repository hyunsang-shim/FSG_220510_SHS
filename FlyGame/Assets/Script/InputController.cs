using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    float speed, slowSpeed;
    bool speedFlag;    
    int direction;
    float movementH, movementV;
    bool isTouchTop, isTouchBottom, isTouchLeft, isTouchRight;
    float shotDelay;
    float curShotDelay;
    public float maxShotDelay;

    public GameObject[] Bullet;
    public GameObject ShotPoint;
    public int BulletPower;

    private void Start()
    {
        speed = Logics.Instance.GetSpeed();
        slowSpeed = Logics.Instance.GetSlowedSpeed();
    }

    private void Update()
    {
        Move();
        Fire();
        Reload();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            ++BulletPower;

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            --BulletPower;

        if (Input.GetKeyDown(KeyCode.KeypadDivide))
            maxShotDelay += 0.1f;

        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            maxShotDelay -= 0.1f;

#endif

    }

    private void Move() {
        direction = 0;  // ���Ⱚ �ʱ�ȭ
        speedFlag = Logics.Instance.GetSlowState(); // ���ο� ���� ����


        // �̵� ���� �Է��� �޴´�.
        movementH = Input.GetAxisRaw("Horizontal");
        movementV = Input.GetAxisRaw("Vertical");

        /// ���Ⱚ
        /// 9   1   3
        /// 8   0   2
        /// 12  4   6
        // �̵� �Է¿� ���� ���Ⱚ�� ������Ʈ �Ѵ�.
        // �����¿츦 ���� 1 2 4 8�� �ϰ�, �߸� ���¸� 0���� �Ѵ�.
        // �밢�� ������ ������ 4���� ���� ������.
        if (movementH > 0)
        {
            direction += 2;     // ������
        }
        else if (movementH < 0)
        {
            direction += 8;     // ����
        }
        else
            direction += 0;     // ���� �߸�

        //���� ���Ⱚ�� �ִ´�.
        if (movementV > 0)
        {
            direction += 1;     // ��
        }
        else if (movementV < 0)
        {
            direction += 4;     // �Ʒ�
        }
        else direction += 0;    // ���� �߸�

       


        // ĳ���� ������ ������ ä, �̵��� ���� �κ��� ����
        if ((isTouchRight && movementH == 1) || (isTouchLeft && movementH == -1))
            movementH = 0;  // �Է����� ���� �̵� ����

        if ((isTouchTop && movementV == 1) || (isTouchBottom && movementV == -1))
            movementV = 0;  // �Է����� ���� �̵� ����
        
        
        // ĳ���� ���� �ִϸ��̼��� ������Ʈ �Ѵ�
        Logics.Instance.SetDirection(direction);
        Logics.Instance.SetPlayerMovement(movementH, movementV);

        // ���Ӹ�� ����
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Logics.Instance.SetSlowState(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Logics.Instance.SetSlowState(false);
        }

    }

    void Fire()
    {
        if (!Input.GetButton("Fire1") && !Input.GetKey(KeyCode.Space))
            return;

        if (curShotDelay < maxShotDelay)
            return;
        
        // �÷��̾� �Ѿ� ����
        // Power�� ���� �Ѿ� ����
        switch(BulletPower)
        {
            case 1:
                GameObject bulletLv_1 = Instantiate(Bullet[0], ShotPoint.transform.position, transform.rotation);
                Rigidbody2D rigidLv_1 = bulletLv_1.GetComponent<Rigidbody2D>();
                rigidLv_1.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletLLv_2 = Instantiate(Bullet[0], ShotPoint.transform.position + Vector3.left * 0.1f, transform.rotation);
                Rigidbody2D rigidLLv_2 = bulletLLv_2.GetComponent<Rigidbody2D>();
                rigidLLv_2.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                GameObject bulletRLv_2 = Instantiate(Bullet[0], ShotPoint.transform.position + Vector3.right * 0.1f, transform.rotation);
                Rigidbody2D rigidRLv_2 = bulletRLv_2.GetComponent<Rigidbody2D>();
                rigidRLv_2.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletLv_3 = Instantiate(Bullet[1], ShotPoint.transform.position, transform.rotation);
                Rigidbody2D rigidLv_3 = bulletLv_3.GetComponent<Rigidbody2D>();
                rigidLv_3.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 4:
                GameObject bulletLv_4 = Instantiate(Bullet[1], ShotPoint.transform.position, transform.rotation);
                Rigidbody2D rigidLv_4 = bulletLv_4.GetComponent<Rigidbody2D>();
                rigidLv_4.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                GameObject bulletLLv_4 = Instantiate(Bullet[0], ShotPoint.transform.position + Vector3.left * 0.15f, transform.rotation);
                Rigidbody2D rigidLLv_4 = bulletLLv_4.GetComponent<Rigidbody2D>();
                rigidLLv_4.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                GameObject bulletRLv_4 = Instantiate(Bullet[0], ShotPoint.transform.position + Vector3.right * 0.15f, transform.rotation);
                Rigidbody2D rigidRLv_4 = bulletRLv_4.GetComponent<Rigidbody2D>();
                rigidRLv_4.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 5:
                GameObject bulletLLv_5 = Instantiate(Bullet[1], ShotPoint.transform.position + Vector3.left * 0.15f, transform.rotation);
                Rigidbody2D rigidLLv_5 = bulletLLv_5.GetComponent<Rigidbody2D>();
                rigidLLv_5.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                GameObject bulletRLv_5 = Instantiate(Bullet[1], ShotPoint.transform.position + Vector3.right * 0.15f, transform.rotation);
                Rigidbody2D rigidRLv_5 = bulletRLv_5.GetComponent<Rigidbody2D>();
                rigidRLv_5.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }



        

        curShotDelay = 0;
        // �Ѿ� �߻�
        
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }

}
