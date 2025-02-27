using UnityEngine;

public static class LevelManager
{
    private static int TotalLevels { get; set; }
    private static StageData StageData { get; set; }
    private static int CurrentLevelIndex { get; set; }

    public static void StartLevels(StageData stageData)
    {
        StageData = stageData;
        TotalLevels = StageData.Levels.Length;
        CurrentLevelIndex = 0;
        StartNextLevel();
    }

    public static void StartNextLevel()
    {
        if (!ValidateLevel(StageData.Levels[CurrentLevelIndex]))
        {
            Debug.LogError("Level validation failed. Proceeding to next level or finishing stage.");
            HandleAllWavesCompleted();
            return;
        };
        WaveManager.Inst.StartWaves(StageData.Levels[CurrentLevelIndex]);
    }

    private static bool ValidateLevel(LevelData level)
    {
        if (level.Waves == null || level.Waves.Length == 0) {
            Debug.LogError("No Waves found in Level!");
            return false;
        }
        return true;
    }

    public static void HandleAllWavesCompleted()
    {
        Debug.Log("Level Completed!");
        CurrentLevelIndex++;

        if (TotalLevels <= CurrentLevelIndex) StageManager.HandleAllLevelsCompleted();
        else StartNextLevel();
    }
}
