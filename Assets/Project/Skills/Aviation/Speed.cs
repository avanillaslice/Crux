using System.Collections.Generic;
using UnityEngine;

namespace Project.Skills.Aviation
{
    public class Speed : SkillBase
    {
        public override string SkillName => "Speed";
        public override string Description => "Increases the ship's movement speed.";
        public override int MaxLevel => 3;

        public static readonly Dictionary<int, float> levelEffects = new Dictionary<int, float>
        {
            { 1, 0.1f },
            { 2, 0.15f },
            { 3, 0.2f }
        };

        public Speed(int level) : base(level) { }

        public override void Activate()
        {
            TargetShip.OnSpawn += OnSpawn;
        }

        private float DetermineMovementSpeedModifier()
        {
            return GetAmountAffected(Level);
        }

        private void OnSpawn()
        {
            TargetShip.MovementSpeedModifier += DetermineMovementSpeedModifier();
        }

        public override void Deactivate()
        {
            // Implementation for Speed deactivation
        }

        public static float GetAmountAffected(int level)
        {
            if (levelEffects.TryGetValue(level, out float effect))
            {
                return effect;
            }
            else
            {
                Debug.LogError("Speed level is invalid");
                return 0f; // Default value or error handling
            }
        }
    }
}