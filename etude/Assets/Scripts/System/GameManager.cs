using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public InteractManager interactManager;     


    //Endless System
    private int count;
    
    
    void addCount()
    {
        count++;
    }

    int getCount()
    {
        return count;
    }
    
    public void Action(GameObject scanObj)
    {
        interactManager.Action(scanObj);
    }
    

}