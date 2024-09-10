using UnityEngine;

public class WeaponEffect : EffectBase
{
    private WeaponSlot AssignedWeaponSlot;
    public override void Activate(GameObject targetShip)
    {
        TargetShip = targetShip;

        if (TargetShip.CompareTag("Player"))
        {
            LoadoutManager.UnlockWeapon(SubType.ToString());

            GameObject weaponPrefab = AssetManager.GetWeaponPrefab(SubType.ToString());
            if (weaponPrefab == null) return;

            AssignedWeaponSlot = LoadoutManager.EquipWeapon(weaponPrefab, false);

            if (AssignedWeaponSlot != null)
            {
                MusicManager.Inst.PlaySoundEffect("GunLoad", 1f);
                if (AssignedWeaponSlot.WeaponType == WeaponType.Special) GameManager.HandleSpecialWeaponUnlock();
            }
        }
        else
        {
            GameObject weaponPrefab = AssetManager.GetWeaponPrefab(SubType.ToString());
            if (weaponPrefab == null)
            {
               Debug.LogError("Weapon prefab not found for " + targetShip.name);
            }

            ShipBase shipComponent = TargetShip.GetComponent<ShipBase>();
            AssignedWeaponSlot = shipComponent.AttemptWeaponAttachment(weaponPrefab, false);
        }

        if (AssignedWeaponSlot == null) return;

        if (Expiry == ExpiryType.Time && Duration > 0) {
            Debug.Log($"Expiry detected for {gameObject.name} with duration {Duration}");
            CoroutineManager.Inst.DeactivateEffectAfterDelay(this, Duration);
        }
    }

    //! REQUIRES PLAYER CENTRIC - LOADOUT MANAGER LOGIC
    public override void Deactivate()
    {
        Debug.Log("Deactivating");
        ShipBase shipComponent = TargetShip.GetComponent<ShipBase>();
        shipComponent.DetachWeaponsFromSlot(AssignedWeaponSlot);
    }
}