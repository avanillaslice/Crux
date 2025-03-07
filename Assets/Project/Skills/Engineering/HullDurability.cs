using System.Collections.Generic;
using UnityEngine;

namespace Project.Skills.Engineering
{
    public class HullDurability : SkillBase
    {
        public override string SkillName => "Hull Durability";
        public override string Description => "Increases the maximum health of the ship.";
        public override int MaxLevel => 3;

        public static readonly Dictionary<int, float> levelEffects = new Dictionary<int, float>
        {
            { 1, 1.15f },
            { 2, 1.3f },
            { 3, 1.5f }
        };

        public HullDurability(int level) : base(level) { }

        public override void Activate()
        {
            TargetShip.OnSpawn += OnSpawn;
        }

        private float DetermineMaxHealthModifier()
        {
            return GetAmountAffected(Level);
        }

        private void OnSpawn()
        {
            TargetShip.MaxHealth *= DetermineMaxHealthModifier();
            TargetShip.Health = TargetShip.MaxHealth;
        }

        public override void Deactivate()
        {
            // Implementation for HullDurability deactivation
        }

        public static float GetAmountAffected(int level)
        {
            if (levelEffects.TryGetValue(level, out float effect))
            {
                return effect;
            }
            else
            {
                Debug.LogError("Hull Durability level is invalid");
                return 1.0f; // Default value or error handling
            }
        }
    }
}