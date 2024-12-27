using System.Collections.Generic;
using UnityEngine;

// Factory class to create and manage ship skills
public static class ShipSkillManager
{
    // ShipSkills holds a list of skills and provides methods to assign them to a ship and fetch specific skills.
    public class ShipSkills
    {
        public Dictionary<string, SkillBase> Skills;

        public ShipSkills(Dictionary<string, SkillBase> skills)
        {
            Skills = skills;
        }

        public void AssignShip(ShipBase targetShip)
        {
            if (Skills.Count == 0 || targetShip == null) return;

            targetShip.ActiveSkills = this;

            foreach (var skill in Skills.Values)
            {
                skill.AttemptActivation(targetShip);
            }
        }

        public SkillBase FetchSkill(string skillName)
        {
            Skills.TryGetValue(skillName, out var skill);
            return skill;
        }
    }

    // BuildShipSkills creates a dictionary of skills based on the initial ship data and assigns them to the target ship if provided.
    public static ShipSkills BuildShipSkills(InitialShipData initialShipData, ShipBase targetShip)
    {
        if (targetShip == null) return null;

        Dictionary<string, SkillBase> _skills = new Dictionary<string, SkillBase>();

        foreach (var skillEntry in initialShipData.Skills)
        {
            if (skillEntry.Value == 0) continue;

            SkillBase skill = CreateSkillInstance(skillEntry.Key, skillEntry.Value);
            _skills.Add(skillEntry.Key, skill);
            skill.AttemptActivation(targetShip);
           
        }
        return new ShipSkills(_skills);
    }

    public static void UnlockSkill(ShipBase targetShip, SkillBase skill, int level) {
        SkillBase _skill = CreateSkillInstance(skill.SkillName, level);
        _skill.AttemptActivation(targetShip);
        targetShip.ActiveSkills.Skills.Add(skill.SkillName, _skill);
    }

    // CreateSkillInstance creates an instance of a skill based on the skill name and level.
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
