using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemDD : MonoBehaviour
{
    public static SystemDD Instance;
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);            
        }else{
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
