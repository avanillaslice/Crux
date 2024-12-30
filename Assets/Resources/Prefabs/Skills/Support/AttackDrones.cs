using System.Collections.Generic;
using UnityEngine;

public class AttackDrones : DroneBase
{
    public override string SkillName => "Attack Drones";
    public override string Description => "Deploys attack drones to assist in combat.";
    public override int MaxLevel => 3;

    public override Dictionary<int, int> levelEffects => new Dictionary<int, int>
    {
        { 1, 1 },
        { 2, 2 },
        { 3, 3 }
    };

    public AttackDrones(int level) : base(level) { }

    protected override void SpawnDrone()
    {
        if (TargetShip == null) return;
        DroneShip attackDrone = TargetShip.SpawnDrone(false);
        if (attackDrone != null)
        {
            ActiveDrones.Add(attackDrone);
            attackDrone.OnDeath += () => HandleDroneDeath(attackDrone);
        }
        else
        {
            Debug.LogError("DroneShip component not found on instantiated drone prefab");
        }
    }
}