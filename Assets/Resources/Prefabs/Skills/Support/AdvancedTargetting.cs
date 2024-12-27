public class AdvancedTargetting : SkillBase
{
    public AdvancedTargetting(int level) : base(level)
    {
        MaxLevel = 1;
        SkillName = "AdvancedTargetting";
    }

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