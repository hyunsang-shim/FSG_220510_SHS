using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFlicker : MonoBehaviour
{

    bool showMessage = true;
    Text txt;
    public string txtFlickText;


    private void Awake()
    {
        txt = GetComponent<Text>();
        txt.text = txtFlickText;
    }
    private void Start()
    { 
        StartCoroutine("Flick");
    }

    IEnumerator Flick()
    {
        while(showMessage)
        {
            txt.text = "";

            yield return new WaitForSecondsRealtime(0.3f);

            txt.text = txtFlickText;

            yield return new WaitForSecondsRealtime(0.7f);
        }
    }
}
