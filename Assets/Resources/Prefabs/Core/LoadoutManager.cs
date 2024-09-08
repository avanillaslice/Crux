using System.Collections.Generic;
using UnityEngine;

/*
LoadoutManager

- Manages the player's inventory of unlocked weapons
*/

public static class LoadoutManager
{
    // Dictionary of unlocked weapons (prefabs)
    private static Dictionary<string, GameObject> Inventory = new Dictionary<string, GameObject>();

    // Dictionary of WeaponSlot IDs and their associated prefabs
    private static Dictionary<int, GameObject> Loadout = new Dictionary<int, GameObject>();

    // Compiles Inventory from InitialPlayerData
    public static void InitialiseLoadout()
    {
        Inventory.Clear();
        Loadout.Clear();

        InitialShipData initialShipData = GameConfig.GetInitialPlayerData();
        if (initialShipData == null)
        {
            Debug.LogError("[LoadoutManager] Failed to fetch InitialPlayerData");
            return;
        }

        // Iterate through weapons in initialShipData
        foreach(var weaponEntry in initialShipData.Weapons)
        {
            if (weaponEntry.Value == false) continue;

            // Fetch the prefab
            GameObject weaponPrefab = AssetManager.GetWeaponPrefab(weaponEntry.Key);
            if (weaponPrefab == null)
            {
                Debug.LogError($"Weapon prefab not found for {weaponEntry.Key}");
                continue;
            }

            // Add to inventory
            Inventory[weaponEntry.Key] = weaponPrefab;
        }
    }

    // Attaches weapon to player ship
    public static void InitialiseWeapons()
    {
        if (Loadout.Count > 0) AttachWeaponsFromLoadout();
        else AttachWeaponsFromInventory();
    }

    // Iterates through the active loadout and attaches weapons to player ship
    private static void AttachWeaponsFromLoadout()
    {
        foreach (var loadoutEntry in Loadout)
        {
            int weaponSlotId = loadoutEntry.Key;
            GameObject weaponPrefab = loadoutEntry.Value;
            WeaponSlot weaponSlot = PlayerManager.Inst.ActivePlayerShip.AttemptWeaponAttachmentToSlot(weaponSlotId, weaponPrefab);
            if (weaponSlot == null) UnassignWeaponSlot(weaponSlotId);
        }
    }

    // Iterates through the inventory and attempts to attach all weapons to player ship
    private static void AttachWeaponsFromInventory()
    {
        if (Inventory.Count == 0)
        {
            Debug.LogWarning("[LoadoutManager] Inventory is empty. No weapons to initialize.");
            return;
        }
        
        foreach (var weaponEntry in Inventory)
        {
            GameObject weaponPrefab = weaponEntry.Value;
            WeaponSlot weaponSlot = PlayerManager.Inst.ActivePlayerShip.AttemptWeaponAttachment(weaponPrefab, false);
            if (weaponSlot != null) AssignPrefabToWeaponSlot(weaponSlot.id, weaponPrefab);
        }
    }

    // Adds the weaponPrefab to the inventory
    public static void UnlockWeapon(string weaponPrefabName)
    {
        if (Inventory.ContainsKey(weaponPrefabName)) return;
        GameObject weaponPrefab = AssetManager.GetWeaponPrefab(weaponPrefabName);
        if (weaponPrefab == null) return;
        Inventory[weaponPrefabName] = weaponPrefab;
    }

    // Equips the weaponPrefab to the player ship, does not add to inventory
    public static WeaponSlot EquipWeapon(string weaponPrefabName, bool force)
    {
        GameObject weaponPrefab = AssetManager.GetWeaponPrefab(weaponPrefabName);
        if (weaponPrefab == null) return null;
        WeaponSlot weaponSlot = PlayerManager.Inst.ActivePlayerShip.AttemptWeaponAttachment(weaponPrefab, force);
        if (weaponSlot == null) return null;
        AssignPrefabToWeaponSlot(weaponSlot.id, weaponPrefab);
        return weaponSlot;
    }

    public static void UnequipWeapon(WeaponSlot weaponSlot)
    {

    }

    public static List<GameObject> GetInventory()
    {
        List<GameObject> inventory = new List<GameObject>();
        foreach (var weaponEntry in Inventory)
        {
            inventory.Add(weaponEntry.Value);
        }
        return inventory;
    }

    private static void AssignPrefabToWeaponSlot(int id, GameObject weaponPrefab)
    {
        Loadout[id] = weaponPrefab;
    }

    private static void UnassignWeaponSlot(int id)
    {
        Loadout.Remove(id);
    }

}
