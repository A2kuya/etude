using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMap : MonoBehaviour
{

    public GameObject Always;
    public GameObject Tab;
    // Start is called before the first frame update
    void Start()
    {
        Tab.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Tab))
        {
            Always.gameObject.SetActive(false);
            Tab.gameObject.SetActive(true);
        }
        else
        {
            Tab.gameObject.SetActive(false);
            Always.gameObject.SetActive(true);
        }

        
    }
}
