using UnityEngine;
using System.Collections.Generic;

public class DroneChargeRate : SkillBase
{
    public override string SkillName => "Drone Charge Rate";
    public override string Description => "Increases the charge rate of drones.";
    public override int MaxLevel => 3;

    public static readonly Dictionary<int, float> levelEffects = new Dictionary<int, float>
    {
        { 1, 0.1f },
        { 2, 0.2f },
        { 3, 0.3f }
    };

    public DroneChargeRate(int level) : base(level) { }

    public override void Activate()
    {
        TargetShip.OnSpawn += OnSpawn;
    }

    private float DetermineDroneChargeRateModifier()
    {
        return GetAmountAffected(Level);
    }

    private void OnSpawn()
    {
        TargetShip.DroneChargeRateModifier += DetermineDroneChargeRateModifier();
    }

    public override void Deactivate()
    {
        // Implementation for DroneChargeRate deactivation
    }

    public static float GetAmountAffected(int level)
    {
        if (levelEffects.TryGetValue(level, out float effect))
        {
            return effect;
        }
        else
        {
            Debug.LogError("Drone Charge Rate level is invalid");
            return 0.1f; // Default value or error handling
        }
    }
}