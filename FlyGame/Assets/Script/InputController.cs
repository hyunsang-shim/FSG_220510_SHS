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
        direction = 0;  // 방향값 초기화
        speedFlag = Logics.Instance.GetSlowState(); // 슬로우 상태 갱신


        // 이동 관련 입력을 받는다.
        movementH = Input.GetAxisRaw("Horizontal");
        movementV = Input.GetAxisRaw("Vertical");

        /// 방향값
        /// 9   1   3
        /// 8   0   2
        /// 12  4   6
        // 이동 입력에 따라 방향값을 업데이트 한다.
        // 상하좌우를 각각 1 2 4 8로 하고, 중립 상태를 0으로 한다.
        // 대각선 방향은 인접한 4방향 값의 합으로.
        if (movementH > 0)
        {
            direction += 2;     // 오른쪽
        }
        else if (movementH < 0)
        {
            direction += 8;     // 왼쪽
        }
        else
            direction += 0;     // 수평 중립

        //상하 방향값을 넣는다.
        if (movementV > 0)
        {
            direction += 1;     // 위
        }
        else if (movementV < 0)
        {
            direction += 4;     // 아래
        }
        else direction += 0;    // 수직 중립

       


        // 캐릭터 방향은 유지한 채, 이동을 막는 부분은 이쪽
        if ((isTouchRight && movementH == 1) || (isTouchLeft && movementH == -1))
            movementH = 0;  // 입력중인 수평 이동 금지

        if ((isTouchTop && movementV == 1) || (isTouchBottom && movementV == -1))
            movementV = 0;  // 입력중인 수직 이동 금지
        
        
        // 캐릭터 방향 애니메이션을 업데이트 한다
        Logics.Instance.SetDirection(direction);
        Logics.Instance.SetPlayerMovement(movementH, movementV);

        // 감속모드 조절
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
        
        // 플레이어 총알 패턴
        // Power에 따라 총알 변경
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
        // 총알 발사
        
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
