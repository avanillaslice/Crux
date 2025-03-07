namespace Project.Skills.Support
{
    public class DefensiveFormations : SkillBase
    {
        public override string SkillName => "Defensive Formations";
        public override string Description => "Enables defensive formations for increased protection.";
        public override int MaxLevel => 1;

        public DefensiveFormations(int level) : base(level) { }

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
}