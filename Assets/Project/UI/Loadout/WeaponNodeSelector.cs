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
			List.EnableHoverState();
			// Enables SlotTypeUIComponent
			// HoverStateComponent.SetActive(true);
		}
		public void DisableHoverState()
		{
			List.DisableHoverState();
			// Disables SlotTypeUIComponent
			// HoverStateComponent.SetActive(false);
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