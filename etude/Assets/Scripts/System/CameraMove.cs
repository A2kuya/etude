using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;        // 따라다닐 타겟 오브젝트의 Transform
    private Transform camera;                // 카메라 자신의 Transform

    Vector3 v;
    // Start is called before the first frame update
    void Start()
    {
       camera = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(target.position.y<15f)
        {
            if((target.position.x>-3f)&&(target.position.x<136f))
                v.x = target.position.x + 2f;
        }
        else
        {
            if ((target.position.x > 54f) && (target.position.x < 111f))
                v.x = target.position.x + 2f;
        }
        v.y = target.position.y+7f;
        v.z = -10f;
        camera.position = Vector3.MoveTowards(camera.position, v, 0.3f);
  
    }
}
