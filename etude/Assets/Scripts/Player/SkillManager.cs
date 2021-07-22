using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
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
            DontDestroyOnLoad(instance);
            return instance;
        }
    }
    private static SkillManager instance;

    public enum SkillType
    {
        Dash1, Dash2, SpecialAttack1,
    }

    public List<SkillType> unlockedSkillsList = new List<SkillType>();

    public void UnlockSkill(SkillType skillType)
    {
        unlockedSkillsList.Add(skillType);
    }

    public bool IsSkillUnlocked(SkillType skillType)
    {
        return unlockedSkillsList.Contains(skillType);
    }
}
