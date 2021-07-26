using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{

    public RectTransform uiGroup;
    public GameObject Popup;
    public Player enterPlayer;
    bool flag;
    float PopupDelay=0.8f;


    public void Start()
    {
        flag=false;
    }


    public void Enter()
    {
        uiGroup.anchoredPosition=Vector2.zero;
    }
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
            Enter();
        }

        flag=!flag;

    }

    public void Buy(int index)
    {
        switch(index)
        {
            case 1:
                if(enterPlayer.money>=10)
                {
                    enterPlayer.SpendMoney(10);
                    enterPlayer.skillpoint++;
                }
                else
                {
                    Popup.SetActive(true);
                    Invoke("ExitPopup",PopupDelay);
                }
                break;
            case 2:
                if(enterPlayer.money>=100)
                {
                    enterPlayer.SpendMoney(100);
                    enterPlayer.skillpoint+=10;
                }
                else
                {
                    Popup.SetActive(true);
                    Invoke("ExitPopup",PopupDelay);
                }
                break;
            case 3:
                if(enterPlayer.money>=10)
                {
                    enterPlayer.SpendMoney(10);
                    enterPlayer.potions++;
                }
                else
                {
                    Popup.SetActive(true);
                    Invoke("ExitPopup",PopupDelay);
                }
                break;
        }
    }

    void ExitPopup()
    {
        Popup.SetActive(false);
    }

}