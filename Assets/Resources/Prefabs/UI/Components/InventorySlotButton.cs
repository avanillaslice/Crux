using TMPro;
using UnityEngine;

public class InventorySlotButton : MonoBehaviour
{
    private GameObject WeaponPrefab;
    public SlotType SlotType;
    public TextMeshProUGUI WeaponName;
    public bool IsEmpty = true;

    public void SetWeapon(GameObject weaponPrefab)
    {
        WeaponPrefab = weaponPrefab;
        WeaponName.text = WeaponPrefab.GetComponent<WeaponBase>().WeaponName;
        SlotType = WeaponPrefab.GetComponent<WeaponBase>().SlotType;
        IsEmpty = false;
    }

    public void Clear()
    {
        WeaponPrefab = null;
        WeaponName.text = "";
        IsEmpty = true;
    }

    public void Select(){
        // Set Selected UI
    }
    public void Deselect(){
        // Set Deselected UI
    }
}