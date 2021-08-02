using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public RectTransform uiGroup;
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

    public void UpgradeSkill(SkillType skillType, ref int skillPoint)
    {
        int pointRequirement = (int)Mathf.Pow(2, skillLevel[(int)skillType]);
        if (skillPoint >= pointRequirement)
        {
            skillPoint -= pointRequirement;
            skillLevel[(int)skillType]++;
        }
    }

    //Ui

    public void Active()
    {
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
}
