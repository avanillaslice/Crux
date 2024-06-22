using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wave : MonoBehaviour
{
    public WaveData WaveData { get; set; }
    private Coroutine SpawnCoroutine { get; set; }
    private int TotalDestroyed { get; set; }
    private int TotalEnemies { get; set; }
    private int TotalSpawned { get; set; }

    private Queue<SpawnItem> SpawnQueue { get; set; }
    private class SpawnItem
    {
        public string ShipType { get; set; }
        public int PathIndex { get; set; }
        public string PathPreset { get; set; }
    }

    public void Initialise(WaveData waveData)
    {
        WaveData = waveData;
        SetupQueue();
        TotalEnemies = SpawnQueue.Count;
        StartSpawnCoroutine();
    }

    private void SetupQueue()
    {
        SpawnQueue = new Queue<SpawnItem>();
        foreach (var enemy in WaveData.Enemies)
        {
            for (int i = 0; i < enemy.Amt; i++)
            {
                SpawnQueue.Enqueue(new SpawnItem { ShipType = enemy.ShipType, PathIndex = enemy.PathIndex, PathPreset = enemy.PathPreset });
            }
        }
    }

    private void StartSpawnCoroutine()
    {
        SpawnCoroutine = StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (SpawnQueue.Count > 0)
        {
            SpawnItem nextSpawn = SpawnQueue.Dequeue();
            SpawnShip(nextSpawn);
            yield return new WaitForSeconds(WaveData.SpawnCooldown);
        }
    }

    private void SpawnShip(SpawnItem nextSpawn)
    {
        EnemyShip shipPrefab = AssetManager.GetEnemyShipPrefab(nextSpawn.ShipType);
        if (shipPrefab)
        {
            EnemyShip ship = Instantiate(shipPrefab, new Vector3(0, 8, 10), Quaternion.Euler(0, 0, 180));
            ship.OnDestroyed += OnEnemyDestroyed;
            WaveManager.Inst.TotalSpawnedEnemies++;

            // Fetch the EnemyShipMovement script and call InitialiseMovement
            EnemyShipMovement shipMovement = ship.GetComponent<EnemyShipMovement>();
            if (shipMovement != null)
            {
                shipMovement.InitialiseMovement(nextSpawn.PathIndex, nextSpawn.PathPreset);
            }
            else
            {
                Debug.LogError("EnemyShipMovement script not found on the instantiated ship.");
            }

            TotalSpawned++;

            if (WaveData.Drops.Count > 0 && TotalSpawned == TotalEnemies)
            {
                EnemyShip enemyShip = ship.GetComponent<EnemyShip>();
                if (enemyShip != null)
                {
                    EffectData effectData = GameConfig.FetchEffectDataBySubType(WaveData.Drops[Random.Range(0, WaveData.Drops.Count)]);
                    enemyShip.AssignItemDrop(effectData);
                }
            }
        }
        else
        {
            Debug.LogError("Failed to load EnemyShipPrefab!");
        }
    }

    private void OnEnemyDestroyed(EnemyShip destroyedShip)
    {
        destroyedShip.OnDestroyed -= OnEnemyDestroyed;
        WaveManager.Inst.TotalDestroyedEnemies++;
        TotalDestroyed++;
        if (TotalDestroyed >= TotalEnemies) EndWave();
    }

    private void EndWave()
    {
        StopCoroutine(SpawnCoroutine); // If this has issues with being null, the while loop was falsy and cleaned this up
        WaveManager.Inst.HandleWaveCompleted();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        StopAllCoroutines();  // Ensure all coroutines are stopped when the object is destroyed
    }
}