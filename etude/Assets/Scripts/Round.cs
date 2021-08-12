using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Round : MonoBehaviour
{
    RectTransform rectTransform;
    public Text text;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start() {
        text.text = GameManager.Instance.getCount() + "회차";
        if(GameManager.Instance.GetNewRound()){
            StartCoroutine(PopUp());
        }
    }


    IEnumerator PopUp(){
        GameManager.Instance.SetNewRoundFalse();
        Vector2 start = new Vector2(0, 0);
        Vector2 end = new Vector2(-150, 250);
        Vector2 startSize = new Vector2(700, 300);
        Vector2 endSize = new Vector2(140, 60);
        rectTransform.anchoredPosition = new Vector3(0,0,0);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 700);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 300);
        text.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,0,0);
        text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 700);
        text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 300);
        text.fontSize = 80;
        yield return new WaitForSeconds(1);
        iTween.ValueTo(gameObject, iTween.Hash("from", start, "to", end, "onupdate", "OnUIMove", "time", 1));
        iTween.ValueTo(gameObject, iTween.Hash("from", startSize, "to", endSize, "onupdate", "OnUISize", "time", 1));
    }
    void OnUIMove(Vector2 targetPosition){
        rectTransform.anchoredPosition = targetPosition;
    }
    void OnUISize(Vector2 size){
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        text.fontSize = (int)(size.x * 80 / 700);
    }
}
