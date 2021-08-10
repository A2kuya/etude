using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopData : ObjData
{
     public GameManager gm;
     public ShopManager shop;
     bool ShopFlag;
     bool firstFlag;

     void Start()
     {
         ShopFlag=false;
         firstFlag=true;
     }
    public override void Action(bool isAction)
    {
        
        if(firstFlag && gm.getCount()==1)
        {
            talkPanel.SetActive(isAction);
            if(isAction==false)
            {
                ShopFlag=true;
                firstFlag=false;
            }
        }
        if(ShopFlag)
        {
            shop.Action();
        }


    }
}
