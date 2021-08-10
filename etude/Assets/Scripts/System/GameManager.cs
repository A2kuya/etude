using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public InteractManager interactManager;
    bool isPause = true;
    Player player;
    private void Start() 
    {
        count=1;
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPause){
                Time.timeScale = 0;
                isPause = false;
                player.canGetKey = false;
            }else{
                Time.timeScale = 1;
                isPause = true;
                player.canGetKey = true;
            }
        }
    }
    public void Action(GameObject scanObj)
    {
        interactManager.Action(scanObj);
    }

    //Endless System
    private int count;
    public void addCount()
    {
        count++;
    }
    public int getCount()
    {
        return count;
    }

    public void setCount(int a)
    {
        count=a;
    }
    
    

}