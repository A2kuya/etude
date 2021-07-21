using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokeGround : MonoBehaviour
{
    bool BossDie;
    // Start is called before the first frame update
    void Start()
    {
        BossDie=false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Break()
    {
        if(BossDie)
            this.gameObject.SetActive(false);
    }

    public void SwitchBossDie()
    {
        BossDie=!BossDie;
    }
}
