using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowForMinimap : MonoBehaviour
{
    public Transform player;
    Vector3 tmp;
    // Start is called before the first frame update

    void Update()
    {

        tmp=player.position;
        tmp.z=-10;
        this.transform.position=tmp;
    }
}
