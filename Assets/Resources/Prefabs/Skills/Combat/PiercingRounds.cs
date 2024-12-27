using UnityEngine;
using System.Collections.Generic;

public class Piercing : SkillBase
{
    public override string SkillName => "Piercing Rounds";
    public override string Description => "Increases the ability of projectiles to pierce through targets.";
    public override int MaxLevel => 3;

    public static readonly Dictionary<int, int> levelEffects = new Dictionary<int, int>
    {
        { 1, 1 },
        { 2, 2 },
        { 3, 3 }
    };

    public Piercing(int level) : base(level) { }

    public override void Activate()
    {
        TargetShip.OnSpawn += OnSpawn;
    }

    private int DeterminePiercingModifier()
    {
        return GetAmountAffected(Level);
    }

    private void OnSpawn()
    {
        TargetShip.PiercingModifier += DeterminePiercingModifier();
    }

    public override void Deactivate()
    {
        // Implementation for Piercing deactivation
    }

    public static int GetAmountAffected(int level)
    {
        if (levelEffects.TryGetValue(level, out int effect))
        {
            return effect;
        }
        else
        {
            Debug.LogError("Piercing Rounds level is invalid");
            return 0; // Default value or error handling
        }
    }
}