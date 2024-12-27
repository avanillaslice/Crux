using UnityEngine;
using System.Collections.Generic;

public class Evasion : SkillBase
{
    public override string SkillName => "Evasion";
    public override string Description => "Increases the chance to evade enemy attacks.";
    public override int MaxLevel => 3;

    public static readonly Dictionary<int, float> levelEffects = new Dictionary<int, float>
    {
        { 1, 0.1f },
        { 2, 0.15f },
        { 3, 0.2f }
    };

    public Evasion(int level) : base(level) { }

    public override void Activate()
    {
        TargetShip.OnSpawn += OnSpawn;
    }

    private float DetermineEvasionModifier()
    {
        return GetAmountAffected(Level);
    }

    private void OnSpawn()
    {
        TargetShip.EvasionChanceModifier += DetermineEvasionModifier();
    }

    public override void Deactivate()
    {
        // Implementation for Evasion deactivation
    }

    public static float GetAmountAffected(int level)
    {
        if (levelEffects.TryGetValue(level, out float effect))
        {
            return effect;
        }
        else
        {
            Debug.LogError("Evasion level is invalid");
            return 0f; // Default value or error handling
        }
    }
}