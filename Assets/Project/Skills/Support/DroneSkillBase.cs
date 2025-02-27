using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DroneSkillBase : SkillBase
{
    public DroneSkillBase(int level) : base(level) { }
    private bool IsSpawningInitialDrones = false;
    private bool IsSpawningReplacementDrones = false;
    private int DronesToReplace = 0;
    public List<DroneShip> ActiveDrones = new List<DroneShip>();
    private int MaxDrones;

    public abstract Dictionary<int, int> levelEffects { get; }

    public override void Activate()
    {
        TargetShip.OnSpawn += OnSpawn;
        TargetShip.OnDeath += OnDeath;
        StageManager.OnStageStart += OnStageStart;
        StageManager.OnStageCompleted += OnStageCompleted;
    }

    private IEnumerator SpawnInitialDrones()
    {
        MaxDrones = DetermineMaxDrones();
        IsSpawningInitialDrones = true;
        int dronesToSpawn = MaxDrones - ActiveDrones.Count;
        int spawnedDrones = 0;
        while (spawnedDrones < dronesToSpawn)
        {
            yield return new WaitForSeconds(3f);
            SpawnDrone();
            spawnedDrones++;
        }
        IsSpawningInitialDrones = false;
    }

    protected void HandleDroneDeath(DroneShip drone)
    {
        ActiveDrones.Remove(drone);
        if (StageManager.StageActive) {
            DronesToReplace++;
            if (IsSpawningReplacementDrones) return;
            else TargetShip.StartCoroutine(SpawnReplacementDrones());
        }
    }

    private IEnumerator SpawnReplacementDrones()
    {
        IsSpawningReplacementDrones = true;
        while (DronesToReplace > 0)
        {
            yield return new WaitForSeconds(10f);
            SpawnDrone();
            DronesToReplace--;
        }

        IsSpawningReplacementDrones = false;
    }

    protected abstract void SpawnDrone();

    private void StopSpawningDrones()
    {
        if (TargetShip != null) {
            TargetShip.StopCoroutine(SpawnInitialDrones());
            TargetShip.StopCoroutine(SpawnReplacementDrones());
        }
        IsSpawningInitialDrones = false;
        IsSpawningReplacementDrones = false;
    }

    private int DetermineMaxDrones()
    {
        return GetAmountAffected(Level);
    }

    private void OnSpawn()
    {
        TargetShip.StartCoroutine(SpawnInitialDrones());
    }

    private void OnDeath()
    {
        StopSpawningDrones();
        DestroyAllDrones();
    }

    private void OnStageCompleted()
    {
        StopSpawningDrones();
    }
    private void OnStageStart()
    {
        if (IsSpawningInitialDrones) return;
        TargetShip.StartCoroutine(SpawnInitialDrones());
    }

    private void DestroyAllDrones()
    {
        for (int i = ActiveDrones.Count - 1; i >= 0; i--)
        {
            DroneShip droneShip = ActiveDrones[i];
            if (droneShip != null)
            {
                droneShip.OnDeath -= () => HandleDroneDeath(droneShip);
                droneShip.Explode();
            }
        }
        ActiveDrones.Clear();
    }

    public override void Deactivate()
    {
        // Implementation for Drones deactivation
    }

    public int GetAmountAffected(int level)
    {
        if (levelEffects.TryGetValue(level, out int effect))
        {
            return effect;
        }
        else
        {
            Debug.LogError($"{SkillName} level is invalid");
            return 1; // Default value or error handling
        }
    }

}