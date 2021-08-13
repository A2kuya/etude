using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public Vector3[] pos;
    RectTransform rect;


    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rect.localPosition.x > pos[1].x)
            rect.localPosition = Vector3.MoveTowards(rect.localPosition, pos[1], 0.7f);
        else if (rect.localPosition.y > pos[2].y)
            rect.localPosition = Vector3.MoveTowards(rect.localPosition, pos[2], 0.7f);
        else if (rect.localPosition == pos[2])
            rect.localPosition = pos[0];
    }
}
