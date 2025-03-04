using System.Collections.Generic;
using Project.Game.Wave;
using Project.Ships;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Project.Core
{
    /// <summary>
    /// Manages the loading and caching of game assets using Unity's Addressables system.
    /// </summary>
    public static class AssetManager
    {
        public static PlayerShip PlayerPrefab { get; private set; }
        public static DroneShip ShieldDronePrefab { get; private set; }
        public static DroneShip AttackDronePrefab { get; private set; }
        public static Wave WavePrefab { get; private set; }
        public static GameObject ItemDropPrefab { get; private set; }
        public static GameObject LifeIconPrefab { get; private set; }
        public static GameObject StarMapPrefab { get; private set; }
        public static GameObject DistantStarPrefab { get; private set; }
        public static GameObject DistantPlanetPrefab { get; private set; }
        public static GameObject PlanetPrefab { get; private set; }

        private static Dictionary<string, EnemyShip> EnemyShipPrefabs { get; set; } = new Dictionary<string, EnemyShip>();
        private static Dictionary<string, GameObject> WeaponPrefabs { get; set; } = new Dictionary<string, GameObject>();
        private static Dictionary<string, GameObject> ProjectilePrefabs { get; set; } = new Dictionary<string, GameObject>();

        public static List<Sprite> DistantStarSprites { get; private set; } = new List<Sprite>();
        public static List<Sprite> DistantPlanetSprites { get; private set; } = new List<Sprite>();
        public static List<Sprite> PlanetSprites { get; private set; } = new List<Sprite>();

        public static Dictionary<string, AudioClip> SoundEffects { get; private set; } = new Dictionary<string, AudioClip>();
        public static Dictionary<string, AudioClip> BackgroundMusic { get; private set; } = new Dictionary<string, AudioClip>();
        public static Dictionary<string, AudioClip> BossMusic { get; private set; } = new Dictionary<string, AudioClip>();

        public static GameObject ShipSelectionUIPrefab { get; private set; }
        public static GameObject PickupMessagePrefab { get; private set; }
        public static GameObject SpecialWeaponUnlockedPrefab { get; private set; }
        public static GameObject PauseMenuPrefab { get; private set; }
        public static GameObject InterStageUIPrefab { get; private set; }
        public static GameObject LoadoutUIPrefab { get; private set; }
        public static GameObject OldLoadoutUIPrefab { get; private set; }
        public static GameObject SkillTreeUIPrefab { get; private set; }

        public static GameObject WeaponNodePrefab { get; private set; }
        public static GameObject WeaponNodeSelectorPrefab { get; private set; }
        public static GameObject WeaponNodeSelectorListCellPrefab { get; private set; }

        private static readonly List<string> WeaponPrefabsToLoad = new List<string> 
        { 
            "Cannon", "CannonSmall", "MissileLauncher", "HomingMissileLauncher", 
            "ElectroShield", "ElectroShieldEffect", "DroneShield", "DroneShieldEffect", 
            "TurretSmall" 
        };
        private static readonly List<string> ProjectilesToLoad = new List<string> 
        { 
            "Plasma", "PlasmaLight", "PlasmaHeavy", "Missile", "HomingMissile", 
            "ElectricExplosion", "ElectricExplosionChain" 
        };
        private static readonly List<string> EnemyShipsToLoad = new List<string> { "SF1", "SF2", "SF3" };

        public static GameObject GetProjectilePrefab(string name)
        {
            if (ProjectilePrefabs.TryGetValue(name, out var prefab))
            {
                return prefab;
            }
            Debug.LogError($"Projectile prefab '{name}' not found.");
            return null;
        }

        public static GameObject GetWeaponPrefab(string name)
        {
            if (WeaponPrefabs.TryGetValue(name, out var prefab))
            {
                return prefab;
            }
            Debug.LogError($"Weapon prefab '{name}' not found.");
            return null;
        }

        public static EnemyShip GetEnemyShipPrefab(string name)
        {
            if (EnemyShipPrefabs.TryGetValue(name, out var prefab))
            {
                return prefab;
            }
            Debug.LogError($"Enemy ship prefab '{name}' not found.");
            return null;
        }

        // New synchronous loading methods
        public static T LoadAsset<T>(string address) where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            handle.WaitForCompletion();
        
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load asset at {address}.");
                return null;
            }
        }

        private static List<T> LoadAllAssets<T>(string address) where T : Object
        {
            var results = new List<T>();
            var handle = Addressables.LoadAssetsAsync<T>(address, null);
            handle.WaitForCompletion();
        
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                results.AddRange(handle.Result);
            }
            else
            {
                Debug.LogError($"Failed to load assets at {address}.");
            }

            return results;
        }

        public static void CacheAssets()
        {
            CacheShipPrefabs();
            CacheWeaponPrefabs();
            CacheProjectilePrefabs();
            CacheMiscAssets();
            CacheBackgroundAssets();
            CacheAudioAssets();
            CacheUIAssets();
        }

        private static void CacheUIAssets()
        {
            ShipSelectionUIPrefab = LoadAsset<GameObject>("ShipSelection");
            PickupMessagePrefab = LoadAsset<GameObject>("PickupMessage");
            SpecialWeaponUnlockedPrefab = LoadAsset<GameObject>("SpecialWeaponUnlocked");
            PauseMenuPrefab = LoadAsset<GameObject>("PauseMenu");
            InterStageUIPrefab = LoadAsset<GameObject>("InterStage");
            OldLoadoutUIPrefab = LoadAsset<GameObject>("OldLoadout");
            SkillTreeUIPrefab = LoadAsset<GameObject>("SkillTree");
            LoadoutUIPrefab = LoadAsset<GameObject>("Loadout");
            WeaponNodeSelectorPrefab = LoadAsset<GameObject>("WeaponNodeSelector");
            WeaponNodePrefab = LoadAsset<GameObject>("WeaponNode");
            WeaponNodeSelectorListCellPrefab = LoadAsset<GameObject>("WeaponNodeSelectorListCell");
        }

        private static void CacheAudioAssets()
        {
            var soundEffects = LoadAllAssets<AudioClip>("SoundEffects");
            foreach (var clip in soundEffects)
            {
                SoundEffects[clip.name] = clip;
            }
        
            var bgm = LoadAllAssets<AudioClip>("BGM");
            foreach (var clip in bgm)
            {
                BackgroundMusic[clip.name] = clip;
            }
        
            var bossMusic = LoadAllAssets<AudioClip>("BossMusic");
            foreach (var clip in bossMusic)
            {
                BossMusic[clip.name] = clip;
            }
        }

        private static void CacheShipPrefabs()
        {
            var playerObj = LoadAsset<GameObject>("Player");
            if (playerObj != null)
            {
                PlayerPrefab = playerObj.GetComponent<PlayerShip>();
                if (PlayerPrefab == null)
                {
                    Debug.LogError("PlayerShip component not found on the loaded GameObject.");
                }
            }

            var shieldDroneObj = LoadAsset<GameObject>("ShieldDrone");
            if (shieldDroneObj != null)
            {
                ShieldDronePrefab = shieldDroneObj.GetComponent<DroneShip>();
                if (ShieldDronePrefab == null)
                {
                    Debug.LogError("DroneShip component not found on the loaded GameObject.");
                }
            }

            var attackDroneObj = LoadAsset<GameObject>("AttackDrone");
            if (attackDroneObj != null)
            {
                AttackDronePrefab = attackDroneObj.GetComponent<DroneShip>();
                if (AttackDronePrefab == null)
                {
                    Debug.LogError("DroneShip component not found on the loaded GameObject.");
                }
            }

            foreach (var shipName in EnemyShipsToLoad)
            {
                var shipObj = LoadAsset<GameObject>(shipName);
                if (shipObj == null) continue;
            
                var enemyShip = shipObj.GetComponent<EnemyShip>();
                if (enemyShip != null)
                {
                    EnemyShipPrefabs[shipName] = enemyShip;
                }
                else
                {
                    Debug.LogError($"EnemyShip component not found on the loaded GameObject for {shipName}.");
                }
            }
        }

        private static void CacheWeaponPrefabs()
        {
            foreach (var weaponName in WeaponPrefabsToLoad)
            {
                var weaponObj = LoadAsset<GameObject>(weaponName);
                if (weaponObj != null)
                {
                    WeaponPrefabs[weaponName] = weaponObj;
                }
            }
        }

        private static void CacheProjectilePrefabs()
        {
            foreach (var projectileName in ProjectilesToLoad)
            {
                var projectileObj = LoadAsset<GameObject>(projectileName);
                if (projectileObj != null)
                {
                    ProjectilePrefabs[projectileName] = projectileObj;
                }
            }
        }

        private static void CacheMiscAssets()
        {
            var waveObj = LoadAsset<GameObject>("Wave");
            if (waveObj != null)
            {
                WavePrefab = waveObj.GetComponent<Wave>();
                if (WavePrefab == null)
                {
                    Debug.LogError("Wave component not found on the loaded GameObject.");
                }
            }

            ItemDropPrefab = LoadAsset<GameObject>("ItemDrop");
        }

        private static void CacheBackgroundAssets()
        {
            StarMapPrefab = LoadAsset<GameObject>("StarMap");
            LifeIconPrefab = LoadAsset<GameObject>("LifeIcon");
            DistantStarPrefab = LoadAsset<GameObject>("DistantStar");
            DistantPlanetPrefab = LoadAsset<GameObject>("DistantPlanet");
            PlanetPrefab = LoadAsset<GameObject>("Planet");

            DistantStarSprites.AddRange(LoadAllAssets<Sprite>("DistantStars"));
            DistantPlanetSprites.AddRange(LoadAllAssets<Sprite>("DistantPlanets"));
            PlanetSprites.AddRange(LoadAllAssets<Sprite>("Planets"));
        }
    }
}
