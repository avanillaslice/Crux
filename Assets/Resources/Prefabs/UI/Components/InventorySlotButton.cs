using TMPro;
using UnityEngine;

public class InventorySlotButton : MonoBehaviour
{
    public Color DefaultColor;
    public Color HighlightedColor;
    public Color InvalidColor;
    public GameObject WeaponPrefab;
    public SlotType SlotType;
    public TextMeshProUGUI WeaponName;
    public bool IsEmpty = true;
    public bool IsSelected;
    public bool IsValid;
    public int ListIndex;

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

    public void HandleClicked()
    {
        if (Loadout.Inst.ActiveContainer == "WeaponSlots" || !IsValid) return;
        Loadout.Inst.HandleSelect();
    }

    public void HandlePointerEnter()
    {
        if (Loadout.Inst.ActiveContainer == "WeaponSlots" || !IsValid) return;
        Loadout.Inst.SetSelectedInventorySlotButton(ListIndex);
    }

    public void Validate()
    {
        WeaponName.color = DefaultColor;
        IsValid = true;
    }

    public void Invalidate()
    {
        WeaponName.color = InvalidColor;
        IsValid = false;
    }

    public void Select()
    {
        if (IsSelected) return;
        WeaponName.color = HighlightedColor;
        IsSelected = true;
    }
    public void Deselect()
    {  
        if (!IsSelected) return;
        WeaponName.color = DefaultColor;
        IsSelected = false;
    }
}