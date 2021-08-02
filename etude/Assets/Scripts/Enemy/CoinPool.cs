using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
    // Start is called before the first frame update
    public static CoinPool instance;
    
    [SerializeField] private GameObject prfCoin;
    [SerializeField] private int price;
    Queue<Coin> pool = new Queue<Coin>();
    void Awake()
    {
        instance = this;
        Initialize();
    }
    void Initialize(){
        for(int i=0;i<price;i++){
            pool.Enqueue(CreateNew());
        }
    }
    Coin CreateNew(){
        var newObj = Instantiate(prfCoin).GetComponent<Coin>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }
    public static Coin GetObject(Vector2 position, Vector2 v)
    {
        if (instance.pool.Count > 0)
        {
            var obj = instance.pool.Dequeue();
            obj.transform.SetParent(null);
            obj.transform.position = position;
            obj.gameObject.SetActive(true);
            obj.GetComponent<Rigidbody2D>().AddForce(v);
            return obj;
        }
        else
        {
            var newObj = instance.CreateNew();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            newObj.GetComponent<Rigidbody2D>().AddForce(v);
            return newObj;
        }
    }
    public static void ReturnObject(Coin obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);
        instance.pool.Enqueue(obj);
    }
}
