using System.Collections.Generic;
using UnityEngine;

public class ShieldDrones : DroneBase
{
    public override string SkillName => "Shield Drones";
    public override string Description => "Deploys shield drones to protect the ship.";
    public override int MaxLevel => 3;

    public override Dictionary<int, int> levelEffects => new Dictionary<int, int>
    {
        { 1, 1 },
        { 2, 2 },
        { 3, 3 }
    };

    public ShieldDrones(int level) : base(level) { }

    protected override void SpawnDrone()
    {
        if (TargetShip == null) return;
        DroneShip shieldDrone = TargetShip.SpawnDrone(true);
        if (shieldDrone != null)
        {
            ActiveDrones.Add(shieldDrone);
            shieldDrone.OnDeath += () => HandleDroneDeath(shieldDrone);
        }
        else
        {
            Debug.LogError("DroneShip component not found on instantiated drone prefab");
        }
    }
}