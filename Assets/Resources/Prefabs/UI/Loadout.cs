using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Loadout : MonoBehaviour
{
    public static Loadout Inst { get; private set; }

    // WEAPON SLOTS
    public GameObject WeaponSlotContainer;
    private List<WeaponSlotButton> WeaponSlotButtons;
    private WeaponSlotButton CurrentWeaponSlotButton;
    private int CurrentWeaponSlotButtonIndex;

    // INVENTORY
    public GameObject InventorySlotContainer;
    private List<InventorySlotButton> InventorySlotButtons;
    private InventorySlotButton CurrentInventorySlotButton;
    private int CurrentInventorySlotButtonIndex;

    // UTILITY
    private string ActiveContainer = "WeaponSlots";
    private List<GameObject> Inventory;

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

    public void HandleBackClicked()
    {
        UIManager.Inst.DisableLoadoutUI();
        UIManager.Inst.EnableInterStageUI();
    }

    private void InitialiseInventorySlotButtons()
    {
        InventorySlotButtons = new List<InventorySlotButton>();
        int i = 0;
        foreach (Transform child in InventorySlotContainer.transform)
        {
            InventorySlotButton inventorySlotButton = child.GetComponent<InventorySlotButton>();
            inventorySlotButton.ListIndex = i;
            InventorySlotButtons.Add(child.GetComponent<InventorySlotButton>());
            i++;
        }
    }

    private void InitialiseWeaponSlotButtons()
    {
        WeaponSlotButtons = new List<WeaponSlotButton>();
        int i = 0;
        foreach (Transform child in WeaponSlotContainer.transform)
        {
            WeaponSlotButton weaponSlotButton = child.GetComponent<WeaponSlotButton>();
            weaponSlotButton.ListIndex = i;
            WeaponSlotButtons.Add(weaponSlotButton);
            i++;
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
        Inventory = LoadoutManager.GetInventory();

        int i = 0;
        foreach (InventorySlotButton inventorySlotButton in InventorySlotButtons)
        {
            if (inventorySlotButton.IsEmpty && Inventory.Count > i)
            {
                inventorySlotButton.SetWeapon(Inventory[i]);
                i++;
            }
            else
            {
                inventorySlotButton.Clear();
            }
        }
    }

    private bool EitherCurrentSlotsAreSelected()
    {
        if ((CurrentInventorySlotButton != null && CurrentInventorySlotButton.IsSelected)
        || (CurrentWeaponSlotButton != null && CurrentWeaponSlotButton.IsSelected))
        {
            return true;
        }
        else return false;
    }

    public void HandleMoveLeft()
    {
        if (ActiveContainer == "WeaponSlots") return;
        SetSelectedWeaponSlotButton(CurrentWeaponSlotButtonIndex);
    }
    public void HandleMoveRight()
    {
        if (ActiveContainer == "Inventory") return;
        SetSelectedInventorySlotButton(0);
    }
    public void HandleMoveUp()
    {
        if (ActiveContainer == "Inventory")
        {
            if (EitherCurrentSlotsAreSelected()) CurrentInventorySlotButtonIndex = (CurrentInventorySlotButtonIndex - 1 + Inventory.Count) % Inventory.Count;
            SetSelectedInventorySlotButton(CurrentInventorySlotButtonIndex);
        }
        else
        {
            if (EitherCurrentSlotsAreSelected()) CurrentWeaponSlotButtonIndex = (CurrentWeaponSlotButtonIndex - 1 + WeaponSlotButtons.Count) % WeaponSlotButtons.Count;
            SetSelectedWeaponSlotButton(CurrentWeaponSlotButtonIndex);
        }
    }
    public void HandleMoveDown()
    {
        if (ActiveContainer == "Inventory")
        {
            if (EitherCurrentSlotsAreSelected()) CurrentInventorySlotButtonIndex = (CurrentInventorySlotButtonIndex + 1) % Inventory.Count;
            SetSelectedInventorySlotButton(CurrentInventorySlotButtonIndex);
        }
        else
        {
            if (EitherCurrentSlotsAreSelected()) CurrentWeaponSlotButtonIndex = (CurrentWeaponSlotButtonIndex + 1) % WeaponSlotButtons.Count;
            SetSelectedWeaponSlotButton(CurrentWeaponSlotButtonIndex);
        }
    }

    private void DeselectCurrentButtons()
    {
        if (CurrentInventorySlotButton != null) CurrentInventorySlotButton.Deselect();
        if (CurrentWeaponSlotButton != null) CurrentWeaponSlotButton.Deselect();
    }
    public void SetSelectedWeaponSlotButton(int index)
    {
        DeselectCurrentButtons();
        CurrentWeaponSlotButton = WeaponSlotButtons[index];
        CurrentWeaponSlotButton.Select();
        CurrentWeaponSlotButtonIndex = index;
        ActiveContainer = "WeaponSlots";
    }

    public void SetSelectedInventorySlotButton(int index)
    {
        DeselectCurrentButtons();
        CurrentInventorySlotButton = InventorySlotButtons[index];
        CurrentInventorySlotButton.Select();
        CurrentInventorySlotButtonIndex = index;
        ActiveContainer = "Inventory";
    }

    public void HandleSelect()
    {
    }
    public void Back()
    {

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
}
