using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Logics.Instance.DIR = Logics.PLAYER_DIR.LEFT;
        }
        else if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            Logics.Instance.DIR = Logics.PLAYER_DIR.NONE;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Logics.Instance.DIR = Logics.PLAYER_DIR.RIGHT;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            Logics.Instance.DIR = Logics.PLAYER_DIR.NONE;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Logics.Instance.DIR = Logics.PLAYER_DIR.UP;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            Logics.Instance.DIR = Logics.PLAYER_DIR.NONE;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Logics.Instance.DIR = Logics.PLAYER_DIR.DOWN;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            Logics.Instance.DIR = Logics.PLAYER_DIR.NONE;
        }


    }
}
