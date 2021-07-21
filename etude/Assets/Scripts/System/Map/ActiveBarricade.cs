using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveBarricade : MonoBehaviour
{
    public BossHyena Boss;

    Transform Barri;
    float StartPosition;
    bool flag;


    void Start()
    {
        Barri = GetComponent<Transform>();
        StartPosition = Barri.position.x;
        flag=true;
    }


    public void Active()
    {
        if(flag)
        {
            if(Barri.position.x - StartPosition < 4)
                Barri.Translate(Vector3.right * Time.deltaTime);
        }
        else
        {
            if (StartPosition - Barri.position.x < 0)
                Barri.Translate(Vector3.left * Time.deltaTime*2);
        }
    }


    public void SwitchFlag()
    {
        flag = !flag;
    }

    public bool getflag()
    {
        return flag;
    }
}
