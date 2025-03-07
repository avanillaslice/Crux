using System.Collections.Generic;
using UnityEngine;

namespace Project.Skills.Support
{
    public class DroneFireRate : SkillBase
    {
        public override string SkillName => "Drone Fire Rate";
        public override string Description => "Increases the fire rate of drones.";
        public override int MaxLevel => 3;

        public static readonly Dictionary<int, float> levelEffects = new Dictionary<int, float>
        {
            { 1, 0.05f },
            { 2, 0.1f },
            { 3, 0.15f }
        };

        public DroneFireRate(int level) : base(level) { }

        public override void Activate()
        {
            TargetShip.OnSpawn += OnSpawn;
        }

        private float DetermineDroneFireRateModifier()
        {
            return GetAmountAffected(Level);
        }

        private void OnSpawn()
        {
            TargetShip.DroneFireRateModifier += DetermineDroneFireRateModifier();
        }

        public override void Deactivate()
        {
            // Implementation for DroneFireRate deactivation
        }

        public static float GetAmountAffected(int level)
        {
            if (levelEffects.TryGetValue(level, out float effect))
            {
                return effect;
            }
            else
            {
                Debug.LogError("Drone Fire Rate level is invalid");
                return 0f; // Default value or error handling
            }
        }
    }
}