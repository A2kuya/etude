using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerForBoss : MonoBehaviour
{
    public Vulture Boss;
    public GameObject Spine;
    public GameObject Platform;
    public GameObject exit;

    float StartPosition;
    Transform[] platform;

    int ran;


    void Start()
    {
        int count = GameManager.Instance.getCount();
        StartPosition=Spine.transform.position.y;
        platform=Platform.GetComponentsInChildren<Transform>();
        ran=Random.Range(1,10);
        Boss = GameManager.Instance.bossFactory.CreateEnemy("Vulture", new Vector3(34, 16, 0), false, 100 * count, 10 * count, 30 * count).GetComponent<Vulture>();
        exit.SetActive(false);
    }
    void Update()
    {
        switch((int)Boss.state)//보스 체력이 50 이하
        {
            case 2:
            ActiveSpin();
            break;

            case 3:
            DeleteRandomPlatfrom();
            break;
        }

        if(Boss.CompareTag("Untagged")){
            exit.SetActive(true);
        }
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
