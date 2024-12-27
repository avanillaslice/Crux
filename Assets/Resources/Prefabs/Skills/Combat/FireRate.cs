using UnityEngine;
using System.Collections.Generic;

public class FireRate : SkillBase
{
    public override string SkillName => "Fire Rate";
    public override string Description => "Increases the rate of fire for the ship's weapons.";
    public override int MaxLevel => 3;

    private static readonly Dictionary<int, float> levelEffects = new Dictionary<int, float>
    {
        { 1, 0.05f },
        { 2, 0.1f },
        { 3, 0.15f }
    };

    public FireRate(int level) : base(level) { }

    public override void Activate()
    {
        TargetShip.OnSpawn += OnSpawn;
    }

    private float DetermineFireRateModifier()
    {
        return GetAmountAffected(Level);
    }

    private void OnSpawn()
    {
        TargetShip.FireRateModifier += DetermineFireRateModifier();
    }

    public override void Deactivate()
    {
        // Implementation for FireRate deactivation
    }

    public static float GetAmountAffected(int level)
    {
        if (levelEffects.TryGetValue(level, out float effect))
        {
            return effect;
        }
        else
        {
            Debug.LogError("Fire Rate level is invalid");
            return 0f; // Default value or error handling
        }
    }
}