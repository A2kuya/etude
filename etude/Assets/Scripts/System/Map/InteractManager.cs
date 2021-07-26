using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractManager : MonoBehaviour
{

    public Text talkText;

    public int talkIndex;
    Dictionary<int, string[]> talkData;
    
    public bool isAction;
    public ShopManager shop;
     public GameObject scanObject;
     public GameObject talkPanel;


    


    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
        
    }

    void GenerateData()
    {
        talkData.Add(101, new string[] {"저는 상인입니다.","스킬포인트도 팔고 있습니다."});
        talkData.Add(102, new string[] {"가기전에 회복 받고 가라."});
    }

    public string GetTalk(int id, int talkIndex)
    {
        if(talkIndex==talkData[id].Length)
        {
            return null;
        }
        else
        {
            return talkData[id][talkIndex];
        }
    }

    void Talk(int id, bool isNpc)
    {
        string talkData= GetTalk(id, talkIndex);

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
    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        ObjData objData = scanObject.GetComponent<ObjData>();
        Talk(objData.id,objData.isNpc);

        switch(objData.id)
        {
            default:
                talkPanel.SetActive(isAction);
                break;
            case 101:
                shop.Action();
                break;
        }
    }

    
}
