public class DefensiveFormations : SkillBase
{
    public DefensiveFormations(int level) : base(level)
    {
        MaxLevel = 1;
        SkillName = "DefensiveFormations";
    }

    public override void Activate()
    {
        TargetShip.OnSpawn += OnSpawn;
    }

    private void OnSpawn()
    {
        TargetShip.DefensiveFormations = true;
    }

    public override void Deactivate()
    {
        // Implementation for DefensiveFormations deactivation
    }
}