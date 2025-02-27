// Data types for Desearializing JSON

using System;
using System.Collections.Generic;

public class InitialShipData
{
    public Dictionary<string, bool> Weapons { get; set; }
    public Dictionary<string, int> Skills { get; set; }
}

[Serializable]
public class EffectData
{
    public EffectType Type;
    public EffectSubType SubType;
    public ExpiryType Expiry;
    public float Duration;
    public float Amt;
    public string PickupMessage;
}

[Serializable]
public class GameData
{
    public StageData[] Stages;
}

[Serializable]
public class StageData
{
    public LevelData[] Levels;
}

[Serializable]
public class LevelData
{
    public WaveData[] Waves;
}

[Serializable]
public class WaveData
{
    public float SpawnCooldown;
    public EnemyData[] Enemies;
    public float WaveDelay;
    public List<EffectSubType> Drops;
}

[Serializable]
public class EnemyData
{
    public string ShipType;
    public int Amt;
    public string PathPreset;
}

[Serializable]
public class PathPoint
{
    public List<int> p; // Change from int to List<int>
    public float d;
    public float s;
    public int f; // New field for facing direction
    public bool sh; // New field for shooting allowed
    public bool c; // New field for curving allowed
}

[Serializable]
public class PathData
{
    public string name;
    public List<int> spawns;
    public List<PathPoint> path;
}
[Serializable]
public class EnemyPaths : Dictionary<string, List<string>> {}

public class DeterminedPath
{
    public int spawnIndex;
    public PathData pathData;
}

[Serializable]
public class Positions
{
    public List<SpawnCoordinate> spawns;
    public List<PositionCoordinate> positions;
}

[Serializable]
public class SpawnCoordinate
{
    public int id;
    public float x;
    public float y;
}

[Serializable]
public class PositionCoordinate
{
    public int id;
    public float x;
    public float y;
}