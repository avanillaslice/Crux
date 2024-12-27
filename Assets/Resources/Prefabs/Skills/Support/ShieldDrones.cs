using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDrones : SkillBase
{
    public override string SkillName => "Shield Drones";
    public override string Description => "Deploys shield drones to protect the ship.";
    public override int MaxLevel => 3;

    public static readonly Dictionary<int, int> levelEffects = new Dictionary<int, int>
    {
        { 1, 1 },
        { 2, 2 },
        { 3, 3 }
    };

    private List<DroneShip> ActiveDrones = new List<DroneShip>();
    private int MaxShieldDrones;

    public ShieldDrones(int level) : base(level) { }

    public override void Activate()
    {
        MaxShieldDrones = DetermineMaxShieldDrones();
        TargetShip.OnSpawn += OnSpawn;
        TargetShip.OnDeath += OnDeath;
    }

    private IEnumerator SpawnInitialDrones()
    {
        while (ActiveDrones.Count < MaxShieldDrones)
        {
            yield return new WaitForSeconds(1f);
            SpawnDrone();
        }
    }

    private void RemoveDroneFromActiveList(DroneShip drone)
    {
        ActiveDrones.Remove(drone);
        TargetShip.StartCoroutine(SpawnReplacementDrone());
    }

    private IEnumerator SpawnReplacementDrone()
    {
        yield return new WaitForSeconds(10f);

        if (ActiveDrones.Count < MaxShieldDrones)
        {
            SpawnDrone();
        }
    }

    private void SpawnDrone()
    {
        if (TargetShip == null) return;
        DroneShip newDrone = TargetShip.SpawnDrone(true);
        if (newDrone != null)
        {
            ActiveDrones.Add(newDrone);
            newDrone.OnDeath += () => RemoveDroneFromActiveList(newDrone);
        }
        else
        {
            Debug.LogError("DroneShip component not found on instantiated drone prefab");
        }
    }

    private int DetermineMaxShieldDrones()
    {
        return GetAmountAffected(Level);
    }

    private void OnSpawn()
    {
        TargetShip.StartCoroutine(SpawnInitialDrones());
    }

    private void OnDeath()
    {
        TargetShip.StopCoroutine(SpawnInitialDrones());
        TargetShip.StopCoroutine(SpawnReplacementDrone());
        DestroyAllDrones();
    }

    private void DestroyAllDrones()
    {
        for (int i = ActiveDrones.Count - 1; i >= 0; i--)
        {
            DroneShip droneShip = ActiveDrones[i];
            if (droneShip != null)
            {
                droneShip.OnDeath -= () => RemoveDroneFromActiveList(droneShip);
                droneShip.Explode();
            }
        }
        ActiveDrones.Clear();
    }

    public override void Deactivate()
    {
        // Implementation for ShieldDrones deactivation
    }

    public static int GetAmountAffected(int level)
    {
        if (levelEffects.TryGetValue(level, out int effect))
        {
            return effect;
        }
        else
        {
            Debug.LogError("Shield Drones level is invalid");
            return 0; // Default value or error handling
        }
    }
}