using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class TitleScene : MonoBehaviour
{
    public Button btnStart;
    public Button btnQuit;
    public Text txtHighScore;
    public Text flickerText;
    public bool showButtons = false;
    public Transform transformStartButtonCursor;
    public Transform transformQuitButtonCursor;
    public GameObject objCursor;
    public int selectedMenu = 0;
    


    private void Awake()
    {
        GetHighScore();
        objCursor.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.anyKeyDown)
        {
            if (!showButtons)
            {
                flickerText.gameObject.SetActive(false);
                btnStart.gameObject.SetActive(true);
                btnQuit.gameObject.SetActive(true);
                objCursor.transform.position = transformStartButtonCursor.transform.position;
                objCursor.SetActive(true);
                showButtons = true;
            }
        }

        if (showButtons)
        {
            if (Input.GetAxisRaw("Vertical") == 1)
            {
                objCursor.transform.position = transformStartButtonCursor.transform.position;
                selectedMenu = 1;
            }
            else if (Input.GetAxisRaw("Vertical") == -1)
            {
                objCursor.transform.position = transformStartButtonCursor.transform.position;
                selectedMenu = 2;
            }

            if (Input.GetButtonDown("Submit"))
            {
                switch (selectedMenu)
                {
                    case 1:
                        StartGame();
                        break;
                    case 2:
                        QuitGame();
                        break;

                }
            }
        }
        

    }

    void GetHighScore()
    {
        StreamReader stringReader = new StreamReader(getPath());

        string line = stringReader.ReadLine();
        txtHighScore.text = string.Format("{0:n0}", int.Parse(line));
        stringReader.Close();
    }
    private string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Resources/" + "HighScore.csv";
#else
        return Application.dataPath +"/"+"HighScore.csv";
#endif
    }


    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
