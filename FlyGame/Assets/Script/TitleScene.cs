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
    GameObject heart;
    public int selectedMenu = 1;
    


    private void Awake()
    {
        GetHighScore();
        heart = new GameObject();
        heart = Instantiate(objCursor);
        heart.transform.SetParent(gameObject.transform);
        heart.SetActive(false);
        AudioManager.Instance.ChangeBGM(0);
    }

    private void Update()
    {

        if (Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Return))
        {
            if (showButtons)
            {
                switch (selectedMenu)
                {
                    case 0:
                    case 1:
                        StartGame();
                        break;
                    case 2:
                        QuitGame();
                        break;

                }
            }
        }

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.anyKeyDown)
        {
            if (!showButtons)
            {
                flickerText.gameObject.SetActive(false);
                btnStart.gameObject.SetActive(true);
                btnQuit.gameObject.SetActive(true);
                heart.transform.position = transformStartButtonCursor.transform.position;
                heart.transform.localScale = Vector3.one;
                heart.SetActive(true);
                showButtons = true;
            }
        }

        if (showButtons)
        {
            heart.SetActive(true);

            if(selectedMenu == 0)
                heart.transform.position = transformStartButtonCursor.transform.position;

            if (Input.GetAxisRaw("Vertical") == 1)
            {
                heart.transform.position = transformStartButtonCursor.transform.position;
                Debug.Log($"Heart Position: {heart.transform.position.y}");
                AudioManager.Instance.PlaySFX("MenuSelect");
                selectedMenu = 1;
            }
            else if (Input.GetAxisRaw("Vertical") == -1)
            {
                heart.transform.position = transformQuitButtonCursor.transform.position;
                Debug.Log($"Heart Position: {heart.transform.position.y}");
                AudioManager.Instance.PlaySFX("MenuSelect"); 
                selectedMenu = 2;
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

    private void OnGUI()
    {
        GUIStyle st = new GUIStyle();
        st.font = new Font("DungGeunMo");

        GUI.Label(new Rect(0, 0, 300, 300), $"showButtons: {showButtons}, selectedMenu: {selectedMenu}", st);
    }
}
