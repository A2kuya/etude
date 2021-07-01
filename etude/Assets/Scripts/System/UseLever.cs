using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseLever : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite newSprite;
    bool flag = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void ChangeSprite(bool flag)
    {
        if (flag)
        {
            spriteRenderer.sprite = newSprite;
            Transform[] cage = GetComponentsInChildren<Transform>();
            cage[1].Translate(Vector2.up * Time.deltaTime);
            cage[2].Translate(Vector2.up * Time.deltaTime);
        }
        
    }

    public void SwitchFlag()
    {
        flag = !flag;
    }




}
