using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public Player player;
    public Text[] TextSkillTree;
    public Text TextSkillPoint;
    public Text TextInformation;
    public RectTransform uiGroup;
    int PlayerSkillPoint=0;
    int[] tmpSkillPoint= new int[10];
    int[] tmpSkillLevel=new int[10];
    int sumtmp=0; 
    public GameObject Information;
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

        int NeedPoint(int i)
    {
        int tmp=i*i;
        return tmp;
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
        if(i==2 && skillLevel[i]+tmpSkillLevel[i]==1)
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
        TextInformation.text="필요 스킬 포인트: "+NeedPoint(skillLevel[i]+tmpSkillLevel[i]+1);
        if(i==0&&skillLevel[i]+tmpSkillLevel[i]+1==5)
        {
            TextInformation.text="필요 스킬 포인트: "+NeedPoint(skillLevel[i]+tmpSkillLevel[i]+1)+"\n\n이제 대쉬 중 무적이 됩니다.";
            return;
        }
    }

    public void MinusSkillLevel(int i)
    {
        if(tmpSkillLevel[i]>0)
        {
            tmpSkillPoint[i]-=NeedPoint(skillLevel[i]+tmpSkillLevel[i]);
            tmpSkillLevel[i]--;
            sumtmp=tmpSkillPoint[0]+tmpSkillPoint[1]+tmpSkillPoint[2];
        }
        TextSkillPoint.text=(PlayerSkillPoint-sumtmp).ToString();
        TextSkillTree[i].text=(skillLevel[i]+tmpSkillLevel[i]).ToString();
        TextInformation.text="회수가능 스킬 포인트: "+NeedPoint(skillLevel[i]+tmpSkillLevel[i]);
    }


    public void Complete()
    {
        UpgradeSkill(SkillType.Dash,ref player.skillPoint,tmpSkillLevel[0]);
        UpgradeSkill(SkillType.SpecialAttack, ref player.skillPoint,tmpSkillLevel[1]);
        UpgradeSkill(SkillType.DoubleJump, ref player.skillPoint, tmpSkillLevel[2]);

        Exit();
    }


    //Information UI

    public void PointONPlus(int datanum)
    {
        Information.SetActive(true);
        if(datanum==0&&skillLevel[datanum]+tmpSkillLevel[datanum]+1==5)
        {
            TextInformation.text="필요 스킬 포인트: "+NeedPoint(skillLevel[datanum]+tmpSkillLevel[datanum]+1)+"\n\n이제 대쉬 중 무적이 됩니다.";
            return;
        }
        TextInformation.text="필요 스킬 포인트: "+NeedPoint(skillLevel[datanum]+tmpSkillLevel[datanum]+1);
    }
    public void PointONMinus(int datanum)
    {
        Information.SetActive(true);
        TextInformation.text="회수가능\n 스킬 포인트: "+NeedPoint(skillLevel[datanum]+tmpSkillLevel[datanum]);
    }
    public void PointOnIcon(int datanum)
    {
        Information.SetActive(true);
        switch(datanum)
        {
            default:
            TextInformation.text="추후에 스킬이 더 추가될 수도?";
            break;
            case 0:
            TextInformation.text="빠르게 이동할 수 있는 대쉬, 단련할수록 더욱 자주 쓸 수 있다.\nMaX Level: 10";
            break;
            case 1:
            TextInformation.text="강한 한방! 계속해서 단련하면 점점 강해질 것 같은 느낌이 든다.";
            break;
            case 2:
            TextInformation.text="공중에서 한번 더 점플할 수 있게 해주는 더블 점프!\n\nMax Level: 1";
            break;
        }
    }
    public void PointOut()
    {
        Information.SetActive(false);
    }
}
