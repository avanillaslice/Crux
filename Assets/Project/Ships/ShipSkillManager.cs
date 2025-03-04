using System;
using System.Collections.Generic;
using Project.Core;
using Project.Skills;
using Project.Skills.Aviation;
using Project.Skills.Combat;
using Project.Skills.Engineering;
using Project.Skills.Support;
using UnityEngine;

namespace Project.Ships
{
    // Enum to list all available skills
    public enum SkillType
    {
        ShieldCapacity,
        HullDurability,
        ShieldRegen,
        Damage,
        FireRate,
        Piercing,
        Speed,
        Evasion,
        CriticalHit,
        ShieldDrones,
        AttackDrones,
        DroneFireRate,
        DroneChargeRate,
        AdvancedTargetting,
        DefensiveFormations
    }

// Factory class to create and manage ship skills
    public static class ShipSkillManager
    {
        // Dictionary to map SkillType to skill creation functions
        private static readonly Dictionary<SkillType, Func<int, SkillBase>> skillFactory = new Dictionary<SkillType, Func<int, SkillBase>>()
        {
            { SkillType.ShieldCapacity, level => new ShieldCapacity(level) },
            { SkillType.HullDurability, level => new HullDurability(level) },
            { SkillType.ShieldRegen, level => new ShieldRegen(level) },
            { SkillType.Damage, level => new Damage(level) },
            { SkillType.FireRate, level => new FireRate(level) },
            { SkillType.Piercing, level => new Piercing(level) },
            { SkillType.Speed, level => new Speed(level) },
            { SkillType.Evasion, level => new Evasion(level) },
            { SkillType.CriticalHit, level => new CriticalHit(level) },
            { SkillType.ShieldDrones, level => new ShieldDrones(level) },
            { SkillType.AttackDrones, level => new AttackDrones(level) },
            { SkillType.DroneFireRate, level => new DroneFireRate(level) },
            { SkillType.DroneChargeRate, level => new DroneChargeRate(level) },
            { SkillType.AdvancedTargetting, level => new AdvancedTargetting(level) },
            { SkillType.DefensiveFormations, level => new DefensiveFormations(level) }
        };

        // ShipSkills holds a list of skills and provides methods to assign them to a ship and fetch specific skills.
        public class ShipSkills
        {
            public Dictionary<SkillType, SkillBase> Skills;

            public ShipSkills(Dictionary<SkillType, SkillBase> skills)
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

            public SkillBase FetchSkill(SkillType skillType)
            {
                Skills.TryGetValue(skillType, out var skill);
                return skill;
            }
        }

        // BuildShipSkills creates a dictionary of skills based on the initial ship data and assigns them to the target ship if provided.
        public static ShipSkills BuildShipSkills(InitialShipData initialShipData)
        {
            Dictionary<SkillType, SkillBase> _skills = new Dictionary<SkillType, SkillBase>();

            foreach (var skillEntry in initialShipData.Skills)
            {
                if (skillEntry.Value == 0) continue;

                SkillType skillType = (SkillType)Enum.Parse(typeof(SkillType), skillEntry.Key);
                SkillBase skill = CreateSkillInstance(skillType, skillEntry.Value);
                _skills.Add(skillType, skill);
            }
            return new ShipSkills(_skills);
        }

        public static SkillBase UnlockSkill(ShipBase targetShip, SkillType skillType, int level)
        {
            try {
                SkillBase _skill = CreateSkillInstance(skillType, level);
                _skill.AttemptActivation(targetShip);
                targetShip.ActiveSkills.Skills.Add(skillType, _skill);
                return _skill;
            } catch (Exception e) {
                Debug.LogError($"Failed to unlock skill {skillType}: {e.Message}");
                return null;
            }
        }

        // CreateSkillInstance creates an instance of a skill based on the SkillType and level.
        private static SkillBase CreateSkillInstance(SkillType skillType, int level)
        {
            if (skillFactory.TryGetValue(skillType, out var createSkill))
            {
                return createSkill(level);
            }
            else
            {
                Debug.LogError($"Unknown skill type: {skillType}");
                return null;
            }
        }
    }
}