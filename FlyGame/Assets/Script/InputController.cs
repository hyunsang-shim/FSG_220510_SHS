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
