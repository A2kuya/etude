using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerForBoss : MonoBehaviour
{
    public Vulture Boss;
    public GameObject Spine;
    public GameObject Platform;

    float StartPosition;
    Transform[] platform;

    int ran;


    void Start()
    {
        StartPosition=Spine.transform.position.y;
        platform=Platform.GetComponentsInChildren<Transform>();
        ran=Random.Range(1,10);
    }
    void Update()
    {

        /*switch(Boss.state)//보스 체력이 50 이하
        {
            case Boss.:
            ActiveSpin();
            break;

            case 3:
            DeleteRandomPlatfrom();
            break;
        }*/
    }




    void ActiveSpin()
    {
        if(Spine.transform.position.y-StartPosition<2f)
                Spine.transform.Translate(Vector2.up * Time.deltaTime*2);
        
    }

    void DeleteRandomPlatfrom()
    {
        if(platform[ran].position.y>-25)
            platform[ran].Translate(Vector2.down*Time.deltaTime*20f);
    }
}
