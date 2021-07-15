using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenRoom : MonoBehaviour
{
    TilemapRenderer tr;
    public GameObject shortcut;
    Color img;
    bool flag = false;

    // Start is called before the first frame update
    void Start()
    {
        tr = shortcut.GetComponent<TilemapRenderer>();
        img = tr.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (flag)
        {
            if (img.a > 0)
                img.a = img.a - 0.01f;
        }
        else
        {
            if (img.a < 1f)
                img.a = img.a + 0.01f;
        }

        tr.material.color = img;
        

    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
            flag = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        flag = false;
    }
}
