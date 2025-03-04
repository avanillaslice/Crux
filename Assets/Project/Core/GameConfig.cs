using System.Collections.Generic;
using Newtonsoft.Json;
using Project.Combat.Weapons.Effects;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Project.Core
{
    public static class GameConfig
    {
        public static bool HasBeenLoaded { get; private set; }

        // GAME DATA
        public static GameData GameData { get; private set; }
        public static EnemyPaths EnemyPaths { get; private set; }
        public static Positions Positions { get; private set; }
        public static Dictionary<string, PathData> EnemyPathPresets { get; private set; } = new Dictionary<string, PathData>();
        public static List<EffectData> EffectDataList { get; private set; } = new List<EffectData>();

        // CONFIG
        public static int InitialLives { get; private set; } = 3;
        public static int InitialScore { get; private set; } = 0;
        public static float RespawnTimer { get; private set; } = 2;
        public static bool CameraMovementEnabled { get; private set; } = false;
        public static float BaseVolume { get; private set; } = 0.1f;

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
            TextAsset textAsset = AssetManager.LoadAsset<TextAsset>("initialPlayerData");
            if (textAsset == null)
            {
                Debug.LogError("Failed to load initial player data!");
                return null;
            }

            Debug.Log("Initial player data loaded successfully");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<InitialShipData>(textAsset.text);
        }


        private static void LoadEffectData()
        {
            TextAsset textAsset = AssetManager.LoadAsset<TextAsset>("effectData");
            if (textAsset == null)
            {
                Debug.LogError("Failed to load effect data!");
                return;
            }

            Debug.Log("Effect data loaded successfully");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            EffectDataList = deserializer.Deserialize<List<EffectData>>(textAsset.text);

            if (EffectDataList == null || EffectDataList.Count == 0)
            {
                Debug.LogError("Failed to deserialize EffectData.");
                return;
            }
        }

        private static void LoadGameData()
        {
            TextAsset textAsset = AssetManager.LoadAsset<TextAsset>("gameData");
            if (textAsset == null)
            {
                Debug.LogError("Failed to load game data!");
                return;
            }

            Debug.Log("Game data loaded successfully");

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            GameData = deserializer.Deserialize<GameData>(textAsset.text);
        }

        private static void LoadEnemyPaths()
        {
            TextAsset textAsset = AssetManager.LoadAsset<TextAsset>("enemyPaths");
            if (textAsset == null) {
                Debug.LogError("Failed to load enemy paths data via Addressables!");
                return;
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            EnemyPaths = deserializer.Deserialize<EnemyPaths>(textAsset.text);

            if (EnemyPaths == null)
            {
                Debug.LogError("Failed to deserialize EnemyPaths.");
            }
        }


        private static void LoadEnemyPathPresets()
        {
            TextAsset textAsset = AssetManager.LoadAsset<TextAsset>("enemyPathPresets");
            if (textAsset == null)
            {
                Debug.LogError("Failed to load enemy path presets data!");
                return;
            }
            List<PathData> pathPresets = JsonConvert.DeserializeObject<List<PathData>>(textAsset.text);

            foreach (PathData preset in pathPresets)
            {
                EnemyPathPresets[preset.name] = preset;
            }
        }

        private static void LoadPositions()
        {
            TextAsset textAsset = AssetManager.LoadAsset<TextAsset>("positionToCoordinates");
            if (textAsset == null)
            {
                Debug.LogError("Failed to load position to coordinates data!");
                return;
            }
            Positions = JsonUtility.FromJson<Positions>(textAsset.text);

            if (Positions == null)
            {
                Debug.LogError("Failed to deserialize Positions.");
            }
        }
    }
}
