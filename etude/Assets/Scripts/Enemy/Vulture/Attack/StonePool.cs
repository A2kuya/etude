using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePool : MonoBehaviour
{
    public static StonePool instance;
    [SerializeField]
    private GameObject prfStone;
    Queue<Stone> pool = new Queue<Stone>();
    void Awake()
    {
        instance = this;
        Initialize(30);
    }
    void Initialize(int count){
        for(int i=0;i<count;i++){
            pool.Enqueue(CreateNew());
        }
    }
    Stone CreateNew(){
        var newObj = Instantiate(prfStone).GetComponent<Stone>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }
    public static Stone GetObject()
    {
        if (instance.pool.Count > 0)
        {
            var obj = instance.pool.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            var newObj = instance.CreateNew();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }
    public static void ReturnObject(Stone obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);
        instance.pool.Enqueue(obj);
    }


}
