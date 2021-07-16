using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == 6)
        {
                col.gameObject.GetComponent<Player>().TakeDamage(10, 0, this.transform.position,Vector2.right);
        }


    }

}
