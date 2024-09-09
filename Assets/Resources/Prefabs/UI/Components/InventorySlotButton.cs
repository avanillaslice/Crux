using TMPro;
using UnityEngine;

public class InventorySlotButton : MonoBehaviour
{
    public Color DefaultColor;
    public Color HighlightedColor;
    private GameObject WeaponPrefab;
    public SlotType SlotType;
    public TextMeshProUGUI WeaponName;
    public bool IsEmpty = true;
    public bool IsSelected;
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
        if (Loadout.Inst.ActiveContainer == "WeaponSlots") return;
        Loadout.Inst.HandleSelect();
    }

    public void HandlePointerEnter()
    {
        if (Loadout.Inst.ActiveContainer == "WeaponSlots") return;
        Loadout.Inst.SetSelectedInventorySlotButton(ListIndex);
    }

    public void Select()
    {
        WeaponName.color = HighlightedColor;
        IsSelected = true;
    }
    public void Deselect(){
        WeaponName.color = DefaultColor;
        IsSelected = false;
    }
}