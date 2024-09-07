using TMPro;
using UnityEngine;

public class WeaponSlotButton : MonoBehaviour
{
    public SlotType SlotType;
    public TextMeshProUGUI WeaponName;
    public bool IsEmpty = true;

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

    public void Select(){
        // Set Selected UI
    }
    public void Deselect(){
        // Set Deselected UI
    }
}