using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerForBoss : MonoBehaviour
{
    public GameObject Boss;
    public GameObject Minimap1;
    public GameObject Minimap2;
    public GameObject GroundForSpine;

    float StartPosition;

    bool flag=false;


    void Start()
    {
        Minimap1.SetActive(true);
        Minimap2.SetActive(false);
        StartPosition=GroundForSpine.transform.position.y;
        StartCoroutine(ActiveSpin());
    }
    void Update()
    {
        if(flag)
        {
            Minimap2.SetActive(true);
            Minimap1.SetActive(false);
            if(StartPosition-GroundForSpine.transform.position.y<4)
                GroundForSpine.transform.Translate(Vector3.down * Time.deltaTime);
        }
    }




    IEnumerator ActiveSpin()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            if (true)   //나중에 여기에 보스 체력 50%이하인지 확인하는 코드가 오면 됨.
            {
                flag=true;
            }
        }
        
    }
}
