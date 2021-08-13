using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerForMap : MonoBehaviour
{

    public GameObject Boss;
    public GameObject Ground;
    public GameObject Shop;
    public ActiveBarricade barri;
    MonsterFactory monsterFactory;
    BossFactory bossFactory;
    bool firstmeet;
    // Start is called before the first frame update

    bool flag;
    void Start()
    {
        int count = GameManager.Instance.getCount();
        firstmeet=false;
        flag=true;
        GameManager.Instance.monsterFactory.CreateEnemy("snake", new Vector3(45, 3, 0), false, 100 * count, 10 * count, 3 * count);
        GameManager.Instance.monsterFactory.CreateEnemy("snake", new Vector3(70, 11, 0), true, 100 * count, 10 * count, 3 * count);
        GameManager.Instance.monsterFactory.CreateEnemy("hyena", new Vector3(117, 3, 0), false, 100 * count, 10 * count, 5 * count);
        GameManager.Instance.monsterFactory.CreateEnemy("hyena", new Vector3(133, 5, 0), true, 100 * count, 10 * count, 5 * count);
        Boss = GameManager.Instance.bossFactory.CreateEnemy("BossHyena", new Vector3(115, 34, 0), true, 100 * count, 10 * count, 5 * count);
        Boss.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        barri.Active();

        if(Boss.GetComponent<BossHyena>().isDead&&flag)
        {
            barri.SwitchFlag();
            Ground.GetComponent<BrokeGround>().SwitchBossDie();
            Shop.SetActive(true);

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
                Shop.SetActive(false);
                firstmeet=true;
            }   
        }
    }
}
