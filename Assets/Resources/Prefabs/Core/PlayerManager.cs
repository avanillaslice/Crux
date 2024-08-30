using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Game Data
    public static PlayerManager Inst { get; private set; }

    // Player Data
    private InitialShipData InitialShipData;
    public int Lives { get; set; }
    public PlayerShip ActivePlayerShip { get; set; }
    private AudioSource AudioSource;

    // Relevant GameObjects
    public GameObject BottomPlayerBoundary;
    public GameObject TopPlayerBoundary;

    // Store active weapon prefabs
    private Dictionary<int, string> WeaponSlotStates = new Dictionary<int, string>();

    // Store Unlocked Skills
    private List<SkillBase> ActiveSkills = new List<SkillBase>();

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Debug.Log("PlayerManager already exists");
            Destroy(gameObject);
            return;  // Ensure no further code execution in this instance
        }
        Inst = this;
        AudioSource = GetComponent<AudioSource>();
        SetInitialShipData();
    }

    private void SetInitialShipData()
    {
        InitialShipData = GameConfig.GetInitialPlayerData();
        if (InitialShipData == null)
        {
            Debug.LogError("[PlayerManager] Failed to fetch InitialPlayerData");
            return;
        }
    }

    public void HandlePlayerDestroyed()
    {
        PlayExplosionSound();
        ActivePlayerShip.DisablePrimaryFire();
        ActivePlayerShip.DisableSpecialFire();
        ActivePlayerShip.DisableShooting();
        SetWeaponSlotStates();
        SetActiveShipToNull();

        Lives -= 1;
        if (Lives > 0)
        {
            HUDManager.Inst.UpdateLivesDisplay();
            RespawnPlayer();
        }
        else
        {
            // Game Over
            GameManager.HandleGameOver();
        }
    }

    // Function called when the player is destroyed
    public void RespawnPlayer()
    {
        Debug.Log("Player destroyed. Respawning in 2 seconds...");
        StartCoroutine(DelayedRespawn(GameConfig.RespawnTimer)); // 2 seconds delay
    }

    // Coroutine to delay the respawn
    private IEnumerator DelayedRespawn(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        _ = SpawnPlayerAsync();
    }

    public async Task FlyOutOfScene()
    {
        TopPlayerBoundary.SetActive(false);
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(MoveOutOfSceneWithAcceleration(ActivePlayerShip.transform, 3.0f, 1f, tcs));
        await tcs.Task; // Wait for the movement to complete
        TopPlayerBoundary.SetActive(true);
    }

    private IEnumerator MoveOutOfSceneWithAcceleration(Transform transform, float initialSpeed, float acceleration, TaskCompletionSource<bool> tcs)
    {
        float currentSpeed = initialSpeed;

        while (transform.position.y <= 7)
        {
            // Accelerate the ship
            currentSpeed += acceleration * Time.deltaTime;
            BackgroundManager.Inst.ScrollSpeedModifier = 1 + currentSpeed * 3;

            // Move the ship upwards
            transform.position += Vector3.up * currentSpeed * Time.deltaTime;

            yield return null;
        }

        tcs.SetResult(true); // Signal that the movement is complete
    }

    public async Task SpawnPlayerAsync(bool initialSpawn = false)
    {
        Vector3 spawnPosition = initialSpawn ? new Vector3(0, -7, 10) : new Vector3(0, -4, 10);

        if (ActivePlayerShip != null) {
            ActivePlayerShip.transform.position = spawnPosition;
        }
        else
        {
            ActivePlayerShip = Instantiate(AssetManager.PlayerPrefab, spawnPosition, Quaternion.identity);
            // Sets the players ship for each still and attemps activation (if not already)
            ShipSkillManager.AssignShipToSkills(ActiveSkills, ActivePlayerShip);
            // Reattach saved weapon prefabs
            ReattachWeapons();
        }

        if (initialSpawn) await FlyIntoScene();
    }

    public async Task FlyIntoScene()
    {
        BottomPlayerBoundary.SetActive(false);
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine(MoveToPositionWithDeceleration(ActivePlayerShip.transform, new Vector3(0, -3, 10), 3.0f, 1f, tcs));
        await tcs.Task; // Wait for the movement to complete
        BottomPlayerBoundary.SetActive(true);
    }

    private IEnumerator MoveToPositionWithDeceleration(Transform transform, Vector3 targetPosition, float initialSpeed, float decelerationDistance, TaskCompletionSource<bool> tcs)
    {
        float currentSpeed = initialSpeed;
    
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            float distanceRemaining = Vector3.Distance(transform.position, targetPosition);
    
            // If within deceleration distance, reduce speed
            if (distanceRemaining < decelerationDistance)
            {
                currentSpeed = Mathf.Lerp(0, initialSpeed, distanceRemaining / decelerationDistance);
            }
            BackgroundManager.Inst.ScrollSpeedModifier = 1 + currentSpeed * 3;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            yield return null;
        }

        tcs.SetResult(true); // Signal that the movement is complete
    }

    public void BuildInitialSkills()
    {
        ActiveSkills = ShipSkillManager.BuildSkillList(InitialShipData, null);
    }

    private void ReattachWeapons()
    {
        if (WeaponSlotStates.Count == 0) 
        {
            // Needs to use the list of strings to fetch prefabs and attach to first available slot
            AttachWeaponsFromInitialPlayerData();
        }
        else
        {
            AttachWeaponsFromWeaponSlotStates();
            // Needs to use the existing dictionary of WeaponSlotStates to reattach weapons to specific slots
        }
    }

    private void AttachWeaponsFromInitialPlayerData()
    {
        foreach (var weapon in InitialShipData.Weapons)
        {
            if (weapon.Value == false) continue;

            GameObject weaponPrefab = AssetManager.GetWeaponPrefab(weapon.Key);
            if (weaponPrefab == null)
            {
                Debug.LogError($"Weapon prefab not found for {weapon.Key}");
                continue;
            }

            WeaponSlot weaponSlot = ActivePlayerShip.AttemptWeaponAttachment(weaponPrefab, false);
            if (weaponSlot != null && !weaponSlot.IsEmpty) weaponSlot.PrefabName = weapon.Key;
        }
    }

    private void SetWeaponSlotStates()
    {
        WeaponSlotStates = new Dictionary<int, string>();
        foreach (var weaponSlot in ActivePlayerShip.WeaponSlots)
        {
            WeaponSlotStates[weaponSlot.id] = weaponSlot.IsEmpty ? null : weaponSlot.PrefabName;
        }
    }

    private void AttachWeaponsFromWeaponSlotStates()
    {
        foreach (var weaponSlotState in WeaponSlotStates)
        {
            if (weaponSlotState.Value == null) continue;

            GameObject weaponPrefab = AssetManager.GetWeaponPrefab(weaponSlotState.Value);
            if (weaponPrefab == null)
            {
                Debug.LogError($"Weapon prefab not found for {weaponSlotState.Value}");
                continue;
            }
            WeaponSlot weaponSlot = ActivePlayerShip.AttemptWeaponAttachmentToSlot(weaponSlotState.Key, weaponPrefab);
            if (weaponSlot != null && !weaponSlot.IsEmpty) weaponSlot.PrefabName = weaponSlotState.Value;
        }
    }

    private void PlayExplosionSound()
    {
        if (AudioSource != null)
        {
            AudioSource.Play();
        }
    }

    public void IncrementLives(int amt)
    {
        Lives += amt;
        HUDManager.Inst.UpdateLivesDisplay();
    }

    public void SetActiveShipToNull()
    {
        ActivePlayerShip = null;
    }
}
