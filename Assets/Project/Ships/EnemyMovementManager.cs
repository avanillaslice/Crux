using System.Collections.Generic;
using UnityEngine;

public static class EnemyMovementManager
{
    private static Dictionary<string, int> LastUsedSpawnIndexes = new Dictionary<string, int>();

    public static DeterminedPath GetPathData(string shipType, string pathPreset = null)
    {
        PathData pathData;
        int spawnIndex;

        // If null, set to...
        // This should never run because it will cause a singular batch of enemies to use different paths entirely
        pathPreset ??= FetchValidPathPreset(shipType);

        if (GameConfig.EnemyPathPresets.TryGetValue(pathPreset, out var pathPresetData))
        {
            pathData = pathPresetData;
        }
        else
        {
            Debug.LogError($"Invalid PathPreset: {pathPreset}");
            return null;
        }

        spawnIndex = DetermineSpawnIndex(pathPreset, pathData.spawns);

        return new DeterminedPath
        {
            spawnIndex = spawnIndex,
            pathData = pathData
        };
    }

    public static string FetchValidPathPreset(string shipType)
    {
        if (!GameConfig.EnemyPaths.TryGetValue(shipType, out var pathList))
        {
            Debug.LogError($"No path data found for ship type: {shipType}");
            return null;
        }
    
        if (pathList == null || pathList.Count == 0)
        {
            Debug.LogError($"Path list is empty for ship type: {shipType}");
            return null;
        }
    
        return pathList[Random.Range(0, pathList.Count)];
    }


    private static int DetermineSpawnIndex(string pathPreset, List<int> spawnList)
    {
        if (!LastUsedSpawnIndexes.ContainsKey(pathPreset))
        {
            LastUsedSpawnIndexes[pathPreset] = 0;
        }

        int lastIndex = LastUsedSpawnIndexes[pathPreset];
        int nextIndex = (lastIndex + 1) % spawnList.Count;

        LastUsedSpawnIndexes[pathPreset] = nextIndex;

        int spawnIndex = spawnList[nextIndex];
        return spawnIndex;
    }
}
