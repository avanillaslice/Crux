using System.Collections.Generic;
using UnityEngine;

public class WeaponNodeSelectorList : MonoBehaviour
{
	// Types
	public enum ListState
	{
		Inactive,
		Active,
	}
	public class PosData {
		public int Position;
        public float Scale;
        public float Opacity;
	}
	// There will always be one that doesnt exist, the first and last position handle the swap
	// This is to prevent an entry without a position AND avoiding the creation of cells
	public readonly Dictionary<int, PosData> PosDataDict = new Dictionary<int, PosData>
	{
		{ 0, new PosData { Position = 0, Scale = 145f, Opacity = 0.0f } },	// To swap
		{ 1, new PosData { Position = 1, Scale = 145f, Opacity = 0.0f } },	// Next top cell
		{ 2, new PosData { Position = 2, Scale = 160f, Opacity = 0.5f } },	// Above cell
		{ 3, new PosData { Position = 3, Scale = 200f, Opacity = 1.0f } },	// Active cell
		{ 4, new PosData { Position = 4, Scale = 160f, Opacity = 0.5f } },	// Below cell
		{ 5, new PosData { Position = 5, Scale = 145f, Opacity = 0.25f } },	// Bottom cell
		{ 6, new PosData { Position = 6, Scale = 145f, Opacity = 0.0f } },	// Next bottom cell
		{ 7, new PosData { Position = 7, Scale = 145f, Opacity = 0.0f } }	// To swap, initially empty
	};

	// Inspector
	public GameObject InitialCell;

	// Data
	[HideInInspector] public ListState State = ListState.Inactive;
	private WeaponNodeSelectorListCell ActiveCell;
	private List<GameObject> Cells = new List<GameObject>();
	private List<WeaponBase> AvailableWeapons = new List<WeaponBase>();

	void Awake()
	{
		ActiveCell = InitialCell.GetComponent<WeaponNodeSelectorListCell>();
		ActiveCell.Init(PosDataDict[3]);
		InitialiseRemainingCells();
	}

	private void InitialiseRemainingCells()
	{
		// Create top cells
		for (int i = 1; i < 3; i++) {
			GameObject topCell = Instantiate(AssetManager.WeaponNodeSelectorListCellPrefab, gameObject.transform);
    		topCell.transform.SetSiblingIndex(ActiveCell.transform.GetSiblingIndex());
    		Cells.Insert(i - 1, topCell); // Inserts at the beginning of the list to maintain order
			topCell.GetComponent<WeaponNodeSelectorListCell>().Init(PosDataDict[i]);
			Cells.Add(topCell);
		}

		Cells.Add(InitialCell);

		// Create bottom cells
    	for (int i = 4; i < PosDataDict.Count - 1; i++)
		{
			GameObject bottomCell = Instantiate(AssetManager.WeaponNodeSelectorListCellPrefab, gameObject.transform);
			bottomCell.GetComponent<WeaponNodeSelectorListCell>().Init(PosDataDict[i]);
			Cells.Add(bottomCell);
		}
	}

	public void Init(WeaponBase initialCellData, List<WeaponBase> availableWeapons)
	{
	    AvailableWeapons = availableWeapons;
	    // Debug.Log("AvailableWeapons:" + AvailableWeapons);
	    // Debug.Log("InitialCellData" + initialCellData);

	    // Find the index of the weapon based on a unique property
	    int weaponIndex = FindWeaponIndex(initialCellData);
	    if (weaponIndex == -1) {
	        Debug.LogError("Invalid weaponIndex");
	        return;
	    }

	    // ActiveCell.SetData(weaponIndex, initialCellData.WeaponName, initialCellData.Description, initialCellData.WeaponIcon);
	    SetDataOnRemainingCells(weaponIndex);
	    DeactivateList();
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
	    return -1; // Return -1 if not found
	}

	private void SetDataOnRemainingCells(int initialWeaponIndex)
	{
		// Debug.Log("AVAILABLE WEAPONS: " + AvailableWeapons.Count + " initialWeaponIndex: " + initialWeaponIndex);
	    for (int i = 0; i < 7; i++)
	    {
	        WeaponNodeSelectorListCell cell = Cells[i].GetComponent<WeaponNodeSelectorListCell>();

	        // Determine correct weaponIndex with wrap-around logic
	        int weaponIndex = (initialWeaponIndex + i - 2 + AvailableWeapons.Count) % AvailableWeapons.Count;
			// Debug.Log("Cell: " + i + " weaponIndex: " + weaponIndex);

	        WeaponBase relevantWeapon = AvailableWeapons[weaponIndex];
	        cell.SetData(weaponIndex, relevantWeapon.WeaponName, relevantWeapon.Description, relevantWeapon.WeaponIcon);
			// Debug.Log("Cell set to: " + relevantWeapon.WeaponName);
	    }
	}

	public void ActivateList()
	{
		// Animation?
		foreach (GameObject cell in Cells) cell.SetActive(true);
		State = ListState.Active;
	}

	public void DeactivateList()
	{
		foreach (GameObject cell in Cells) {
			if (cell.GetComponent<WeaponNodeSelectorListCell>() == ActiveCell) continue;
			cell.SetActive(false);
		}
		State = ListState.Inactive;
	}

	public void ScrollUp()
	{
	    // Shift cells up
	    foreach (GameObject cell in Cells)
	    {
	        WeaponNodeSelectorListCell cellComponent = cell.GetComponent<WeaponNodeSelectorListCell>();
	        Debug.Log("Shifting Cell: " + cellComponent.ListPosition + " " + cellComponent.Name.text + " to position: " + (cellComponent.ListPosition - 1));
	        cellComponent.Scroll(PosDataDict[cellComponent.ListPosition - 1]);
	    }

		// Fetch WeaponListIndex from current top cell, move it to second last position
	    WeaponNodeSelectorListCell thirdLastCellComponent = Cells[Cells.Count - 3].GetComponent<WeaponNodeSelectorListCell>();
	    int weaponIndex = (thirdLastCellComponent.WeaponListIndex + 1) % AvailableWeapons.Count;
	    GameObject topCell = Cells[0];
	    Cells.RemoveAt(0);
	    Cells.Insert(Cells.Count - 1, topCell); // Move to second last position
	    topCell.transform.SetSiblingIndex(Cells.Count - 2); // Physically move to second last position
	    topCell.GetComponent<WeaponNodeSelectorListCell>().SetData(weaponIndex, AvailableWeapons[weaponIndex].WeaponName, AvailableWeapons[weaponIndex].Description, AvailableWeapons[weaponIndex].WeaponIcon);
	    topCell.GetComponent<WeaponNodeSelectorListCell>().Init(PosDataDict[6]); // Use the second last position in PosDataDict
	}

	public void ScrollDown()
	{
		// Shift cells down
		foreach (GameObject cell in Cells)
		{
			WeaponNodeSelectorListCell cellComponent = cell.GetComponent<WeaponNodeSelectorListCell>();
			Debug.Log("Shifting Cell: " + cellComponent.ListPosition + " to position: " + (cellComponent.ListPosition + 1));
			cellComponent.Scroll(PosDataDict[cellComponent.ListPosition + 1]);
		}
	
		// Fetch WeaponListIndex from current bottom cell, move it to the top position
		WeaponNodeSelectorListCell secondCellComponent = Cells[1].GetComponent<WeaponNodeSelectorListCell>();
		int weaponIndex = (secondCellComponent.WeaponListIndex - 1 + AvailableWeapons.Count) % AvailableWeapons.Count;
		GameObject bottomCell = Cells[Cells.Count - 1];
		Cells.RemoveAt(Cells.Count - 1);
		Cells.Insert(0, bottomCell); // Move to top position
		bottomCell.transform.SetSiblingIndex(0); // Physically move to top position
		bottomCell.GetComponent<WeaponNodeSelectorListCell>().SetData(weaponIndex, AvailableWeapons[weaponIndex].WeaponName, AvailableWeapons[weaponIndex].Description, AvailableWeapons[weaponIndex].WeaponIcon);
		bottomCell.GetComponent<WeaponNodeSelectorListCell>().Init(PosDataDict[0]); // Use the top position in PosDataDict
	}
}