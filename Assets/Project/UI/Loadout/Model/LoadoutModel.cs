using System.Collections.Generic;
using Project.Combat.Weapons;
using Project.Core;
using Project.Ships;
using UnityEngine;

namespace Project.UI.Loadout.Model
{
	/// <summary>
	/// Model class for the Loadout UI system. Handles all data-related operations.
	/// </summary>
	public class LoadoutModel
	{
		// Singleton instance
		private static LoadoutModel _instance;
		public static LoadoutModel Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LoadoutModel();
				}
				return _instance;
			}
		}

		// Data properties
		public Dictionary<SlotType, List<WeaponBase>> AvailableWeapons { get; private set; }
		public List<WeaponSlot> ActiveWeaponSlots { get; private set; }
		public List<NodeData> WeaponNodeDataList { get; private set; } = new List<NodeData>();
		public List<List<NodeData>> WeaponNodeGroups { get; private set; } = new List<List<NodeData>>();

		// Constructor
		private LoadoutModel()
		{
			InitializeAvailableWeapons();
		}

		/// <summary>
		/// Initializes the available weapons dictionary with empty lists for each slot type.
		/// </summary>
		private void InitializeAvailableWeapons()
		{
			AvailableWeapons = new Dictionary<SlotType, List<WeaponBase>>
						{
								{ SlotType.Single, new List<WeaponBase>() },
								{ SlotType.Dual, new List<WeaponBase>() },
								{ SlotType.System, new List<WeaponBase>() }
						};
		}

		/// <summary>
		/// Updates the available weapons from the LoadoutManager.
		/// </summary>
		public void UpdateAvailableWeapons()
		{
			AvailableWeapons[SlotType.Single] = LoadoutManager.GetInventoryByType(SlotType.Single);
			AvailableWeapons[SlotType.Dual] = LoadoutManager.GetInventoryByType(SlotType.Dual);
			AvailableWeapons[SlotType.Dual].AddRange(LoadoutManager.GetInventoryByType(SlotType.Single));
			AvailableWeapons[SlotType.System] = LoadoutManager.GetInventoryByType(SlotType.System);
		}

		/// <summary>
		/// Loads the active weapon slots from the player ship.
		/// </summary>
		public void LoadActiveWeaponSlots()
		{
			ActiveWeaponSlots = PlayerManager.Inst.ActivePlayerShip.GetActiveWeaponSlots();
		}

		/// <summary>
		/// Processes the weapon slots and creates node data for each attach point.
		/// </summary>
		public void ProcessWeaponSlots()
		{
			WeaponNodeDataList.Clear();
			WeaponNodeGroups.Clear();

			int nodeId = 0;
			foreach (WeaponSlot weaponSlot in ActiveWeaponSlots)
			{
				List<NodeData> relatedNodes = new List<NodeData>();

				// Process attach points in reverse order to match original implementation
				for (int j = weaponSlot.AttachPoints.Count - 1; j >= 0; j--)
				{
					AttachPoint attachPoint = weaponSlot.AttachPoints[j];

					// Create node data
					NodeData nodeData = new NodeData
					{
						NodeId = nodeId,
						AttachPoint = attachPoint,
						WeaponSlot = weaponSlot,
						XPos = attachPoint.transform.localPosition.x,
						YPos = attachPoint.transform.localPosition.y,
						Side = attachPoint.Side,
						Position = new Vector3(
									attachPoint.transform.position.x,
									attachPoint.transform.position.y + 0.05f,
									0) // Z will be set by the view
					};

					// Add to collections
					relatedNodes.Add(nodeData);
					WeaponNodeDataList.Add(nodeData);
					nodeId++;
				}

				// Add the group to the groups list
				WeaponNodeGroups.Add(relatedNodes);
			}

			// Sort groups by Y position (highest first)
			WeaponNodeGroups.Sort((listA, listB) => listB[0].YPos.CompareTo(listA[0].YPos));
		}

		/// <summary>
		/// Equips a weapon to a weapon slot.
		/// </summary>
		/// <param name="weaponBase">The weapon to equip</param>
		/// <param name="slotId">The ID of the slot to equip to</param>
		/// <returns>The updated weapon slot, or null if the operation failed</returns>
		public WeaponSlot EquipWeapon(WeaponBase weaponBase, string slotId)
		{
			GameObject targetWeaponPrefab = LoadoutManager.FetchWeaponPefab(weaponBase);
			if (targetWeaponPrefab == null)
			{
				Debug.LogError("Could not identify weapon");
				return null;
			}

			// Try to parse the slotId as an integer
			if (int.TryParse(slotId, out int slotIdInt))
			{
				WeaponSlot weaponSlot = LoadoutManager.EquipWeaponToSlot(targetWeaponPrefab, slotIdInt);
				if (weaponSlot == null)
				{
					Debug.LogError($"Failed to equip weapon: {weaponBase.WeaponName}");
					return null;
				}

				MusicManager.Inst.PlaySoundEffect("GunLoad", 1f);
				return weaponSlot;
			}
			else
			{
				Debug.LogError($"Failed to parse slotId: {slotId}. LoadoutManager.EquipWeaponToSlot requires an integer slotId.");
				return null;
			}
		}

		/// <summary>
		/// Finds the index of a weapon in the available weapons list.
		/// </summary>
		/// <param name="targetWeapon">The weapon to find</param>
		/// <param name="slotType">The slot type to search in</param>
		/// <returns>The index of the weapon, or -1 if not found</returns>
		public int FindWeaponIndex(WeaponBase targetWeapon, SlotType slotType)
		{
			List<WeaponBase> weapons = AvailableWeapons[slotType];

			for (int i = 0; i < weapons.Count; i++)
			{
				WeaponBase weapon = weapons[i];
				if (weapon.WeaponName == targetWeapon.WeaponName &&
						weapon.Description == targetWeapon.Description &&
						weapon.WeaponIcon == targetWeapon.WeaponIcon)
				{
					return i;
				}
			}

			Debug.LogError("WeaponIndex not found...");
			return -1;
		}

		/// <summary>
		/// Calculates the positions for weapon node selectors.
		/// </summary>
		/// <param name="selectorDistance">The distance from the center for selectors</param>
		/// <returns>A list of positions for the selectors</returns>
		public List<Vector3> CalculateSelectorPositions(float selectorDistance)
		{
			List<Vector3> positions = new List<Vector3>();
			int nodeCount = WeaponNodeDataList.Count;

			if (nodeCount == 0)
			{
				Debug.LogError("No weapon nodes available for calculating selector positions");
				return positions;
			}

			// Determine if the first node is centered
			bool firstNodeIsCentered = WeaponNodeDataList[0].XPos == 0;

			// Angle between each selector
			float anglePerSelector = 360f / nodeCount;

			// Calculate positions
			for (int i = 0; i < nodeCount; i++)
			{
				float angle;
				if (i == 0)
				{
					// First position
					angle = firstNodeIsCentered ? 0 : (anglePerSelector / 2);
				}
				else
				{
					// Subsequent positions
					float baseAngle = firstNodeIsCentered ? 0 : (anglePerSelector / 2);
					angle = baseAngle + (anglePerSelector * i);
				}

				positions.Add(CalculateSelectorPosition(angle, selectorDistance));
			}

			return positions;
		}

		/// <summary>
		/// Calculates a single selector position based on an angle.
		/// </summary>
		/// <param name="angleDegrees">The angle in degrees</param>
		/// <param name="distance">The base distance from center</param>
		/// <returns>The calculated position</returns>
		private Vector3 CalculateSelectorPosition(float angleDegrees, float distance)
		{
			// Calculate distance modifier based on the original angle
			float normalisedAngle = Mathf.Abs((angleDegrees % 180) - 90);

			// Use a non-linear function to adjust the distanceModifier
			float t = normalisedAngle / 90;
			float distanceModifier = Mathf.SmoothStep(0, 1f, t);

			// Apply a 90-degree counterclockwise offset for positioning
			float adjustedAngle = angleDegrees + 90;

			// Calculate offsets using the adjusted angle
			float offsetX = -Mathf.Cos(adjustedAngle * Mathf.Deg2Rad) * (distance * (1f + distanceModifier));
			float offsetY = Mathf.Sin(adjustedAngle * Mathf.Deg2Rad) * distance;
			offsetY *= angleDegrees % 90 == 0 ? 1f : 0.9f;

			// Create the new position vector
			Vector3 offset = new Vector3(offsetX, offsetY, 0);

			// Transform the offset to local coordinates
			Vector3 localPosition = PlayerManager.Inst.ActivePlayerShip.transform.localPosition + offset;

			return localPosition;
		}

		/// <summary>
		/// Resets the model data.
		/// </summary>
		public void Reset()
		{
			WeaponNodeDataList.Clear();
			WeaponNodeGroups.Clear();
		}
	}

	/// <summary>
	/// Data class for weapon nodes.
	/// </summary>
	public class NodeData
	{
		public int NodeId;
		public AttachPoint AttachPoint;
		public WeaponSlot WeaponSlot;
		public float XPos;
		public float YPos;
		public RelativeSide Side;
		public Vector3 Position;
		public List<NodeData> LinkedNodes = new List<NodeData>();
	}
}