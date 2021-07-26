using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFactory : EnemyFactory
{
    public GameObject prfBossHyena;
    public GameObject prfVulture;
    
    //몹 추가시 프리팹 추가 필요
    public BossFactory(){
        prfBossHyena = Resources.Load<GameObject>("Prefabs/BossHyena");
        prfVulture = Resources.Load<GameObject>("Prefabs/Vulture");

    }
    //몹 추가시 프리팹 추가 필요
    private GameObject PrefabSet(string s){
        switch(s){
            case "BossHyena":
            case "Bosshyena":
            case "bosshyena":
            case "bossHyena":
                return prfBossHyena;
            case "Vulture":
            case "vulture":
                return prfVulture;
            default:
                return null;
        }
    }
    public override GameObject CreateEnemy(string s)
    {
        GameObject enemy = PrefabSet(s);
        MonoBehaviour.Instantiate(enemy);
        return enemy;
    }
    public override GameObject CreateEnemy(string s, Vector3 position, bool isLeft = true)
    {
        GameObject enemy = PrefabSet(s);
        enemy.GetComponent<Enemy>().ConstructSet(isLeft);
        enemy.transform.position = position;
        MonoBehaviour.Instantiate(enemy);
        return enemy;
    }
    public override GameObject CreateEnemy(string s, Vector3 position, bool isLeft, int hp, int damage)
    {
        GameObject enemy = PrefabSet(s);
        enemy = Resources.Load<GameObject>("Prefabs/Hyena");
        enemy.GetComponent<Enemy>().ConstructSet(isLeft, hp, damage);
        enemy.transform.position = position;
        MonoBehaviour.Instantiate(enemy);
        return enemy;
    }
}