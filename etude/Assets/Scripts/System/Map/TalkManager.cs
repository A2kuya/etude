using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    // Start is called before the first frame update
    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        GenerateData();
        
    }

    // Update is called once per frame
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
}
