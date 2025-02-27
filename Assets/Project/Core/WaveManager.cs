using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Inst { get; private set; }
    private LevelData LevelData { get; set; }  
    private int CurrentWaveIndex { get; set; }
    private WaveData WaveData { get; set; }
    private Coroutine WaveSpawnerCoroutine { get; set; }
    private int TotalWaves { get; set; }
    public int TotalSpawnedEnemies { get; set; }
    public int TotalDestroyedEnemies { get; set; }

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;  // Ensure no further code execution in this instance
        }
        Inst = this;
    }

    public void StartWaves(LevelData levelData)
    {
        LevelData = levelData;
        TotalWaves = LevelData.Waves.Length;
        CurrentWaveIndex = 0;
        TotalSpawnedEnemies = 0;
        TotalDestroyedEnemies = 0;
        SpawnNextWave();
    }

    private void SpawnNextWave()
    {
        WaveData = LevelData.Waves[CurrentWaveIndex];
        if (WaveData.WaveDelay > 0) StartWaveCoroutine();
        else SpawnWave();
    }

    private void StartWaveCoroutine()
    {
        WaveSpawnerCoroutine = StartCoroutine(WaveSpawner());
    }

    private Wave SpawnWave()
    {
        Wave wave = Instantiate(AssetManager.WavePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        wave.Initialise(WaveData);
        return wave;
    }

    private IEnumerator WaveSpawner()
    {
        while (true)
        {
            if (CurrentWaveIndex >= TotalWaves) break;
            Wave wave = SpawnWave();
            yield return new WaitForSeconds(wave.WaveData.WaveDelay);
        }
    }

    public void HandleWaveCompleted()
    {
        CurrentWaveIndex++;

        if (TotalDestroyedEnemies >= TotalSpawnedEnemies)
        {
            StopCoroutine(WaveSpawnerCoroutine);
            if (CurrentWaveIndex >= TotalWaves) LevelManager.HandleAllWavesCompleted();
            else SpawnNextWave();
        }
    }
}