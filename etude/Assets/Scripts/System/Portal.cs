using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    bool flag;
    // Start is called before the first frame update
    void Start()
    {
        flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        print(flag);
        if (flag)
            SceneManager.LoadScene("Boss");
    }



    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
            flag = true;
    }
}
