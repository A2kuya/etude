using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{

    bool flag=false;
    Image[] image;
    Text text;
    Color[] colorarray;

    void Start()
    {
        image=gameObject.transform.GetComponentsInChildren<Image>();
        text=gameObject.transform.GetChild(image.Length-1).GetComponent<Text>();
        colorarray= new Color[image.Length];
        for(int i=0; i<image.Length;i++)
                    colorarray[i]=image[i].color;
    }

    void Update()
    {
         if(!flag)
        {
            if (colorarray[0].a > 0)
                colorarray[0].a = colorarray[0].a - 0.05f;
            if(colorarray[1].a>-1f)
            {
                for(int i=1; i<image.Length;i++)
                {
                    colorarray[i].a-=0.1f;
                }
            }
            text.gameObject.SetActive(false);

        }
        else
        {
            
            if (colorarray[0].a < 0.6f)
                colorarray[0].a = colorarray[0].a + 0.05f;
            if(colorarray[1].a<1f)
            {
                for(int i=1; i<image.Length;i++)
                    colorarray[i].a+=0.1f;
            }
            text.gameObject.SetActive(true);
        }

        for(int i=0; i<image.Length;i++)
        {
            image[i].color=colorarray[i];
        }


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
