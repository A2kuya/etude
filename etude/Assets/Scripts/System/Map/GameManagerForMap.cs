using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerForMap : MonoBehaviour
{

    public Text talkText;
    public GameObject talkPanel;
    public GameObject scanObject;

    public bool isAction;

    public void Action(GameObject scanObj)
    {
        if(isAction)
        {
            isAction=false;
        }
        isAction=true;
        scanObject=scanObj;
        talkText.text="안녕 나는 상인이야.";
        talkPanel.SetActive(isAction);

    }

}