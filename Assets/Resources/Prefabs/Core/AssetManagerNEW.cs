using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetManagerNEW
{
    // PREFABS
    public static PlayerShip PlayerPrefab { get; private set; }
    public static DroneShip ShieldDronePrefab { get; private set; }
    public static DroneShip AttackDronePrefab { get; private set; }
    public static Wave WavePrefab { get; private set; }
    public static GameObject ItemDropPrefab { get; private set; }
    public static GameObject LifeIconPrefab { get; private set; }
    public static GameObject WeaponSlotPrefab { get; private set; }
    public static GameObject StarMapPrefab { get; private set; }
    public static GameObject DistantStarPrefab { get; private set; }
    public static GameObject DistantPlanetPrefab { get; private set; }
    public static GameObject PlanetPrefab { get; private set; }

    // PREFAB DICTIONARIES
    public static Dictionary<string, EnemyShip> EnemyShipPrefabs { get; private set; } = new Dictionary<string, EnemyShip>();
    public static Dictionary<string, GameObject> WeaponPrefabs { get; private set; } = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> ProjectilePrefabs { get; private set; } = new Dictionary<string, GameObject>();

    // SPRITES
    public static List<Sprite> DistantStarSprites { get; private set; } = new List<Sprite>();
    public static List<Sprite> DistantPlanetSprites { get; private set; } = new List<Sprite>();
    public static List<Sprite> PlanetSprites { get; private set; } = new List<Sprite>();

    // AUDIO
    public static Dictionary<string, AudioClip> SoundEffects { get; private set; } = new Dictionary<string, AudioClip>();
    public static Dictionary<string, AudioClip> BackgroundMusic { get; private set; } = new Dictionary<string, AudioClip>();
    public static Dictionary<string, AudioClip> BossMusic { get; private set; } = new Dictionary<string, AudioClip>();

    // UI
    public static GameObject ShipSelectionUIPrefab { get; private set; }
    public static GameObject PickupMessagePrefab { get; private set; }
    public static GameObject SpecialWeaponUnlockedPrefab { get; private set; }
    public static GameObject PauseMenuPrefab { get; private set; }
    public static GameObject InterStageUIPrefab { get; private set; }
    public static GameObject LoadoutUIPrefab { get; private set; }
    public static GameObject OldLoadoutUIPrefab { get; private set; }
    public static GameObject SkillTreeUIPrefab { get; private set; }

    // UI COMPONENTS
    public static GameObject WeaponNodePrefab { get; private set; }
    public static GameObject WeaponNodeSelectorPrefab { get; private set; }
    public static GameObject WeaponNodeSelectorListPrefab { get; private set; }
    public static GameObject WeaponNodeSelectorListCellPrefab { get; private set; }

    // SETTINGS
    private static List<string> WeaponPrefabsToLoad = new List<string> { "Cannon", "CannonSmall", "MissileLauncher", "HomingMissileLauncher", "ElectroShield", "ElectroShieldEffect", "DroneShield", "DroneShieldEffect", "TurretSmall" };
    private static List<string> ProjectilesToLoad = new List<string> { "Plasma", "PlasmaLight", "PlasmaHeavy", "Missile", "HomingMissile", "ElectricExplosion", "ElectricExplosionChain" };
    private static List<string> EnemyShipsToLoad = new List<string> { "SF1", "SF2", "SF3" };

    public static GameObject GetProjectilePrefab(string name)
    {
        if (ProjectilePrefabs.TryGetValue(name, out GameObject prefab))
        {
            return prefab;
        }
        Debug.LogError($"Projectile prefab '{name}' not found.");
        return null;
    }

    public static GameObject GetWeaponPrefab(string name)
    {
        if (WeaponPrefabs.TryGetValue(name, out GameObject prefab))
        {
            return prefab;
        }
        Debug.LogError($"Weapon prefab '{name}' not found.");
        return null;
    }

    public static EnemyShip GetEnemyShipPrefab(string name)
    {
        if (EnemyShipPrefabs.TryGetValue(name, out EnemyShip prefab))
        {
            return prefab;
        }
        Debug.LogError($"Enemy ship prefab '{name}' not found.");
        return null;
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
        LoadAsset<GameObject>("Prefabs/UI/MainMenu/ShipSelectionUI", result => ShipSelectionUIPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Gameplay/PickupMessage", result => PickupMessagePrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Gameplay/SpecialWeaponUnlocked", result => SpecialWeaponUnlockedPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Gameplay/PauseMenu", result => PauseMenuPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/InterStage/InterStageUI", result => InterStageUIPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Loadout/OldLoadout/OldLoadoutUI", result => OldLoadoutUIPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/SkillTree/SkillTreeUI", result => SkillTreeUIPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Loadout/LoadoutUI", result => LoadoutUIPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Loadout/WeaponNodeSelector", result => WeaponNodeSelectorPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Loadout/WeaponNode", result => WeaponNodePrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Loadout/WeaponNodeSelectorList", result => WeaponNodeSelectorListPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Loadout/WeaponNodeSelectorListCell", result => WeaponNodeSelectorListCellPrefab = result);
    }

    private static void CacheAudioAssets()
    {
        CacheSoundEffects();
        CacheBackgroundMusic();
        CacheBossMusic();
    }

    private static void CacheSoundEffects()
    {
        LoadAllAssets<AudioClip>("Audio/SoundEffects", SoundEffects);
    }

    private static void CacheBackgroundMusic()
    {
        LoadAllAssets<AudioClip>("Audio/BackgroundMusic", BackgroundMusic);
    }

    private static void CacheBossMusic()
    {
        LoadAllAssets<AudioClip>("Audio/BackgroundMusic/Boss", BossMusic);
    }

    private static void CacheShipPrefabs()
    {
        LoadAsset<PlayerShip>("Prefabs/Ships/Player", result => PlayerPrefab = result);
        LoadAsset<DroneShip>("Prefabs/Ships/ShieldDrone", result => ShieldDronePrefab = result);
        LoadAsset<DroneShip>("Prefabs/Ships/AttackDrone", result => AttackDronePrefab = result);
        foreach (string shipName in EnemyShipsToLoad)
        {
            LoadAsset<EnemyShip>($"Prefabs/Ships/{shipName}", result =>
            {
                if (result != null)
                {
                    EnemyShipPrefabs[shipName] = result;
                }
            });
        }
    }

    private static void CacheWeaponPrefabs()
    {
        foreach (string weaponName in WeaponPrefabsToLoad)
        {
            LoadAsset<GameObject>($"Prefabs/Combat/Weapons/{weaponName}", result =>
            {
                if (result != null)
                {
                    WeaponPrefabs[weaponName] = result;
                }
            });
        }
    }

    private static void CacheProjectilePrefabs()
    {
        foreach (string projectileName in ProjectilesToLoad)
        {
            LoadAsset<GameObject>($"Prefabs/Combat/Projectiles/{projectileName}", result =>
            {
                if (result != null)
                {
                    ProjectilePrefabs[projectileName] = result;
                }
            });
        }
    }

    private static void CacheMiscAssets()
    {
        LoadAsset<Wave>("Prefabs/Game/Wave", result => WavePrefab = result);
        LoadAsset<GameObject>("Prefabs/Game/ItemDrop", result => ItemDropPrefab = result);
    }

    private static void CacheBackgroundAssets()
    {
        LoadAsset<GameObject>("Prefabs/UI/Background/StarMap", result => StarMapPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Components/ShipIcon", result => LifeIconPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Background/DistantStar", result => DistantStarPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Background/DistantPlanet", result => DistantPlanetPrefab = result);
        LoadAsset<GameObject>("Prefabs/UI/Background/Planet", result => PlanetPrefab = result);

        LoadAllAssets<Sprite>("Sprites/SPACE/DistantStars", DistantStarSprites);
        LoadAllAssets<Sprite>("Sprites/SPACE/DistantPlanets", DistantPlanetSprites);
        LoadAllAssets<Sprite>("Sprites/SPACE/Planets", PlanetSprites);
    }

    private static void LoadAsset<T>(string address, System.Action<T> onLoaded) where T : UnityEngine.Object
    {
        Addressables.LoadAssetAsync<T>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"Failed to load asset at {address}");
            }
        };
    }

    private static void LoadAllAssets<T>(string address, ICollection<T> collection) where T : UnityEngine.Object
    {
        Addressables.LoadAssetsAsync<T>(address, null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var asset in handle.Result)
                {
                    collection.Add(asset);
                }
            }
            else
            {
                Debug.LogError($"Failed to load assets at {address}");
            }
        };
    }

    private static void LoadAllAssets<T>(string address, IDictionary<string, T> dictionary) where T : UnityEngine.Object
    {
        Addressables.LoadAssetsAsync<T>(address, null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var asset in handle.Result)
                {
                    dictionary[asset.name] = asset;
                }
            }
            else
            {
                Debug.LogError($"Failed to load assets at {address}");
            }
        };
    }
}