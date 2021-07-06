using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveBarricade : MonoBehaviour
{
    Transform Barri;
    float StartPosition;
    bool flag=true;

    // Start is called before the first frame update
    void Start()
    {
        Barri = GetComponent<Transform>();
        StartPosition = Barri.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (flag)
            Deactivate();
        else
            Active();
    }

    void Active()
    {
        if(StartPosition - Barri.position.x < 4)
            Barri.Translate(Vector3.right * Time.deltaTime);
    }

    void Deactivate()
    {
        if (StartPosition - Barri.position.x < 4)
            Barri.Translate(Vector3.left * Time.deltaTime*2);
    }


    public void SwitchFlag()
    {
        flag = !flag;
    }
}
