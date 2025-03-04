using Project.Core;
using Project.Ships;

namespace Project.Skills.Support
{
    public class AdvancedTargetting : SkillBase
    {
        public override string SkillName => "Advanced Targetting";
        public override string Description => "Enables advanced targetting systems for improved accuracy.";
        public override int MaxLevel => 1;

        public AdvancedTargetting(int level) : base(level) { }

        public override void Activate()
        {
            TargetShip.OnSpawn += OnSpawn;
            StageManager.OnStageStart += OnStageStart;
        }

        private void OnSpawn()
        {
            TargetShip.AdvancedTargetting = true;
        }

        private void OnStageStart()
        {
            if (!TargetShip.AdvancedTargetting) {
                TargetShip.AdvancedTargetting = true;
                AttackDrones attackDroneSkill = (AttackDrones)TargetShip.ActiveSkills.FetchSkill(SkillType.AttackDrones);
                if (attackDroneSkill != null) {
                    foreach (DroneShip drone in attackDroneSkill.ActiveDrones) {
                        drone.AdvancedTargetting = true;
                    }
                }
            };
        }

        public override void Deactivate()
        {
            // Implementation for AdvancedTargetting deactivation
        }
    }
}