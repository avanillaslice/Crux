using TMPro;
using UnityEngine;

public class WeaponSlotButton : MonoBehaviour
{
    public GameObject HighlightLayer;
    public SlotType SlotType;
    public TextMeshProUGUI WeaponName;
    public bool IsEmpty = true;
    public bool IsSelected;
    public int ListIndex;

    public void SetWeapon(string weaponName)
    {
        WeaponName.text = weaponName;
        IsEmpty = false;
    }

    public void Clear()
    {
        WeaponName.text = "Empty";
        IsEmpty = true;
    }

    public void HandleClicked()
    {
        if (Loadout.Inst.ActiveContainer == "Inventory") return;
        Loadout.Inst.HandleSelect();
    }
    
    public void HandlePointerEnter()
    {
        if (Loadout.Inst.ActiveContainer == "Inventory") return;
        Loadout.Inst.SetSelectedWeaponSlotButton(ListIndex);
    }

    public void Select()
    {
        HighlightLayer.SetActive(true);
        IsSelected = true;
    }
    public void Deselect(){
        HighlightLayer.SetActive(false);
        IsSelected = false;
    }
}