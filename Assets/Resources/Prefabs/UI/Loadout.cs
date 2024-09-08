using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class Loadout : MonoBehaviour
{
    public static Loadout Inst { get; private set; }

    public GameObject WeaponSlotContainer;
    private List<WeaponSlotButton> WeaponSlotButtons;
    private WeaponSlotButton CurrentWeaponSlotButton;
    private int CurrentWeaponSlotButtonIndex;

    public GameObject InventorySlotContainer;
    private List<InventorySlotButton> InventorySlotButtons;
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
        InitialiseWeaponSlotButtons();
        InitialiseInventorySlotButtons();
    }

    void OnEnable()
    {
        SetWeaponSlots();
        SetInventory();
        SetSelectedWeaponSlotButton(0);
    }

    void OnDisable()
    {
        ClearWeaponSlots();
        ClearInventorySlots();
    }

    private void ClearWeaponSlots()
    {
        foreach (WeaponSlotButton button in WeaponSlotButtons)
        {
            button.Clear();
        }
    }

    private void ClearInventorySlots()
    {
        foreach (InventorySlotButton button in InventorySlotButtons)
        {
            button.Clear();
        }
    }

    public void HandleBackClicked()
    {
        UIManager.Inst.DisableLoadoutUI();
        UIManager.Inst.EnableInterStageUI();
    }

    private void InitialiseInventorySlotButtons()
    {
        InventorySlotButtons = new List<InventorySlotButton>();
        foreach (Transform child in InventorySlotContainer.transform)
        {
            InventorySlotButtons.Add(child.GetComponent<InventorySlotButton>());
        }
    }

    private void InitialiseWeaponSlotButtons()
    {
        WeaponSlotButtons = new List<WeaponSlotButton>();
        foreach (Transform child in WeaponSlotContainer.transform)
        {
            WeaponSlotButtons.Add(child.GetComponent<WeaponSlotButton>());
        }
    }

    private void SetWeaponSlots()
    {
        Debug.Log("Setting WeaponSlotButtons");
        List<WeaponSlot> activeWeaponSlots = PlayerManager.Inst.ActivePlayerShip.GetActiveWeaponSlots();

        foreach (WeaponSlot weaponSlot in activeWeaponSlots)
        {
            Debug.Log("Attempting to SetWeaponSlotButton: " + weaponSlot.WeaponName);
            bool foundValidWeaponSlotButton = false;

            foreach (WeaponSlotButton weaponSlotButton in WeaponSlotButtons)
            {
                if (!weaponSlotButton.IsEmpty || weaponSlotButton.SlotType != weaponSlot.Type) continue;
                weaponSlotButton.SetWeapon(weaponSlot.WeaponName);
                foundValidWeaponSlotButton = true;
                break;
            }

            if (!foundValidWeaponSlotButton)
            {
                Debug.Log("Unable to find WeaponSlotButton for weapon slot of type: " + weaponSlot.Type);
            }
        }
    }

    private void SetInventory()
    {
        List<GameObject> inventory = LoadoutManager.GetInventory();

        int i = 0;
        foreach (InventorySlotButton inventorySlotButton in InventorySlotButtons)
        {
            if (inventorySlotButton.IsEmpty && inventory.Count > i)
            {
                inventorySlotButton.SetWeapon(inventory[i]);
                i++;
            }
            else
            {
                inventorySlotButton.Clear();
            }
        }
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
