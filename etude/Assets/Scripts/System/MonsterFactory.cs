using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFactory : EnemyFactory
{
    public GameObject prfHyena;
    public GameObject prfSnake;
    public GameObject prfBossHyena;
    public GameObject prfVulture;
    
    //몹 추가시 프리팹 추가 필요
    public MonsterFactory(){
        prfHyena = Resources.Load<GameObject>("Prefabs/Hyena");
        prfSnake = Resources.Load<GameObject>("Prefabs/Snake");
        prfBossHyena = Resources.Load<GameObject>("Prefabs/BossHyena");
        prfVulture = Resources.Load<GameObject>("Prefabs/Vulture");

    }
    //몹 추가시 프리팹 추가 필요
    private GameObject PrefabSet(string s){
        switch(s){
            case "Snake":
            case "sanke":
                return prfSnake;
            case "Hyena":
            case "hyena":
                return prfHyena;
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
        if(enemy == null){
            Debug.Log(s + "생성 실패");
            return null;
        }
        MonoBehaviour.Instantiate(enemy);
        return enemy;
    }
    public override GameObject CreateEnemy(string s, Vector3 position, bool isLeft = true)
    {
        GameObject enemy = PrefabSet(s);
        if(enemy == null){
            Debug.Log(s + "생성 실패");
            return null;
        }
        enemy.GetComponent<Enemy>().ConstructSet(isLeft);
        enemy.transform.position = position;
        MonoBehaviour.Instantiate(enemy);
        return enemy;
    }
    public override GameObject CreateEnemy(string s, Vector3 position, bool isLeft, int hp, int damage)
    {
        GameObject enemy = PrefabSet(s);
        if(enemy == null){
            Debug.Log(s + "생성 실패");
            return null;
        }
        enemy = Resources.Load<GameObject>("Prefabs/Hyena");
        enemy.GetComponent<Enemy>().ConstructSet(isLeft, hp, damage);
        enemy.transform.position = position;
        MonoBehaviour.Instantiate(enemy);
        return enemy;
    }
}
