using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    float speed, slowSpeed;
    bool speedFlag;    
    int direction;
    public float movementH, movementV;
    bool isTouchTop, isTouchBottom, isTouchLeft, isTouchRight;
    float shotDelay;
    float curShotDelay;
    public float maxShotDelay;
    public bool isHit = false;

    public GameObject ShotPoint;

    private void Start()
    {
        speed = Logics.Instance.GetSpeed();
        slowSpeed = Logics.Instance.GetSlowedSpeed();
    }

    private void Update()
    {
        Move();
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
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(1))
        {
            Logics.Instance.SetSlowState(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetMouseButtonUp(1))
        {
            Logics.Instance.SetSlowState(false);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
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

        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (!isHit)
            {
                isHit = true;
                Logics.Instance.PlayerHit();
                collision.gameObject.SetActive(false);
                gameObject.SetActive(false);
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
