using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class Loadout : MonoBehaviour
{
    public static Loadout Inst { get; private set; }

    public List<WeaponSlotButton> WeaponSlotButtons;
    private WeaponSlotButton CurrentWeaponSlotButton;
    private int CurrentWeaponSlotButtonIndex;
    private static List<string> WeaponNames;
    private int CurrentWeaponNameIndex;

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Debug.Log("Loadout already exists");
            Destroy(gameObject);
            return;  // Ensure no further code execution in this instance
        }
        Inst = this;
        SetWeaponSlotButtons();
        SetWeaponNames();
        SetSelectedWeaponSlotButton(0);
    }

    private void SetWeaponSlotButtons()
    {
        Debug.Log("Setting WeaponSlotButtons");
        List<WeaponSlot> activeWeaponSlots = PlayerManager.Inst.ActivePlayerShip.GetActiveWeaponSlots();

        foreach (WeaponSlot weaponSlot in activeWeaponSlots)
        {
            foreach (WeaponSlotButton weaponSlotButton in WeaponSlotButtons)
            {
                if (!weaponSlotButton.IsEmpty || weaponSlotButton.SlotType != weaponSlot.Type) continue;
                Debug.Log("Attempting to SetWeaponSlotButton: " + weaponSlot.WeaponName);
                weaponSlotButton.SetWeapon(weaponSlot.WeaponName);
                break;
            }
            Debug.Log("Unable to find WeaponSlotButton for weapon slot of type: " + weaponSlot.Type);
        }
    }

    private void SetWeaponNames()
    {

    }

    // public void HandleMoveLeft()
    // public void HandleMoveRight()
    public void HandleMoveUp()
    {
        CurrentWeaponSlotButtonIndex = (CurrentWeaponSlotButtonIndex - 1 + WeaponSlotButtons.Count) % WeaponSlotButtons.Count;
        SetSelectedWeaponSlotButton(CurrentWeaponSlotButtonIndex);
    }
    public void HandleMoveDown()
    {
        CurrentWeaponSlotButtonIndex = (CurrentWeaponSlotButtonIndex + 1) % WeaponSlotButtons.Count;
        SetSelectedWeaponSlotButton(CurrentWeaponSlotButtonIndex);
    }
    private void SetSelectedWeaponSlotButton(int index)
    {
        if (CurrentWeaponSlotButton != null) CurrentWeaponSlotButton.Deselect();
        CurrentWeaponSlotButton = WeaponSlotButtons[index];
        CurrentWeaponSlotButton.Select();
    }

    public void HandleSelect()
    {
    }
    public void Back()
    {

    }
}
