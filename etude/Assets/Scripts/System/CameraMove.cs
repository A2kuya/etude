using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;        // ����ٴ� Ÿ�� ������Ʈ�� Transform
    private Transform tr;                // ī�޶� �ڽ��� Transform

    Vector3 v;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        v = target.position;
        v.z = -10f;
        tr.position = v;
  
    }
}
