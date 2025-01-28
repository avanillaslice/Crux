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
				if (WeaponPrefab == null) {
					WeaponName.text = "Empty";
					IsEmpty = false;
					return;
				}
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
        if (OldLoadoutUI.Inst.ActiveContainer == "WeaponSlots" || !IsValid) return;
        OldLoadoutUI.Inst.HandleSelect();
    }

    public void HandlePointerEnter()
    {
        if (OldLoadoutUI.Inst.ActiveContainer == "WeaponSlots" || !IsValid) return;
        OldLoadoutUI.Inst.SetSelectedInventorySlotButton(ListIndex);
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