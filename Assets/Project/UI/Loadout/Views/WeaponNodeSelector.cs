using Project.Combat.Weapons;
using Project.Core;
using Project.Ships;
using Project.UI.Loadout.Events;
using Project.UI.Loadout.Model;
using TMPro;
using UnityEngine;

namespace Project.UI.Loadout
{
	/// <summary>
	/// Represents a weapon selector in the Loadout UI.
	/// </summary>
	public class WeaponNodeSelector : MonoBehaviour
	{
		// Inspector
		[Header("UI References")]
		[SerializeField] private WeaponNodeSelectorList list;
		[SerializeField] private SpriteRenderer slotTypeIcon;
		[SerializeField] private TextMeshProUGUI slotTypeText;
		[SerializeField] public TextMeshProUGUI ID;

		[Header("Connection Ports")]
		[SerializeField] private GameObject leftPort;
		[SerializeField] private GameObject rightPort;
		[SerializeField] private GameObject topPort;
		[SerializeField] private GameObject bottomPort;

		// Data properties
		private SlotType slotType;
		private WeaponNode weaponNode;

		// State tracking
		private bool isHovered = false;
		private bool isSelected = false;

		void Awake()
		{
			// Validate port references
			if (leftPort == null || rightPort == null || topPort == null || bottomPort == null)
			{
				Debug.LogWarning("Port references are missing on WeaponNodeSelector");
			}

			// Validate other references
			if (list == null)
			{
				Debug.LogError("WeaponNodeSelectorList reference is missing on WeaponNodeSelector");
			}

			if (slotTypeText == null)
			{
				Debug.LogError("SlotTypeText reference is missing on WeaponNodeSelector");
			}

			if (ID == null)
			{
				Debug.LogError("ID reference is missing on WeaponNodeSelector");
			}
		}

		/// <summary>
		/// Updates the content of this selector with the specified data.
		/// </summary>
		/// <param name="attachPoint">The attach point this selector is associated with</param>
		/// <param name="weaponSlot">The weapon slot this selector is associated with</param>
		/// <param name="weaponNode">The weapon node this selector is associated with</param>
		/// <param name="nodeId">The ID of the node</param>
		public void UpdateContent(AttachPoint attachPoint, WeaponSlot weaponSlot, WeaponNode weaponNode, int nodeId)
		{
			if (attachPoint == null)
			{
				Debug.LogError("AttachPoint is null in UpdateContent");
				return;
			}

			if (weaponSlot == null)
			{
				Debug.LogError("WeaponSlot is null in UpdateContent");
				return;
			}

			if (weaponNode == null)
			{
				Debug.LogError("WeaponNode is null in UpdateContent");
				return;
			}

			if (ID != null)
			{
				ID.text = nodeId.ToString();
			}

			this.weaponNode = weaponNode;

			if (attachPoint.AttachedWeapon == null)
			{
				Debug.LogError("AttachedWeapon is null on AttachPoint");
				return;
			}

			WeaponBase assignedWeapon = attachPoint.AttachedWeapon.GetComponent<WeaponBase>();
			if (assignedWeapon == null)
			{
				Debug.LogError("WeaponBase component not found on AttachedWeapon");
				return;
			}

			slotType = assignedWeapon.SlotType;

			if (list != null)
			{
				list.Init(assignedWeapon, LoadoutModel.Instance.AvailableWeapons[slotType], nodeId);
			}

			SetWeaponType(weaponSlot.Type);
		}

		/// <summary>
		/// Sets the weapon type display.
		/// </summary>
		/// <param name="slotType">The type of weapon slot</param>
		private void SetWeaponType(SlotType slotType)
		{
			if (slotTypeText == null) return;

			switch (slotType)
			{
				case SlotType.Single:
					slotTypeText.text = "Single";
					break;

				case SlotType.Dual:
					slotTypeText.text = "Dual";
					break;

				case SlotType.System:
					slotTypeText.text = "System";
					break;

				default:
					return;
			}
		}

		/// <summary>
		/// Scrolls the weapon list up.
		/// </summary>
		public void ScrollUp()
		{
			if (list != null)
			{
				list.ScrollUp();
				LoadoutEvents.TriggerListScrolled(list, -1);
			}
		}

		/// <summary>
		/// Scrolls the weapon list down.
		/// </summary>
		public void ScrollDown()
		{
			if (list != null)
			{
				list.ScrollDown();
				LoadoutEvents.TriggerListScrolled(list, 1);
			}
		}

		/// <summary>
		/// Handles selection of this selector.
		/// </summary>
		public void HandleSelect()
		{
			if (list == null) return;

			if (list.State == WeaponNodeSelectorList.ListState.Active)
			{
				// List is open, so select the current weapon and close the list
				SelectWeapon();
				DeactivateList();
				isSelected = false;
				LoadoutEvents.TriggerSelectorDeselected(this);
			}
			else
			{
				// List is closed, so open it
				ActivateList();
				isSelected = true;
				LoadoutEvents.TriggerSelectorSelected(this);
			}
		}

		/// <summary>
		/// Handles deselection of this selector.
		/// </summary>
		public void HandleDeselect()
		{
			if (list == null) return;

			if (list.State == WeaponNodeSelectorList.ListState.Active)
			{
				DeactivateList();
			}
			isSelected = false;
			LoadoutEvents.TriggerSelectorDeselected(this);
		}

		/// <summary>
		/// Selects the currently active weapon in the list.
		/// </summary>
		private void SelectWeapon()
		{
			if (list == null || list.ActiveCell == null || weaponNode == null || weaponNode.WeaponSlot == null)
			{
				Debug.LogError("Required references are null in SelectWeapon");
				return;
			}

			int weaponIndex = list.ActiveCell.WeaponListIndex;
			if (weaponIndex < 0 || weaponIndex >= list.AvailableWeapons.Count)
			{
				Debug.LogError($"Invalid weaponIndex {weaponIndex} in SelectWeapon. AvailableWeapons.Count = {list.AvailableWeapons.Count}");
				return;
			}

			WeaponBase selectedWeapon = list.AvailableWeapons[weaponIndex];

			// Trigger the weapon selected event
			LoadoutEvents.TriggerWeaponSelected(selectedWeapon);

			// Convert the slot ID to a string
			string slotIdString = weaponNode.WeaponSlot.id.ToString();

			// Equip the weapon
			WeaponSlot updatedSlot = LoadoutModel.Instance.EquipWeapon(selectedWeapon, slotIdString);

			if (updatedSlot != null)
			{
				// Trigger the weapon equipped event
				LoadoutEvents.TriggerWeaponEquipped(selectedWeapon, slotIdString);

				// Refresh the node to reflect the new weapon
				weaponNode.RefreshSelector();
			}
		}

		/// <summary>
		/// Activates the weapon list.
		/// </summary>
		private void ActivateList()
		{
			if (list != null)
			{
				list.ActivateList();
				LoadoutEvents.TriggerListActivated(list);
			}
		}

		/// <summary>
		/// Deactivates the weapon list.
		/// </summary>
		private void DeactivateList()
		{
			if (list != null)
			{
				list.DeactivateList();
				LoadoutEvents.TriggerListDeactivated(list);
			}
		}

		/// <summary>
		/// Enables the hover state for this selector.
		/// </summary>
		public void EnableHoverState()
		{
			if (isHovered) return;

			if (list != null)
			{
				list.EnableHoverState();
			}

			isHovered = true;
			LoadoutEvents.TriggerSelectorHoverEnter(this);
		}

		/// <summary>
		/// Disables the hover state for this selector.
		/// </summary>
		public void DisableHoverState()
		{
			if (!isHovered) return;

			if (list != null)
			{
				list.DisableHoverState();
			}

			isHovered = false;
			LoadoutEvents.TriggerSelectorHoverExit(this);
		}

		/// <summary>
		/// Gets the position of the left port.
		/// </summary>
		/// <returns>The position of the left port</returns>
		public Vector3 GetLeftPortPosition()
		{
			return leftPort != null ? leftPort.transform.position : transform.position;
		}

		/// <summary>
		/// Gets the position of the right port.
		/// </summary>
		/// <returns>The position of the right port</returns>
		public Vector3 GetRightPortPosition()
		{
			return rightPort != null ? rightPort.transform.position : transform.position;
		}

		/// <summary>
		/// Gets the position of the top port.
		/// </summary>
		/// <returns>The position of the top port</returns>
		public Vector3 GetTopPortPosition()
		{
			return topPort != null ? topPort.transform.position : transform.position;
		}

		/// <summary>
		/// Gets the position of the bottom port.
		/// </summary>
		/// <returns>The position of the bottom port</returns>
		public Vector3 GetBottomPortPosition()
		{
			return bottomPort != null ? bottomPort.transform.position : transform.position;
		}
	}
}