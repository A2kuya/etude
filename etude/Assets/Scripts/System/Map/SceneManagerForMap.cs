using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerForMap : MonoBehaviour
{

    public GameObject Boss;
    public GameObject Ground;
    public ActiveBarricade barri;
    bool firstmeet;
    // Start is called before the first frame update

    bool flag;
    void Start()
    {
        firstmeet=false;
        Boss.SetActive(false);
        flag=true;
    }

    // Update is called once per frame
    void Update()
    {
        barri.Active();

        if(Boss.GetComponent<BossHyena>().isDead&&flag)
        {
            barri.SwitchFlag();
            Ground.GetComponent<BrokeGround>().SwitchBossDie();

            flag=false;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer==6)
        {
            if(!firstmeet)
            {
                barri.SwitchFlag();
                Boss.SetActive(true);
                firstmeet=true;
            }   
        }
    }
}
