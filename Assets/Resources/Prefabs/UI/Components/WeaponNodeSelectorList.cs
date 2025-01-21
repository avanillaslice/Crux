using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class WeaponNodeSelectorList : MonoBehaviour
{
	// Config
	private readonly int CellLimit = 3; // Uses count to determine call quantity + opacity/scale shift per scroll 

	// Inspector
	public GameObject CellContainer;
	public GameObject InitialCell;

	// Data
	private List<WeaponBase> AvailableWeapons = new List<WeaponBase>();
	private WeaponNodeSelectorListCell ActiveCell;
	private List<GameObject> Cells = new List<GameObject>();
	private float CellHeight;

	void Awake()
	{
		ActiveCell = InitialCell.GetComponent<WeaponNodeSelectorListCell>();
		RectTransform activeCellRect = ActiveCell.GetComponent<RectTransform>();
		CellHeight = activeCellRect.rect.height;
		InitialiseRemainingCells();
	}

	private void InitialiseRemainingCells()
	{
		// Create top cell
		Vector3 relativePosition = ActiveCell.transform.position + new Vector3(0, CellHeight, 0);
		GameObject topCell = Instantiate(AssetManager.WeaponNodeSelectorListCellPrefab, relativePosition, Quaternion.identity, gameObject.transform);
		Cells.Add(topCell);
		Cells.Add(InitialCell);

		// Create bottom cells
		int i = 0;
		while (i < CellLimit - 2)
		{
			relativePosition = ActiveCell.transform.position + new Vector3(0, -CellHeight * (i + 1), 0);
			GameObject bottomCell = Instantiate(AssetManager.WeaponNodeSelectorListCellPrefab, relativePosition, Quaternion.identity, gameObject.transform);
			Cells.Add(bottomCell);
		}
	}

	public void Init(WeaponBase initialCellData, List<WeaponBase> availableWeapons)
	{
		AvailableWeapons = availableWeapons;
		ActiveCell.SetData(1, initialCellData.WeaponName, initialCellData.Description, initialCellData.WeaponIcon);
		SetDataOnRemainingCells();
	}

	private void SetDataOnRemainingCells()
	{
		int i = 0;
		while (i < CellLimit)
		{
			if (i == 1) continue; // Skip if position of ActiveCell, already setup

			WeaponNodeSelectorListCell cell = Cells[i].GetComponent<WeaponNodeSelectorListCell>();

			int weaponIndex = AvailableWeapons.Count < i ? i - AvailableWeapons.Count : i;
			WeaponBase RelevantWeapon = AvailableWeapons[weaponIndex];
			cell.SetData(weaponIndex, RelevantWeapon.WeaponName, RelevantWeapon.Description, RelevantWeapon.WeaponIcon);
		}
	}

	public void ActivateList(List<WeaponBase> availableWeapons)
	{
		// Animation?
		AvailableWeapons = availableWeapons; // When to sort? Should currently active be put at the top of list?
		InitCells();
	}

	public void DeactivateList()
	{

	}

	private void InitCells()
	{


	}

	private void ScrollUp()
	{

	}

	private void ScrollDown()
	{

	}
}