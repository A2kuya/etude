using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{

    public RectTransform uiGroup;
    public Player enterPlayer;

    bool flag;


    public void Start()
    {
        flag=false;
    }
    public void Enter(Player player)
    {
        enterPlayer=player;
        uiGroup.anchoredPosition=Vector2.zero;
    }

    // Update is called once per frame
    public void Exit()
    {
        uiGroup.anchoredPosition=Vector2.down*1000;
    }

    public void Action()
    {
        if(flag)
        {
            Exit();
        }
        else
        {
            Enter(enterPlayer);
        }

        flag=!flag;

    }

}
