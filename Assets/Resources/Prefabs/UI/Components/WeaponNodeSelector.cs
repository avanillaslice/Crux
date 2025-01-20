using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponNodeSelector : MonoBehaviour
{
	// Inspector
	public GameObject WeaponList;
	public Image WeaponIcon;
	public TextMeshProUGUI WeaponName;
	public TextMeshProUGUI WeaponType;
	public GameObject HoverStateComponent; // Contains Light/Medium/Heavy

	// Data
	private bool ListIsActive = false;

	void Awake()
	{
		// Disable Hover and Selected components
		// HoverStateComponent.SetActive(false);
	}

	public void UpdateWeaponDetails(AttachPoint attachPoint, WeaponSlot weaponSlot)
	{
		WeaponBase assignedWeapon = attachPoint.AttachedWeapon.GetComponent<WeaponBase>();
		// ! REQUIRES UI ELEMENT
		// WeaponIcon = assignedWeapon.WeaponIcon;
		WeaponName.text = assignedWeapon.WeaponName;
		// SetWeaponType(weaponSlot.Type);
	}

	// Probably needs to be WeaponSlotType and not WeaponType
	private void SetWeaponType(SlotType weaponType)
	{
		switch (weaponType)
		{
			case global::SlotType.Single:
				{
					WeaponType.text = "Single";
					// Set Color
					break;
				}
				;
			case global::SlotType.Dual:
				{
					WeaponType.text = "Dual";
					// Set Color
					break;
				}
			case global::SlotType.System:
				{
					WeaponType.text = "System";
					// Set Color
					break;
				}
			default: return;
		}
	}

	public void ScrollUp() {}

	public void ScrollDown() {}

	public void HandleSelect()
	{
		if (WeaponList.activeSelf) {
			SelectWeapon();
			DeactivateList();
		}
		else ActivateList();
	}

	private void SelectWeapon() {

	}

	public void HandleDeselect()
	{
		// which will do things
	}
	private void ActivateList()
	{
		WeaponList.SetActive(true); // OnEnable animation
	}

	private void DeactivateList()
	{
		WeaponList.SetActive(false); // OnDisable animation
	}

	public void EnableHoverState()
	{
		// Enables SlotTypeUIComponent
		HoverStateComponent.SetActive(true);
	}
	public void DisableHoverState()
	{
		// Disables SlotTypeUIComponent
		HoverStateComponent.SetActive(false);
	}

}