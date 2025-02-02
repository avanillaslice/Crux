using TMPro;
using UnityEngine;

public class WeaponNodeSelector : MonoBehaviour
{
	// Inspector
	public WeaponNodeSelectorList List;
	public SpriteRenderer SlotTypeIcon; // To left of selector
	public TextMeshProUGUI SlotTypeText;
	public TextMeshProUGUI ID;
	// public GameObject HoverStateComponent; // Contains Light/Medium/Heavy

	// Data
	private SlotType SlotType;
	private WeaponNode WeaponNode;

	void Awake()
	{
		// Disable Hover and Selected components
		// HoverStateComponent.SetActive(false);
	}

	public void UpdateContent(AttachPoint attachPoint, WeaponSlot weaponSlot, WeaponNode weaponNode, int nodeId)
	{
		ID.text = nodeId.ToString();
		WeaponNode = weaponNode;
		WeaponBase assignedWeapon = attachPoint.AttachedWeapon.GetComponent<WeaponBase>();
		SlotType = assignedWeapon.SlotType;
		List.Init(assignedWeapon, LoadoutUI.Inst.AvailableWeapons[SlotType], nodeId);
		SetWeaponType(weaponSlot.Type);
	}

	// Probably needs to be WeaponSlotType and not WeaponType
	private void SetWeaponType(SlotType slotType)
	{
		switch (slotType)
		{
			case global::SlotType.Single:
				{
					SlotTypeText.text = "Single";
					// Set Color
					break;
				}
				;
			case global::SlotType.Dual:
				{
					SlotTypeText.text = "Dual";
					// Set Color
					break;
				}
			case global::SlotType.System:
				{
					SlotTypeText.text = "System";
					// Set Color
					break;
				}
			default: return;
		}
	}

	public void ScrollUp() {
		List.ScrollUp();
	}

	public void ScrollDown() {
		List.ScrollDown();
	}

	public void HandleSelect()
	{
		if (List.State == WeaponNodeSelectorList.ListState.Active) {
			SelectWeapon();
			DeactivateList();
		}
		else ActivateList();
	}

	public void HandleDeselect()
	{
		if (List.State != WeaponNodeSelectorList.ListState.Active) return;
		DeactivateList();
	}

	private void SelectWeapon() {
		GameObject targetWeaponPrefab = LoadoutManager.FetchWeaponPefab(List.AvailableWeapons[List.ActiveCell.WeaponListIndex]);
		if (targetWeaponPrefab == null) {
			Debug.LogError("Could not identify weapon");
			return;
		}
		WeaponSlot weaponSlot = LoadoutManager.EquipWeaponToSlot(targetWeaponPrefab, WeaponNode.WeaponSlot.id);
		if (weaponSlot == null) Debug.LogError($"Failed to equip weapom: {List.AvailableWeapons[List.ActiveCell.WeaponListIndex].WeaponName}");
		else MusicManager.Inst.PlaySoundEffect("GunLoad", 1f);
	}

	private void ActivateList()
	{
		List.ActivateList(); // OnEnable animation?
	}

	private void DeactivateList()
	{
		List.DeactivateList(); // OnDisable animation
	}

	public void EnableHoverState()
	{
		// Enables SlotTypeUIComponent
		// HoverStateComponent.SetActive(true);
	}
	public void DisableHoverState()
	{
		// Disables SlotTypeUIComponent
		// HoverStateComponent.SetActive(false);
	}

}