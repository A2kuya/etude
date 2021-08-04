using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public Player player;
    public Text[] TextSkillTree;
    public Text TextSkillPoint;
    public RectTransform uiGroup;
    int PlayerSkillPoint=0;
    int[] tmpSkillPoint= new int[10];
    int[] tmpSkillLevel=new int[10];
    int sumtmp=0; 
    public static SkillManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SkillManager>();
                if(instance == null)
                {
                    var instanceContainer = new GameObject("SkillManager");
                    instance = instanceContainer.AddComponent<SkillManager>();
                }
            }
            return instance;
        }
    }
    private static SkillManager instance;

    public enum SkillType
    {
        Dash, SpecialAttack, DoubleJump
    }

    static private int skillTypeSize = System.Enum.GetValues(typeof(SkillType)).Length;
    public int[] skillLevel = new int[skillTypeSize];

    public bool IsSkillUnlocked(SkillType skillType, int num)
    {
        return skillLevel[(int)skillType] >= num;
    }

    public void UpgradeSkill(SkillType skillType, ref int skillPoint, int upgradeskilllevel)
    {
        for(int i=skillLevel[(int)skillType];i<skillLevel[(int)skillType]+upgradeskilllevel;i++)
        {
            skillPoint-=NeedPoint(i+1);
        }
        skillLevel[(int)skillType]+=upgradeskilllevel;
    }

    //Ui

    public void Active(ref int skillPoint)
    {
        PlayerSkillPoint=skillPoint;
        TextSkillPoint.text=PlayerSkillPoint.ToString();
        for(int i=0;i<3;i++)
            TextSkillTree[i].text=skillLevel[i].ToString();
        for(int i=0;i<3;i++)
        {
            tmpSkillLevel[i]=0;
            tmpSkillPoint[i]=0;
        }

        if(uiGroup.anchoredPosition!=Vector2.zero)
        {
            uiGroup.anchoredPosition=Vector2.zero;
        }
        else
        {
            uiGroup.anchoredPosition=Vector2.up*1000;
        }
    }

    public void Exit()
    { 
        uiGroup.anchoredPosition=Vector2.up*1000;
    }

    public void PlusSkillLevel(int i)
    {
        if(i==0 && skillLevel[i]+tmpSkillLevel[i]==10)
        {
            return;
        }
        if(PlayerSkillPoint-sumtmp>=NeedPoint(skillLevel[i]+tmpSkillLevel[i]))
        {
            tmpSkillLevel[i]++;
            tmpSkillPoint[i]+=NeedPoint(skillLevel[i]+tmpSkillLevel[i]);
            sumtmp=tmpSkillPoint[0]+tmpSkillPoint[1]+tmpSkillPoint[2];
        }
        TextSkillPoint.text=(PlayerSkillPoint-sumtmp).ToString();
        TextSkillTree[i].text=(skillLevel[i]+tmpSkillLevel[i]).ToString();
    }

    public void MinusSkillLevel(int i)
    {
        if(skillLevel[i]+tmpSkillLevel[i]>0)
        {
            tmpSkillPoint[i]-=NeedPoint(skillLevel[i]+tmpSkillLevel[i]);
            tmpSkillLevel[i]--;
            sumtmp=tmpSkillPoint[0]+tmpSkillPoint[1]+tmpSkillPoint[2];
        }
        TextSkillPoint.text=(PlayerSkillPoint-sumtmp).ToString();
        TextSkillTree[i].text=(skillLevel[i]+tmpSkillLevel[i]).ToString();
    }


    public void Complete()
    {
        UpgradeSkill(SkillType.Dash,ref player.skillPoint,tmpSkillLevel[0]);
        UpgradeSkill(SkillType.SpecialAttack, ref player.skillPoint,tmpSkillLevel[1]);
        UpgradeSkill(SkillType.DoubleJump, ref player.skillPoint, tmpSkillLevel[2]);

        Exit();
    }


    int NeedPoint(int i)
    {
        int tmp=i*i;
        return tmp;
    }

    public void PointON()
    {
        print("마우스가 위에 있군요.");
    }
}
