using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public static class GameConfig
{
    public static bool HasBeenLoaded { get; private set; }

    // GAME DATA
    public static GameData GameData { get; private set; }
    public static EnemyPaths EnemyPaths { get; private set; }
    public static Positions Positions { get; private set; }
    public static Dictionary<string, PathData> EnemyPathPresets { get; private set; } = new Dictionary<string, PathData>();
    public static List<EffectData> EffectDataList = new List<EffectData>();

    // CONFIG
    public static int InitialLives = 3;
    public static int InitialScore = 0;
    public static float RespawnTimer = 2;
    public static bool CameraMovementEnabled = false;

    public static void Initialise()
    {
        LoadGameData();
        LoadEnemyPaths();
        LoadEnemyPathPresets();
        LoadPositions();
        LoadEffectData();
        AssetManager.CacheAssets();
        HasBeenLoaded = true;
        Debug.Log("Game Config Loaded");
    }

    public static EffectData FetchEffectDataBySubType(EffectSubType subType)
    {
        return EffectDataList.Find(v => v.SubType == subType);
    }

    public static InitialShipData GetInitialPlayerData()
    {
        TextAsset yamlData = Resources.Load<TextAsset>("initialPlayerData");
        if (yamlData == null)
        {
            Debug.LogError("Failed to load initial player data!");
            return null;
        }

        Debug.Log("Initial player data loaded successfully");

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<InitialShipData>(yamlData.text);
    }


    private static void LoadEffectData()
    {
        TextAsset yamlData = Resources.Load<TextAsset>("effectData");
        if (yamlData == null)
        {
            Debug.LogError("Failed to load effect data!");
            return;
        }

        Debug.Log("Effect data loaded successfully");

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        EffectDataList = deserializer.Deserialize<List<EffectData>>(yamlData.text);

        if (EffectDataList == null || EffectDataList.Count == 0)
        {
            Debug.LogError("Failed to deserialize EffectData.");
            return;
        }
    }

    private static void LoadGameData()
    {
        TextAsset yamlData = Resources.Load<TextAsset>("gameData");
        if (yamlData == null)
        {
            Debug.LogError("Failed to load game data!");
            return;
        }

        Debug.Log("Game data loaded successfully");

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        GameData = deserializer.Deserialize<GameData>(yamlData.text);
    }

    private static void LoadEnemyPaths()
    {
        TextAsset yamlData = Resources.Load<TextAsset>("enemyPaths");
        if (yamlData == null)
        {
            Debug.LogError("Failed to load enemy paths data!");
            return;
        }
    
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
    
        EnemyPaths = deserializer.Deserialize<EnemyPaths>(yamlData.text);
    
        if (EnemyPaths == null)
        {
            Debug.LogError("Failed to deserialize EnemyPaths.");
        }
    }


    private static void LoadEnemyPathPresets()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("enemyPathPresets");
        if (jsonData == null)
        {
            Debug.LogError("Failed to load enemy path presets data!");
            return;
        }
        List<PathData> pathPresets = JsonConvert.DeserializeObject<List<PathData>>(jsonData.text);

        foreach (PathData preset in pathPresets)
        {
            EnemyPathPresets[preset.name] = preset;
        }
    }

    private static void LoadPositions()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("positionToCoordinates");
        if (jsonData == null)
        {
            Debug.LogError("Failed to load position to coordinates data!");
            return;
        }
    
        Positions = JsonUtility.FromJson<Positions>(jsonData.text);
    
        if (Positions == null)
        {
            Debug.LogError("Failed to deserialize Positions.");
        }
    }

}
