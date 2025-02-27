using UnityEngine;
using System.Collections.Generic;

public class ShieldCapacity : SkillBase
{
    public override string SkillName => "Shield Capacity";
    public override string Description => "Increases the maximum shield capacity of the ship.";
    public override int MaxLevel => 3;

    public static readonly Dictionary<int, float> levelEffects = new Dictionary<int, float>
    {
        { 1, 1.15f },
        { 2, 1.3f },
        { 3, 1.5f }
    };

    public ShieldCapacity(int level) : base(level) { }

    public override void Activate()
    {
        TargetShip.OnSpawn += OnSpawn;
    }

    private float DetermineMaxShieldModifier()
    {
        return GetAmountAffected(Level);
    }

    private void OnSpawn()
    {
        TargetShip.MaxShield *= DetermineMaxShieldModifier();
        TargetShip.Shield = TargetShip.MaxShield;
    }

    public override void Deactivate()
    {
        // Implementation for ShieldCapacity deactivation
    }

    public static float GetAmountAffected(int level)
    {
        if (levelEffects.TryGetValue(level, out float effect))
        {
            return effect;
        }
        else
        {
            Debug.LogError("Shield Capacity level is invalid");
            return 1f; // Default value or error handling
        }
    }
}