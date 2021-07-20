using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    //NPC talk

    public TalkManager talkManager;
    public Text talkText;
    public GameObject talkPanel;
    public GameObject scanObject;
    public int talkIndex;
    public bool isAction;

    //Endless System


    //Shop

    public Shop shop;

    private int count;


    private void Awake()
        {
            DontDestroyOnLoad(gameObject);

        }
    

    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        ObjData objData = scanObject.GetComponent<ObjData>();
        Talk(objData.id,objData.isNpc);

        if(objData.id==101)
        {
            shop.Action();

        }
        else
        {
            talkPanel.SetActive(isAction);
        }
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

    void addCount()
    {
        count++;
    }
    int GetCount()
    {
        return count;
    }

}