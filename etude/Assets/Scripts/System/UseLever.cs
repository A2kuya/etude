using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseLever : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    bool flag = false;
    Transform[] cage;
    float StartPosition;

    // Start is called before the first frame update
    void Start()
    {
        cage = GetComponentsInChildren<Transform>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        StartPosition = cage[1].position.y;
    }

    public void ChangeSprite(bool flag)
    {
        if (flag)
        {
            spriteRenderer.sprite = newSprite;
            if (cage[1].position.y-StartPosition < 2.2)
            {
                print(cage[1].position.y);
                cage[1].Translate(Vector2.up * Time.deltaTime);
                cage[2].Translate(Vector2.up * Time.deltaTime);
            }
        }
        
    }

    public void SwitchFlag()
    {
        flag = !flag;
    }




}
