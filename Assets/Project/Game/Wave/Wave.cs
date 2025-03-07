using System.Collections;
using System.Collections.Generic;
using Project.Core;
using Project.Ships;
using UnityEngine;

namespace Project.Game.Wave
{
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
                // Choose random path preset here if not hardcoded
                var pathPreset = enemy.PathPreset ?? EnemyMovementManager.FetchValidPathPreset(enemy.ShipType);
                Debug.Log($"{enemy.ShipType} - {pathPreset}");
                for (var i = 0; i < enemy.Amt; i++)
                {
                    SpawnQueue.Enqueue(new SpawnItem { ShipType = enemy.ShipType, PathPreset = pathPreset });
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
                var nextSpawn = SpawnQueue.Dequeue();
                SpawnShip(nextSpawn);
                yield return new WaitForSeconds(WaveData.SpawnCooldown);
            }
        }

        private void SpawnShip(SpawnItem nextSpawn)
        {
            var shipPrefab = AssetManager.GetEnemyShipPrefab(nextSpawn.ShipType);
            if (shipPrefab)
            {
                var ship = Instantiate(shipPrefab, new Vector3(0, 8, 10), Quaternion.Euler(0, 0, 180));
                ship.OnDestroyed += OnEnemyDestroyed;
                WaveManager.Inst.TotalSpawnedEnemies++;

                // Fetch the EnemyShipMovement script and call InitialiseMovement
                var shipMovement = ship.GetComponent<EnemyShipMovement>();
                if (shipMovement)
                {
                    shipMovement.InitialiseMovement(nextSpawn.PathPreset);
                }
                else
                {
                    Debug.LogError("EnemyShipMovement script not found on the instantiated ship.");
                }

                TotalSpawned++;

                if (WaveData.Drops.Count <= 0 || TotalSpawned != TotalEnemies) return;
                var enemyShip = ship.GetComponent<EnemyShip>();
                if (!enemyShip) return;
                var effectData = GameConfig.FetchEffectDataBySubType(WaveData.Drops[Random.Range(0, WaveData.Drops.Count)]);
                enemyShip.AssignItemDrop(effectData);
            }
            else
            {
                Debug.LogError("Failed to load EnemyShipPrefab!");
            }
        }

        private void OnEnemyDestroyed(EnemyShip destroyedShip)
        {
            if (GameManager.SceneIsChanging) return;
            destroyedShip.OnDestroyed -= OnEnemyDestroyed;
            WaveManager.Inst.TotalDestroyedEnemies++;
            TotalDestroyed++;
            if (TotalDestroyed >= TotalEnemies) EndWave();
        }

        private void EndWave()
        {
            StopCoroutine(SpawnCoroutine);
            WaveManager.Inst.HandleWaveCompleted();
            Destroy(gameObject);
        }
    }
}