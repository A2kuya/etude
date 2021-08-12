using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    bool isOpen;
    public Sprite close;
    public Sprite open;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isOpen = false;
    }

    public void Action(){
        if(!isOpen){
            spriteRenderer.sprite = open;
            isOpen = true;
        }else{
            GameManager.Instance.NextRound();
        }
    }
}
