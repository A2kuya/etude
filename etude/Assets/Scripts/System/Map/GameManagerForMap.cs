using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerForMap : MonoBehaviour
{
    public TalkManager talkManager;
    public Text talkText;
    public GameObject talkPanel;
    public GameObject scanObject;
    public int talkIndex;

    public bool isAction;

    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        ObjData objData = scanObject.GetComponent<ObjData>();
        Talk(objData.id,objData.isNpc);

        talkPanel.SetActive(isAction);

    }

    void Talk(int id, bool isNpc)
    {
        string talkData= talkManager.GetTalk(id, talkIndex);

        if(talkData==null)
        {
            isAction=false;
            talkIndex=0;
            return;
        }

        if(isNpc) 
        {
            talkText.text=talkData;
        }
        else
        {
            talkText.text=talkData;
        }

        isAction=true;
        talkIndex++;
    }

}