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
        GameObject prfEnemy = PrefabSet(s);
        if(prfEnemy == null){
            Debug.Log(s + "생성 실패");
            return null;
        }
        GameObject enemy = MonoBehaviour.Instantiate(prfEnemy);
        return enemy;
    }
    public override GameObject CreateEnemy(string s, Vector3 position, bool isLeft = true)
    {
        GameObject prfEnemy = PrefabSet(s);
        if(prfEnemy == null){
            Debug.Log(s + "생성 실패");
            return null;
        }
        GameObject enemy = MonoBehaviour.Instantiate(prfEnemy);
        enemy.GetComponent<Enemy>().ConstructSet(isLeft);
        enemy.transform.position = position;
        return enemy;

    }
    public override GameObject CreateEnemy(string s, Vector3 position, bool isLeft, int hp, int damage, int price)
    {
        GameObject prfEnemy = PrefabSet(s);
        if(prfEnemy == null){
            Debug.Log(s + "생성 실패");
            return null;
        }
        GameObject enemy = MonoBehaviour.Instantiate(prfEnemy);
        enemy.GetComponent<Enemy>().ConstructSet(isLeft, hp, damage, price);
        enemy.transform.position = position;
        return enemy;
    }
    public override GameObject CreateEnemy(string s, Transform transform){
        GameObject prfEnemy = PrefabSet(s);
        if(prfEnemy == null){
            Debug.Log(s + "생성 실패");
            return null;
        }
        GameObject enemy = MonoBehaviour.Instantiate(prfEnemy);
        enemy.transform.SetParent(transform);
        return enemy;
    }
    public override GameObject CreateEnemy(string s, Transform transform, Vector3 position, bool isLeft){
        GameObject prfEnemy = PrefabSet(s);
        if(prfEnemy == null){
            Debug.Log(s + "생성 실패");
            return null;
        }
        GameObject enemy = MonoBehaviour.Instantiate(prfEnemy);
        enemy.GetComponent<Enemy>().ConstructSet(isLeft);
        enemy.transform.position = position;
        enemy.transform.SetParent(transform);
        return enemy;
    }
    public override GameObject CreateEnemy(string s, Transform transform, Vector3 position, bool isLeft, int hp, int damage, int price){
        GameObject prfEnemy = PrefabSet(s);
        if(prfEnemy == null){
            Debug.Log(s + "생성 실패");
            return null;
        }
        GameObject enemy = MonoBehaviour.Instantiate(prfEnemy);
        enemy.GetComponent<Enemy>().ConstructSet(isLeft, hp, damage, price);
        enemy.transform.position = position;
        enemy.transform.SetParent(transform);
        return enemy;
    }
}