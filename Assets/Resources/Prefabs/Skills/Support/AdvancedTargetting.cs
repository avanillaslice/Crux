using UnityEngine;

public class AdvancedTargetting : SkillBase
{
    public override string SkillName => "Advanced Targetting";
    public override string Description => "Enables advanced targetting systems for improved accuracy.";
    public override int MaxLevel => 1;

    public AdvancedTargetting(int level) : base(level) { }

    public override void Activate()
    {
        TargetShip.OnSpawn += OnSpawn;
    }

    private void OnSpawn()
    {
        TargetShip.AdvancedTargetting = true;
    }

    public override void Deactivate()
    {
        // Implementation for AdvancedTargetting deactivation
    }
}