using System.Collections.Generic;
using UnityEngine;

public static class AssetManager
{
    // PREFABS
    public static PlayerShip PlayerPrefab { get; private set; }
    public static DroneShip ShieldDronePrefab { get; private set; }
    public static DroneShip AttackDronePrefab { get; private set; }
    public static Wave WavePrefab { get; private set; }
    public static ItemDrop ItemDropPrefab { get; private set; }
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

    // SETTINGS
    private static List<string> WeaponPrefabsToLoad = new List<string> { "Cannon", "CannonSmall", "MissileLauncher", "HomingMissileLauncher", "ElectroShield", "ElectroShieldEffect", "DroneShield", "DroneShieldEffect", "TurretSmall" };
    private static List<string> ProjectilesToLoad = new List<string> { "Plasma", "PlasmaLight", "PlasmaHeavy", "Missile", "HomingMissile", "ElectricExplosion", "ElectricExplosionChain" };
    private static List<string> EnemyShipsToLoad = new List<string> { "SF1", "SF2", "SF3" };

    // Fetch function for EnemyShipPrefabs
    public static EnemyShip GetEnemyShipPrefab(string key)
    {
        if (EnemyShipPrefabs.TryGetValue(key, out var prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogError($"EnemyShip prefab with key '{key}' not found.");
            return null;
        }
    }

    // Fetch function for WeaponPrefabs
    public static GameObject GetWeaponPrefab(string key)
    {
        if (WeaponPrefabs.TryGetValue(key, out var weaponPrefabComponent))
        {
            return weaponPrefabComponent;
        }
        else
        {
            Debug.LogError($"Weapon prefab with key '{key}' not found.");
            return null;
        }
    }

    // Fetch function for ProjectilePrefabs
    public static GameObject GetProjectilePrefab(string key)
    {
        if (ProjectilePrefabs.TryGetValue(key, out var prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogError($"Projectile prefab with key '{key}' not found.");
            return null;
        }
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
        ShipSelectionUIPrefab = Resources.Load<GameObject>("Prefabs/UI/ShipSelectionUI");
        PickupMessagePrefab = Resources.Load<GameObject>("Prefabs/UI/PickupMessage");
        SpecialWeaponUnlockedPrefab = Resources.Load<GameObject>("Prefabs/UI/SpecialWeaponUnlocked");
        PauseMenuPrefab = Resources.Load<GameObject>("Prefabs/UI/PauseMenu");
        InterStageUIPrefab = Resources.Load<GameObject>("Prefabs/UI/InterStageUI");
        LoadoutUIPrefab = Resources.Load<GameObject>("Prefabs/UI/LoadoutUI");
        if (ShipSelectionUIPrefab == null) Debug.LogError("Failed to load ShipSelectionUIPrefab!");
        if (PickupMessagePrefab == null) Debug.LogError("Failed to load PickupMessagePrefab!");
        if (SpecialWeaponUnlockedPrefab == null) Debug.LogError("Failed to load SpecialWeaponUnlockedPrefab!");
        if (PauseMenuPrefab == null) Debug.LogError("Failed to load PauseMenuPrefab!");
        if (LoadoutUIPrefab == null) Debug.LogError("Failed to load LoadoutUIPrefab!");
    }

    private static void CacheAudioAssets()
    {
        CacheSoundEffects();
        CacheBackgroundMusic();
        CacheBossMusic();
    }

    private static void CacheSoundEffects()
    {
        SoundEffects.Clear(); // Clear the dictionary first

        // Load all audio files from the specified directory
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Audio/SoundEffects");
        foreach (AudioClip audioClip in audioClips)
        {
            string fileName = audioClip.name;
            SoundEffects[fileName] = audioClip;
        }

        if (SoundEffects.Count == 0) Debug.LogError("No audio files found in the Audio/SoundEffects directory.");
    }

    private static void CacheBackgroundMusic()
    {
        BackgroundMusic.Clear(); // Clear the dictionary first

        // Load all audio files from the specified directory
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Audio/BackgroundMusic");
        foreach (AudioClip audioClip in audioClips)
        {
            string fileName = audioClip.name;
            BackgroundMusic[fileName] = audioClip;
        }

        if (BackgroundMusic.Count == 0) Debug.LogError("No audio files found in the Audio/BackgroundMusic directory.");
    }

    private static void CacheBossMusic()
    {
        BossMusic.Clear(); // Clear the dictionary first

        // Load all audio files from the specified directory
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Audio/BackgroundMusic/Boss");
        foreach (AudioClip audioClip in audioClips)
        {
            string fileName = audioClip.name;
            BossMusic[fileName] = audioClip;
        }

        if (BossMusic.Count == 0) Debug.LogError("No audio files found in the Audio/BackgroundMusic/Boss directory.");
    }

    private static void CacheShipPrefabs()
    {
        PlayerPrefab = Resources.Load<PlayerShip>("Prefabs/Ships/Player");
        ShieldDronePrefab = Resources.Load<DroneShip>("Prefabs/Ships/ShieldDrone");
        AttackDronePrefab = Resources.Load<DroneShip>("Prefabs/Ships/AttackDrone");
        foreach (string shipName in EnemyShipsToLoad)
        {
            EnemyShip shipPrefab = Resources.Load<EnemyShip>($"Prefabs/Ships/{shipName}");
            if (shipPrefab != null)
            {
                EnemyShipPrefabs[shipName] = shipPrefab;
            }
            else
            {
                Debug.LogError($"Failed to load EnemyShip {shipName} prefab!");
            }
        }
        if (PlayerPrefab == null) Debug.LogError("Failed to load Player prefab!");
        if (ShieldDronePrefab == null) Debug.LogError("Failed to load ShieldDrone Prefab!");
        if (AttackDronePrefab == null) Debug.LogError("Failed to load ShieldDrone Prefab!");
    }

    private static void CacheWeaponPrefabs()
    {
        foreach (string weaponName in WeaponPrefabsToLoad)
        {
            GameObject weaponPrefab = Resources.Load<GameObject>($"Prefabs/Combat/Weapons/{weaponName}");
            if (weaponPrefab != null)
            {
                WeaponPrefabs[weaponName] = weaponPrefab;
            }
            else
            {
                Debug.LogError($"Failed to load Weapon {weaponName} prefab!");
            }
        }
    }

    private static void CacheProjectilePrefabs()
    {
        foreach (string projectileName in ProjectilesToLoad)
        {
            GameObject projectilePrefab = Resources.Load<GameObject>($"Prefabs/Combat/Projectiles/{projectileName}");
            if (projectilePrefab != null)
            {
                ProjectilePrefabs[projectileName] = projectilePrefab;
            }
            else
            {
                Debug.LogError($"Failed to load {projectileName} prefab!");
            }
        }
    }

    private static void CacheMiscAssets()
    {
        // CORE
        WavePrefab = Resources.Load<Wave>("Prefabs/Game/Wave");
        if (WavePrefab == null) Debug.LogError("Failed to load Wave prefab!");

        // GAME
        ItemDropPrefab = Resources.Load<ItemDrop>("Prefabs/Game/ItemDrop");
        if (ItemDropPrefab == null) Debug.LogError("Failed to load ItemDrop prefab!");
    }

    private static void CacheBackgroundAssets()
    {
        // UI
        StarMapPrefab = Resources.Load<GameObject>("Prefabs/UI/Background/StarMap");
        LifeIconPrefab = Resources.Load<GameObject>("Prefabs/UI/Components/ShipIcon");
        if (LifeIconPrefab == null) Debug.LogError("Failed to load ShipIcon prefab!");
        if (StarMapPrefab == null) Debug.LogError("Failed to load StarMap prefab!");

        // Distant Object
        DistantStarPrefab = Resources.Load<GameObject>("Prefabs/UI/Background/DistantStar");
        DistantPlanetPrefab = Resources.Load<GameObject>("Prefabs/UI/Background/DistantPlanet");
        PlanetPrefab = Resources.Load<GameObject>("Prefabs/UI/Background/Planet");
        if (DistantStarPrefab == null) Debug.LogError("Failed to load DistantStarPrefab!");
        if (DistantPlanetPrefab == null) Debug.LogError("Failed to load DistantPlanetPrefab!");
        if (PlanetPrefab == null) Debug.LogError("Failed to load PlanetPrefab!");

        // Load all sprites from the specified directories
        DistantStarSprites.AddRange(Resources.LoadAll<Sprite>("Sprites/SPACE/DistantStars"));
        DistantPlanetSprites.AddRange(Resources.LoadAll<Sprite>("Sprites/SPACE/DistantPlanets"));
        PlanetSprites.AddRange(Resources.LoadAll<Sprite>("Sprites/SPACE/Planets"));
        if (DistantStarSprites.Count == 0) Debug.LogError("No distant star sprites found in the specified directory.");
        if (DistantPlanetSprites.Count == 0) Debug.LogError("No distant planet sprites found in the specified directory.");
        if (PlanetSprites.Count == 0) Debug.LogError("No planet sprites found in the specified directory.");
    }
}