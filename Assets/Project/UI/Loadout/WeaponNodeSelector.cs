using Project.Combat.Weapons;
using Project.Core;
using Project.Ships;
using TMPro;
using UnityEngine;

namespace Project.UI.Loadout
{
	public class WeaponNodeSelector : MonoBehaviour
	{
		// Inspector
		public WeaponNodeSelectorList List;
		public SpriteRenderer SlotTypeIcon; // To left of selector
		public TextMeshProUGUI SlotTypeText;
		public TextMeshProUGUI ID;
		// public GameObject HoverStateComponent; // Contains Light/Medium/Heavy

		[Header("Connection Ports")]
		public GameObject LeftPort;
		public GameObject RightPort;
		public GameObject TopPort;
		public GameObject BottomPort;

		// Data
		private SlotType SlotType;
		private WeaponNode WeaponNode;

		// State tracking
		private bool IsHovered = false;
		private bool IsSelected = false;

		void Awake()
		{
			// Validate port references
			if (LeftPort == null || RightPort == null || TopPort == null || BottomPort == null)
			{
				Debug.LogWarning("Port references are missing on WeaponNodeSelector");
			}
			
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
				case global::Project.Ships.SlotType.Single:
				{
					SlotTypeText.text = "Single";
					// Set Color
					break;
				}
					;
				case global::Project.Ships.SlotType.Dual:
				{
					SlotTypeText.text = "Dual";
					// Set Color
					break;
				}
				case global::Project.Ships.SlotType.System:
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

		/// <summary>
		/// Handles selection state changes. If the list is active, selects the current weapon and deactivates the list.
		/// If the list is not active, activates the list.
		/// </summary>
		public void HandleSelect()
		{
			if (List.State == WeaponNodeSelectorList.ListState.Active) {
				// List is open, so select the current weapon and close the list
				SelectWeapon();
				DeactivateList();
				IsSelected = false;
			}
			else {
				// List is closed, so open it
				ActivateList();
				IsSelected = true;
			}
		}

		/// <summary>
		/// Handles deselection by deactivating the list if it's active.
		/// </summary>
		public void HandleDeselect()
		{
			if (List.State == WeaponNodeSelectorList.ListState.Active) {
				DeactivateList();
			}
			IsSelected = false;
		}

		/// <summary>
		/// Selects the currently active weapon in the list and equips it to the weapon slot.
		/// </summary>
		private void SelectWeapon() {
			GameObject targetWeaponPrefab = LoadoutManager.FetchWeaponPefab(List.AvailableWeapons[List.ActiveCell.WeaponListIndex]);
			if (targetWeaponPrefab == null) {
				Debug.LogError("Could not identify weapon");
				return;
			}
			
			WeaponSlot weaponSlot = LoadoutManager.EquipWeaponToSlot(targetWeaponPrefab, WeaponNode.WeaponSlot.id);
			if (weaponSlot == null) {
				Debug.LogError($"Failed to equip weapon: {List.AvailableWeapons[List.ActiveCell.WeaponListIndex].WeaponName}");
			}
			else {
				MusicManager.Inst.PlaySoundEffect("GunLoad", 1f);
				// Refresh the node to reflect the new weapon
				WeaponNode.RefreshSelector();
			}
		}

		/// <summary>
		/// Activates the weapon list for selection.
		/// </summary>
		private void ActivateList()
		{
			List.ActivateList();
		}

		/// <summary>
		/// Deactivates the weapon list.
		/// </summary>
		private void DeactivateList()
		{
			List.DeactivateList();
		}

		/// <summary>
		/// Enables the hover state visual effects.
		/// </summary>
		public void EnableHoverState()
		{
			if (IsHovered) return;
			
			List.EnableHoverState();
			IsHovered = true;
		}

		/// <summary>
		/// Disables the hover state visual effects.
		/// </summary>
		public void DisableHoverState()
		{
			if (!IsHovered) return;
			
			List.DisableHoverState();
			IsHovered = false;
		}

		// Returns the position of the left port for line connections
		public Vector3 GetLeftPortPosition()
		{
			if (LeftPort != null)
			{
				return LeftPort.transform.position;
			}
			
			// Fallback to the selector's position if the port is missing
			return transform.position;
		}
		
		// Returns the position of the right port for line connections
		public Vector3 GetRightPortPosition()
		{
			if (RightPort != null)
			{
				return RightPort.transform.position;
			}
			
			// Fallback to the selector's position if the port is missing
			return transform.position;
		}

		public Vector3 GetTopPortPosition()
		{
			if (TopPort != null)
			{
				return TopPort.transform.position;
			}

			// Fallback to the selector's position if the port is missing
			return transform.position;
		}

		public Vector3 GetBottomPortPosition()
		{
			if (BottomPort != null)
			{
				return BottomPort.transform.position;
			}

			// Fallback to the selector's position if the port is missing
			return transform.position;
		}
	}
}