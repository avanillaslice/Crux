using UnityEngine;
using System.Collections.Generic;

public class CriticalHit : SkillBase
{
    public override string SkillName => "Critical Hit";
    public override string Description => "Increases the chance of dealing critical damage.";
    public override int MaxLevel => 3;

    private static readonly Dictionary<int, float> levelEffects = new Dictionary<int, float>
    {
        { 1, 0.1f },
        { 2, 0.15f },
        { 3, 0.2f }
    };

    public CriticalHit(int level) : base(level) { }

    public override void Activate()
    {
        TargetShip.OnSpawn += OnSpawn;
    }

    private float DetermineCriticalHitModifier()
    {
        return GetAmountAffected(Level);
    }

    private void OnSpawn()
    {
        TargetShip.CriticalHitChanceModifier += DetermineCriticalHitModifier();
    }

    public override void Deactivate()
    {
        // Implementation for CriticalHit deactivation
    }

    public static float GetAmountAffected(int level)
    {
        if (levelEffects.TryGetValue(level, out float effect))
        {
            return effect;
        }
        else
        {
            Debug.LogError("Critical Hit level is invalid");
            return 0f; // Default value or error handling
        }
    }
}