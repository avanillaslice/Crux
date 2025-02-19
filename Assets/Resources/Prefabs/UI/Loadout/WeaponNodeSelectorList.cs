using System.Collections.Generic;
using UnityEngine;

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
	// There will always be one that doesnt exist, the first and last position handle the swap
	// This is to prevent an entry without a position AND avoiding the creation of cells
	public readonly Dictionary<int, PosData> PosDataDict = new Dictionary<int, PosData>
	{
		{ 1, new PosData { Position = 1, YPos = 170f,	Scale = 0.7f, Opacity = 0.0f } },	// Next top cell
		{ 2, new PosData { Position = 2, YPos = 95f, 	Scale = 0.8f, Opacity = 0.05f } },	// Above cell
		{ 3, new PosData { Position = 3, YPos = 0f, 	Scale = 1.0f, Opacity = 1.0f } },	// Active cell
		{ 4, new PosData { Position = 4, YPos = -95f,	Scale = 0.8f, Opacity = 0.5f } },	// Below cell
		{ 5, new PosData { Position = 5, YPos = -170f,	Scale = 0.7f, Opacity = 0.25f } },	// Bottom cell
		{ 6, new PosData { Position = 6, YPos = -245f,	Scale = 0.7f, Opacity = 0.0f } },	// Next bottom cell
	};

	// TEMP
	public int ListId;

	// Data
	private bool Initialised = false;
	private Queue<ScrollDirection> ScrollQueue = new Queue<ScrollDirection>();
	public bool IsScrolling = false;
	public int ScrollBuffer = 0;
	[HideInInspector] public ListState State = ListState.Inactive;
	public WeaponNodeSelectorListCell ActiveCell;
	private List<GameObject> Cells = new List<GameObject>();
	public List<WeaponBase> AvailableWeapons = new List<WeaponBase>();

	void Awake()
	{
		InitialiseCells();
	}

	private void InitialiseCells()
	{
		// Create top cells
		for (int i = 1; i < 7; i++)
		{
			GameObject newCell = Instantiate(AssetManager.WeaponNodeSelectorListCellPrefab, gameObject.transform);
			Cells.Add(newCell);
			newCell.GetComponent<WeaponNodeSelectorListCell>().Init(PosDataDict[i]);
		}
	}

	public void Init(WeaponBase initialCellData, List<WeaponBase> availableWeapons, int listId)
	{
		AvailableWeapons = availableWeapons;
		// Debug.Log("AvailableWeapons:" + AvailableWeapons);
		// Debug.Log("InitialCellData" + initialCellData);

		// Find the index of the weapon based on a unique property
		int weaponIndex = FindWeaponIndex(initialCellData);
		if (weaponIndex == -1)
		{
			Debug.LogError("Invalid weaponIndex");
			return;
		}

		ListId = listId;
		SetDataOnRemainingCells(weaponIndex);
		DeactivateList();
		Initialised = true;
	}

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
		Debug.LogError("WeaponIndex not found...");
		return -1; // Return -1 if not found
	}

	private void SetDataOnRemainingCells(int initialWeaponIndex)
	{
		// Debug.Log("AVAILABLE WEAPONS: " + AvailableWeapons.Count + " initialWeaponIndex: " + initialWeaponIndex);
		for (int i = 0; i < 6; i++)
		{
			WeaponNodeSelectorListCell cell = Cells[i].GetComponent<WeaponNodeSelectorListCell>();
			if (i == 2) ActiveCell = cell;
			// Determine correct weaponIndex with wrap-around logic
			int weaponIndex = (initialWeaponIndex + i - 2 + AvailableWeapons.Count) % AvailableWeapons.Count;
			// Debug.Log("Cell: " + i + " weaponIndex: " + weaponIndex);

			WeaponBase relevantWeapon = AvailableWeapons[weaponIndex];
			cell.SetData(weaponIndex, relevantWeapon.WeaponName, relevantWeapon.Description, relevantWeapon.WeaponIcon, ListId);
			// Debug.Log("Cell set to: " + relevantWeapon.WeaponName);
		}
	}

	public void ActivateList()
	{
		// Animation?
		if (!Initialised) return;
		foreach (GameObject cell in Cells) {
			if (cell.GetComponent<WeaponNodeSelectorListCell>().ListPosition == 3) {
				cell.GetComponent<WeaponNodeSelectorListCell>().Animator.Play("ActivateCell");
				continue;
			}
			cell.SetActive(true);
		}
		State = ListState.Active;
	}

	public void DeactivateList()
	{
		foreach (GameObject cell in Cells)
		{
			WeaponNodeSelectorListCell cellComponent = cell.GetComponent<WeaponNodeSelectorListCell>();
			if (cellComponent.ListPosition == 3) {
				if (Initialised) cellComponent.Animator.Play("DeactivateCell");
				continue;
			};
			cell.SetActive(false);
			IsScrolling = false;
			ScrollQueue.Clear();
		}
		State = ListState.Inactive;
	}

	public void ScrollUp()
	{
		if (!Initialised) return;
		if (IsScrolling)
		{
			if (ScrollQueue.Count < 3) ScrollQueue.Enqueue(ScrollDirection.Up);
			return;
		}
		IsScrolling = true;

		// Shift cells up
		GameObject topCell = Cells[0];

		int i = 0;
		foreach (GameObject cell in Cells)
		{
			WeaponNodeSelectorListCell cellComponent = cell.GetComponent<WeaponNodeSelectorListCell>();
			if (cellComponent == topCell.GetComponent<WeaponNodeSelectorListCell>()) continue;
			if ((cellComponent.ListPosition - 1) < 0) continue;
			if (i == 4) cellComponent.OnScrollComplete += HandleScrollComplete;
			// Debug.Log("Shifting Cell: " + cellComponent.ListPosition + " " + cellComponent.Name.text + " to position: " + (cellComponent.ListPosition - 1));
			if (cellComponent.ListPosition - 1 == 3) ActiveCell = cellComponent;
			cellComponent.Scroll(PosDataDict[cellComponent.ListPosition - 1]);
			i++;
		}

		// Fetch weaponIndex from last cell, set incremented value on top cell, then move it to last place
		WeaponNodeSelectorListCell lastCellComponent = Cells[Cells.Count - 1].GetComponent<WeaponNodeSelectorListCell>();
		int weaponIndex = (lastCellComponent.WeaponListIndex + 1) % AvailableWeapons.Count;
		
		Cells.RemoveAt(0);
		Cells.Add(topCell); // Move to last position
		topCell.GetComponent<WeaponNodeSelectorListCell>().SetData(weaponIndex, AvailableWeapons[weaponIndex].WeaponName, AvailableWeapons[weaponIndex].Description, AvailableWeapons[weaponIndex].WeaponIcon, ListId);
		topCell.GetComponent<WeaponNodeSelectorListCell>().Init(PosDataDict[6]); // Use the second last position in PosDataDict
	}

	public void ScrollDown()
	{
		if (!Initialised) return;
		if (IsScrolling)
		{
			if (ScrollQueue.Count < 3) ScrollQueue.Enqueue(ScrollDirection.Down);
			return;
		}
		IsScrolling = true;

		// Shift cells down
		GameObject bottomCell = Cells[Cells.Count - 1];

		int i = 0;
		foreach (GameObject cell in Cells)
		{
			WeaponNodeSelectorListCell cellComponent = cell.GetComponent<WeaponNodeSelectorListCell>();
			if (cellComponent == bottomCell.GetComponent<WeaponNodeSelectorListCell>()) continue;
			if ((cellComponent.ListPosition + 1) > PosDataDict.Count) continue;
			// Debug.Log("Shifting Cell: " + cellComponent.ListPosition + " to position: " + (cellComponent.ListPosition + 1));
			if (i == 4) cellComponent.OnScrollComplete += HandleScrollComplete;
			if (cellComponent.ListPosition + 1 == 3) ActiveCell = cellComponent;
			cellComponent.Scroll(PosDataDict[cellComponent.ListPosition + 1]);
			i++;
		}

		// Fetch WeaponListIndex from current bottom cell, move it to the top position
		WeaponNodeSelectorListCell firstCellComponent = Cells[0].GetComponent<WeaponNodeSelectorListCell>();
		int weaponIndex = (firstCellComponent.WeaponListIndex - 1 + AvailableWeapons.Count) % AvailableWeapons.Count;

		Cells.RemoveAt(Cells.Count - 1);
		Cells.Insert(0, bottomCell); // Move to top position
		bottomCell.GetComponent<WeaponNodeSelectorListCell>().Init(PosDataDict[1]);
		bottomCell.GetComponent<WeaponNodeSelectorListCell>().SetData(weaponIndex, AvailableWeapons[weaponIndex].WeaponName, AvailableWeapons[weaponIndex].Description, AvailableWeapons[weaponIndex].WeaponIcon, ListId);
	}

	private void HandleScrollComplete(WeaponNodeSelectorListCell eventCell)
	{
		eventCell.OnScrollComplete -= HandleScrollComplete;
		IsScrolling = false;
		if (ScrollQueue.Count == 0) return;

		ScrollDirection direction = ScrollQueue.Dequeue();
		if (direction == ScrollDirection.Up) ScrollUp();
		else if (direction == ScrollDirection.Down) ScrollDown();
	}
	
	public void EnableHoverState() {
		ActiveCell.EnableHover();
	}
	
	public void DisableHoverState() {
		ActiveCell.DisableHover();
	}
}