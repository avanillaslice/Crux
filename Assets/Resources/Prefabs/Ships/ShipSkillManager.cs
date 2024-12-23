using System.Collections.Generic;
using UnityEngine;

public static class ShipSkillFactory
{
    public class SkillList
    {
        public List<SkillBase> Skills;

        public SkillList(List<SkillBase> skills)
        {
            Skills = skills;
        }

        public void AssignShip(ShipBase targetShip)
        {
            if (Skills.Count == 0 || targetShip == null) return;

            foreach (SkillBase skill in Skills)
            {
                skill.AttemptActivation(targetShip);
            }
        }

        public SkillBase FetchSkill(string skillName)
        {
            foreach (var skill in Skills)
            {
                if (skill.SkillName == skillName)
                {
                    return skill;
                }
            }
            return null;
        }
    }

    public static SkillList BuildSkillList(InitialShipData initialShipData, ShipBase targetShip)
    {
        List<SkillBase> _skills = new List<SkillBase>();
        var skills = initialShipData.Skills;
        foreach (var skillEntry in skills)
        {
            if (skillEntry.Value == 0) continue;

            SkillBase skill = CreateSkillInstance(skillEntry.Key, skillEntry.Value);
            if (skill != null)
            {
                if (targetShip != null) skill.AttemptActivation(targetShip);
                _skills.Add(skill);
            }
        }
        return new SkillList(_skills);
    }

    private static SkillBase CreateSkillInstance(string skillName, int level)
    {
        switch (skillName)
        {
            case "ShieldCapacity":
                return new ShieldCapacity(level);
            case "HullDurability":
                return new HullDurability(level);
            case "ShieldRegen":
                return new ShieldRegen(level);
            case "Damage":
                return new Damage(level);
            case "FireRate":
                return new FireRate(level);
            case "Piercing":
                return new Piercing(level);
            case "Speed":
                return new Speed(level);
            case "Evasion":
                return new Evasion(level);
            case "CriticalHit":
                return new CriticalHit(level);
            case "ShieldDrones":
                return new ShieldDrones(level);
            case "AttackDrones":
                return new AttackDrones(level);
            case "DroneFireRate":
                return new DroneFireRate(level);
            case "DroneChargeRate":
                return new DroneChargeRate(level);
            case "AdvancedTargetting":
                return new AdvancedTargetting(level);
            case "DefensiveFormations":
                return new DefensiveFormations(level);
            default:
                Debug.LogError($"Unknown skill: {skillName}");
                return null;
        }
    }
}
