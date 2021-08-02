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
    int[] tmp= new int[10];
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

    public List<SkillType> unlockedSkillsList = new List<SkillType>();

    public void UnlockSkill(SkillType skillType)
    {
        unlockedSkillsList.Add(skillType);
    }

    public bool IsSkillUnlocked(SkillType skillType)
    {
        return unlockedSkillsList.Contains(skillType);
    }

    public void UpgradeSkill(SkillType skillType, ref int skillPoint, int upgradeskilllevel)
    {
        skillLevel[(int)skillType]+=upgradeskilllevel;
        skillPoint-=upgradeskilllevel;
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
            tmp[i]=0;
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
        if(PlayerSkillPoint-sumtmp>0)
        {
            tmp[i]++;
            sumtmp=tmp[0]+tmp[1]+tmp[2];
        }
        TextSkillPoint.text=(PlayerSkillPoint-sumtmp).ToString();
        TextSkillTree[i].text=(skillLevel[i]+tmp[i]).ToString();
    }

    public void MinusSkillLevel(int i)
    {
        if(skillLevel[i]+tmp[i]>0)
        {
            tmp[i]--;
            sumtmp=tmp[0]+tmp[1]+tmp[2];
        }
        TextSkillPoint.text=(PlayerSkillPoint-sumtmp).ToString();
        TextSkillTree[i].text=(skillLevel[i]+tmp[i]).ToString();
    }


    public void Complete()
    {
        UpgradeSkill(SkillType.Dash,ref player.skillPoint,tmp[0]);
        UpgradeSkill(SkillType.SpecialAttack, ref player.skillPoint,tmp[1]);
        UpgradeSkill(SkillType.DoubleJump, ref player.skillPoint, tmp[2]);

        Exit();
    }




}
