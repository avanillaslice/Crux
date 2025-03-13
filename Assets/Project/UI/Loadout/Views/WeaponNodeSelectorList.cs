using System.Collections.Generic;
using Project.Combat.Weapons;
using Project.Core;
using Project.UI.Loadout.Events;
using Project.UI.Loadout.Model;
using UnityEngine;

namespace Project.UI.Loadout
{
	/// <summary>
	/// Represents a list of available weapons for a weapon node.
	/// </summary>
	public class WeaponNodeSelectorList : MonoBehaviour
	{
		// Types
		public enum ScrollDirection
		{
			Up,
			Down
		}

		public enum ListState
		{
			Inactive,
			Active,
		}

		public class PosData
		{
			public int Position;
			public float YPos;
			public float Scale;
			public float Opacity;
		}

		// Position data for cells
		public readonly Dictionary<int, PosData> PosDataDict = new Dictionary<int, PosData>
				{
						{ 1, new PosData { Position = 1, YPos = 130f, Scale = 0.6f, Opacity = 0.0f } },  // Next top cell
            { 2, new PosData { Position = 2, YPos = 95f, Scale = 0.8f, Opacity = 0.35f } },  // Above cell
            { 3, new PosData { Position = 3, YPos = 0f, Scale = 1.0f, Opacity = 1.0f } },    // Active cell
            { 4, new PosData { Position = 4, YPos = -95f, Scale = 0.8f, Opacity = 0.35f } }, // Below cell
            { 5, new PosData { Position = 5, YPos = -130f, Scale = 0.6f, Opacity = 0.0f } }, // Bottom cell
            { 6, new PosData { Position = 6, YPos = -245f, Scale = 0.4f, Opacity = 0.0f } }, // Next bottom cell
        };

		// Inspector
		public int ListId;

		// Data properties
		private bool isInitialized = false;
		private Queue<ScrollDirection> scrollQueue = new Queue<ScrollDirection>();
		public bool IsScrolling { get; private set; } = false;
		public int ScrollBuffer { get; private set; } = 0;
		public ListState State { get; private set; } = ListState.Inactive;
		public WeaponNodeSelectorListCell ActiveCell { get; private set; }
		public List<WeaponBase> AvailableWeapons { get; private set; } = new List<WeaponBase>();

		// References
		private List<GameObject> cells = new List<GameObject>();

		void Awake()
		{
			InitializeCells();
		}

		/// <summary>
		/// Initializes the cells for this list.
		/// </summary>
		private void InitializeCells()
		{
			// Create cells
			for (int i = 1; i < 7; i++)
			{
				GameObject newCell = Instantiate(AssetManager.WeaponNodeSelectorListCellPrefab, gameObject.transform);
				if (newCell == null)
				{
					Debug.LogError($"Failed to instantiate WeaponNodeSelectorListCellPrefab for position {i}");
					continue;
				}

				WeaponNodeSelectorListCell cellComponent = newCell.GetComponent<WeaponNodeSelectorListCell>();
				if (cellComponent == null)
				{
					Debug.LogError($"WeaponNodeSelectorListCell component not found on instantiated prefab for position {i}");
					Destroy(newCell);
					continue;
				}

				cells.Add(newCell);
				cellComponent.Init(PosDataDict[i]);
			}
		}

		/// <summary>
		/// Initializes the list with the specified data.
		/// </summary>
		/// <param name="initialCellData">The initial weapon data</param>
		/// <param name="availableWeapons">The list of available weapons</param>
		/// <param name="listId">The ID of this list</param>
		public void Init(WeaponBase initialCellData, List<WeaponBase> availableWeapons, int listId)
		{
			if (initialCellData == null)
			{
				Debug.LogError("initialCellData is null in WeaponNodeSelectorList.Init");
				return;
			}

			if (availableWeapons == null || availableWeapons.Count == 0)
			{
				Debug.LogError("availableWeapons is null or empty in WeaponNodeSelectorList.Init");
				return;
			}

			AvailableWeapons = availableWeapons;

			// Find the index of the weapon
			int weaponIndex = FindWeaponIndex(initialCellData);
			if (weaponIndex == -1)
			{
				Debug.LogError($"Invalid weaponIndex for {initialCellData.WeaponName}");
				// Use the first weapon as a fallback
				weaponIndex = 0;
			}

			ListId = listId;
			SetDataOnRemainingCells(weaponIndex);
			DeactivateList();
			isInitialized = true;
		}

		/// <summary>
		/// Finds the index of a weapon in the available weapons list.
		/// </summary>
		/// <param name="targetWeapon">The weapon to find</param>
		/// <returns>The index of the weapon, or -1 if not found</returns>
		private int FindWeaponIndex(WeaponBase targetWeapon)
		{
			for (int i = 0; i < AvailableWeapons.Count; i++)
			{
				WeaponBase weapon = AvailableWeapons[i];
				// Compare based on a unique property or set of properties
				if (weapon.WeaponName == targetWeapon.WeaponName &&
					weapon.Description == targetWeapon.Description &&
					weapon.WeaponIcon == targetWeapon.WeaponIcon)
				{
					return i;
				}
			}

			Debug.LogWarning($"WeaponIndex not found for {targetWeapon.WeaponName}");
			return -1; // Return -1 if not found
		}

		/// <summary>
		/// Sets the data on all cells based on the initial weapon index.
		/// </summary>
		/// <param name="initialWeaponIndex">The index of the initial weapon</param>
		private void SetDataOnRemainingCells(int initialWeaponIndex)
		{
			if (cells.Count < 6)
			{
				Debug.LogError("Not enough cells initialized in WeaponNodeSelectorList");
				return;
			}

			for (int i = 0; i < 6; i++)
			{
				WeaponNodeSelectorListCell cell = cells[i].GetComponent<WeaponNodeSelectorListCell>();
				if (cell == null)
				{
					Debug.LogError($"WeaponNodeSelectorListCell component not found on cell {i}");
					continue;
				}

				if (i == 2) ActiveCell = cell;

				// Determine correct weaponIndex with wrap-around logic
				int weaponIndex = (initialWeaponIndex + i - 2 + AvailableWeapons.Count) % AvailableWeapons.Count;

				if (weaponIndex < 0 || weaponIndex >= AvailableWeapons.Count)
				{
					Debug.LogError($"Invalid weaponIndex {weaponIndex} for cell {i}. AvailableWeapons.Count = {AvailableWeapons.Count}");
					continue;
				}

				WeaponBase relevantWeapon = AvailableWeapons[weaponIndex];
				cell.SetData(weaponIndex, relevantWeapon.WeaponName, relevantWeapon.Description, relevantWeapon.WeaponIcon, ListId);
			}
		}

		/// <summary>
		/// Activates the list for selection.
		/// </summary>
		public void ActivateList()
		{
			if (!isInitialized) return;

			foreach (GameObject cell in cells)
			{
				WeaponNodeSelectorListCell cellComponent = cell.GetComponent<WeaponNodeSelectorListCell>();
				if (cellComponent == null) continue;

				if (cellComponent.ListPosition == 3)
				{
					cellComponent.Animator.Play("ActivateCell");
					continue;
				}
				cell.SetActive(true);
			}

			State = ListState.Active;
		}

		/// <summary>
		/// Deactivates the list.
		/// </summary>
		public void DeactivateList()
		{
			foreach (GameObject cell in cells)
			{
				WeaponNodeSelectorListCell cellComponent = cell.GetComponent<WeaponNodeSelectorListCell>();
				if (cellComponent == null) continue;

				if (cellComponent.ListPosition == 3)
				{
					if (isInitialized) cellComponent.Animator.Play("DeactivateCell");
					continue;
				}

				cell.SetActive(false);
			}

			IsScrolling = false;
			scrollQueue.Clear();
			State = ListState.Inactive;
		}

		/// <summary>
		/// Scrolls the list up.
		/// </summary>
		public void ScrollUp()
		{
			if (!isInitialized) return;

			if (IsScrolling)
			{
				if (scrollQueue.Count < 3) scrollQueue.Enqueue(ScrollDirection.Up);
				return;
			}

			IsScrolling = true;

			// Shift cells up
			GameObject topCell = cells[0];

			int i = 0;
			foreach (GameObject cell in cells)
			{
				WeaponNodeSelectorListCell cellComponent = cell.GetComponent<WeaponNodeSelectorListCell>();
				if (cellComponent == null) continue;
				if (cellComponent == topCell.GetComponent<WeaponNodeSelectorListCell>()) continue;
				if ((cellComponent.ListPosition - 1) < 0) continue;

				if (i == 4) cellComponent.OnScrollComplete += HandleScrollComplete;
				if (cellComponent.ListPosition - 1 == 3) ActiveCell = cellComponent;

				cellComponent.Scroll(PosDataDict[cellComponent.ListPosition - 1]);
				i++;
			}

			// Fetch weaponIndex from last cell, set incremented value on top cell, then move it to last place
			WeaponNodeSelectorListCell lastCellComponent = cells[cells.Count - 1].GetComponent<WeaponNodeSelectorListCell>();
			if (lastCellComponent == null)
			{
				Debug.LogError("Last cell component is null in ScrollUp");
				IsScrolling = false;
				return;
			}

			int weaponIndex = (lastCellComponent.WeaponListIndex + 1) % AvailableWeapons.Count;

			cells.RemoveAt(0);
			cells.Add(topCell); // Move to last position

			WeaponNodeSelectorListCell topCellComponent = topCell.GetComponent<WeaponNodeSelectorListCell>();
			if (topCellComponent == null)
			{
				Debug.LogError("Top cell component is null in ScrollUp");
				IsScrolling = false;
				return;
			}

			topCellComponent.SetData(weaponIndex, AvailableWeapons[weaponIndex].WeaponName, AvailableWeapons[weaponIndex].Description, AvailableWeapons[weaponIndex].WeaponIcon, ListId);
			topCellComponent.Init(PosDataDict[6]); // Use the last position in PosDataDict
		}

		/// <summary>
		/// Scrolls the list down.
		/// </summary>
		public void ScrollDown()
		{
			if (!isInitialized) return;

			if (IsScrolling)
			{
				if (scrollQueue.Count < 3) scrollQueue.Enqueue(ScrollDirection.Down);
				return;
			}

			IsScrolling = true;

			// Shift cells down
			GameObject bottomCell = cells[cells.Count - 1];

			int i = 0;
			foreach (GameObject cell in cells)
			{
				WeaponNodeSelectorListCell cellComponent = cell.GetComponent<WeaponNodeSelectorListCell>();
				if (cellComponent == null) continue;
				if (cellComponent == bottomCell.GetComponent<WeaponNodeSelectorListCell>()) continue;
				if ((cellComponent.ListPosition + 1) > PosDataDict.Count) continue;

				if (i == 4) cellComponent.OnScrollComplete += HandleScrollComplete;
				if (cellComponent.ListPosition + 1 == 3) ActiveCell = cellComponent;

				cellComponent.Scroll(PosDataDict[cellComponent.ListPosition + 1]);
				i++;
			}

			// Fetch WeaponListIndex from current first cell, move bottom cell to the top position
			WeaponNodeSelectorListCell firstCellComponent = cells[0].GetComponent<WeaponNodeSelectorListCell>();
			if (firstCellComponent == null)
			{
				Debug.LogError("First cell component is null in ScrollDown");
				IsScrolling = false;
				return;
			}

			int weaponIndex = (firstCellComponent.WeaponListIndex - 1 + AvailableWeapons.Count) % AvailableWeapons.Count;

			cells.RemoveAt(cells.Count - 1);
			cells.Insert(0, bottomCell); // Move to top position

			WeaponNodeSelectorListCell bottomCellComponent = bottomCell.GetComponent<WeaponNodeSelectorListCell>();
			if (bottomCellComponent == null)
			{
				Debug.LogError("Bottom cell component is null in ScrollDown");
				IsScrolling = false;
				return;
			}

			bottomCellComponent.Init(PosDataDict[1]);
			bottomCellComponent.SetData(weaponIndex, AvailableWeapons[weaponIndex].WeaponName, AvailableWeapons[weaponIndex].Description, AvailableWeapons[weaponIndex].WeaponIcon, ListId);
		}

		/// <summary>
		/// Handles the completion of a scroll operation.
		/// </summary>
		/// <param name="eventCell">The cell that completed scrolling</param>
		private void HandleScrollComplete(WeaponNodeSelectorListCell eventCell)
		{
			eventCell.OnScrollComplete -= HandleScrollComplete;
			IsScrolling = false;

			if (scrollQueue.Count == 0) return;

			ScrollDirection direction = scrollQueue.Dequeue();
			if (direction == ScrollDirection.Up) ScrollUp();
			else if (direction == ScrollDirection.Down) ScrollDown();
		}

		/// <summary>
		/// Enables the hover state for the active cell.
		/// </summary>
		public void EnableHoverState()
		{
			if (ActiveCell != null)
			{
				ActiveCell.EnableHover();
			}
		}

		/// <summary>
		/// Disables the hover state for the active cell.
		/// </summary>
		public void DisableHoverState()
		{
			if (ActiveCell != null)
			{
				ActiveCell.DisableHover();
			}
		}
	}
}