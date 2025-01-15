using System.Collections.Generic;
using UnityEngine;

public class Loadout : UIWindowBase
{
	// Inspector
	public GameObject WeaponSlotUIContainer;
	public GameObject InitialWeaponSlotCursor;

	// Data
	private float WeaponSlotSelectorDistance = 4f;
	private bool FirstWeaponSlotNodeIsCentered = true;
	private List<WeaponSlotNode> WeaponSlotNodes;
	private List<WeaponSlotSelector> WeaponSlotSelectors;
	private List<List<WeaponSlotNode>> WeaponSlotNodeGroups;

	private WeaponSlotNode WeaponSlotCursor;

	public List<WeaponBase> LightWeapons;
	public List<WeaponBase> MediumWeapons;
	public List<WeaponBase> HeavyWeapons;

	void Awake()
	{
		InitialiseLoadoutUI();
	}

	void Enable()
	{
		UpdateAvailableWeapons();
		SetInitialCursor();
	}

	private void InitialiseLoadoutUI()
	{
		InstantiateWeaponSlotNodes();
		if (WeaponSlotNodeGroups.Count == 0)
		{
			Debug.LogError("NO NODES FOUND");
			return;
		}
		InitialiseWeaponSlotSelectors();
		UpdateAvailableWeapons();
	}

	// Fetch AttachPoints, Sort by YPOS, Sort into Left/Right/Middle
	private void InstantiateWeaponSlotNodes()
	{
		// Find all AttachPoints from PlayerShip
		List<WeaponSlot> weaponSlots = PlayerManager.Inst.ActivePlayerShip.WeaponSlots;

		// Sort attachPoints by YPOS (what kind of list do I use for static order?)
		foreach (WeaponSlot weaponSlot in weaponSlots)
		{
			List<WeaponSlotNode> relatedWeaponSlotNodes = new List<WeaponSlotNode>();

			// Instantiate and group WeaponSlotNodes
			foreach (AttachPoint attachPoint in weaponSlot.AttachPoints)
			{
				// FetchGameObject for transform position
				WeaponSlotNode weaponSlotNode = Instantiate(AssetManager.WeaponSlotNode, attachPoint.transform.position, Quaternion.identity);
				weaponSlotNode.Init(attachPoint, weaponSlot);

				// Add to group
				relatedWeaponSlotNodes.Add(weaponSlotNode);
				WeaponSlotNodes.Add(weaponSlotNode);

				// Not sure if I need this now that they are partnered up
				// if (weaponSlotNode.transform.position.x > 0) LeftWeaponSlotNodes.Add(weaponSlotNode);
				// else if (weaponSlotNode.transform.position.x < 0) RightWeaponSlotNodes.Add(weaponSlotNode);
				// else CentralWeaponSlotNodes.Add(weaponSlotNode); // Maybe? Just felt weird it not being in a list
			}

			// Assign related nodes to each group
			foreach (WeaponSlotNode weaponSlotNode in relatedWeaponSlotNodes)
			{
				weaponSlotNode.SetRelatedNodes(relatedWeaponSlotNodes);
				WeaponSlotNodeGroups.Add(relatedWeaponSlotNodes);
			}
		}

		// Sort WeaponSlotNodes by YPos
		// Does this mean I dont need to for WeaponSlotNodeGroups?
		WeaponSlotNodes.Sort((x, y) => x.YPos.CompareTo(y.YPos));
		WeaponSlotNodeGroups.Sort((listA, listB) => listA[0].YPos.CompareTo(listB[0].YPos));
	}

	private void InitialiseWeaponSlotSelectors()
	{
		List<Vector3> WeaponSlotSelectorPositions = DetermineWeaponSlotSelectorPositions();
		InstantiateWeaponSlotSelectors(WeaponSlotSelectorPositions);
		LinkNodesToSelectors();
	}

	private List<Vector3> DetermineWeaponSlotSelectorPositions()
	{
		List<Vector3> WeaponSlotSelectorPositions = new List<Vector3>();

		// Angle between each WeaponSlotSelector
		float anglePerSelector = 360 / WeaponSlotNodes.Count;

		// Determine first WeaponSlotSelector position
		Vector3 FirstWeaponSlotSelectorPosition;
		if (WeaponSlotNodes[0].XPos == 0)
		{
			FirstWeaponSlotNodeIsCentered = true;
			FirstWeaponSlotSelectorPosition = CalculateLocalPosition(0);
		}
		else
		{
			FirstWeaponSlotNodeIsCentered = false;
			FirstWeaponSlotSelectorPosition = CalculateLocalPosition(anglePerSelector / 2);
		}

		// Calculate each position and add to list
		int i = 0;
		while (i < WeaponSlotNodes.Count)
		{
			if (i == 0) WeaponSlotSelectorPositions.Add(FirstWeaponSlotSelectorPosition);
			else
			{
				Vector3 newPosition = CalculateLocalPosition(anglePerSelector * i);
				WeaponSlotSelectorPositions.Add(newPosition);
			}
			i++;
		}

		return WeaponSlotSelectorPositions;
	}

	private void InstantiateWeaponSlotSelectors(List<Vector3> weaponSlotSelectorPositions)
	{
		if (weaponSlotSelectorPositions.Count == 0)
		{
			Debug.LogError("No weaponSlotSelectorPositions have been set");
			// This should throw error to cease other functions
			return;
		}

		foreach (Vector3 weaponslotSelectorPosition in weaponSlotSelectorPositions)
		{
			GameObject weaponSlotSelector = Instantiate(AssetManager.WeaponSlotSelector, weaponslotSelectorPosition, Quaternion.identity);
			WeaponSlotSelectors.Add(weaponSlotSelector.GetComponent<WeaponSlotSelector>());
		}
	}

	private void LinkNodesToSelectors()
	{
		// WeaponSlotNodeGroups and WeaponSlotNodes have been sorted by YPos
		int nodeCounter = 0;
		int nodeGroupCounter = 0;
		WeaponSlotNode RearAsymmetricalWeaponSlotNode;
		while (nodeCounter < WeaponSlotNodes.Count)
		{
			// Handle first WeaponSlotNodeGroup
			if (nodeCounter == 0 && FirstWeaponSlotNodeIsCentered)
			{
				WeaponSlotNodes[0].AssignSelector(WeaponSlotSelectors[0]);
				nodeCounter++;
			}
			else
			{
				foreach (WeaponSlotNode weaponSlotNode in WeaponSlotNodeGroups[nodeGroupCounter])
				{
					if (weaponSlotNode.XPos > 0)
					{ // On the right
						weaponSlotNode.AssignSelector(WeaponSlotSelectors[nodeCounter]);
					}
					else if (weaponSlotNode.XPos < 0)
					{ // On the left
						weaponSlotNode.AssignSelector(WeaponSlotSelectors[WeaponSlotNodes.Count - nodeCounter]);
					}
					else
					{ // Centered
						// Needs to save it until last, OR divide the total count by two to get the lowest selector...
						// Log error if already set, can only have one of these...
						RearAsymmetricalWeaponSlotNode = weaponSlotNode;
					}
					nodeCounter++;
				}
			}
			nodeGroupCounter++;
		}
	}

	Vector3 CalculateLocalPosition(float angleDegrees)
	{
		// Convert angle to radians
		float angleRadians = angleDegrees * Mathf.Deg2Rad;

		// Calculate offsets
		float offsetX = Mathf.Cos(angleRadians) * WeaponSlotSelectorDistance;
		float offsetY = Mathf.Sin(angleRadians) * WeaponSlotSelectorDistance;

		// Create the new position vector
		Vector3 offset = new Vector3(offsetX, offsetY, 0);

		// Transform the offset to local coordinates
		Vector3 localPosition = PlayerManager.Inst.ActivePlayerShip.transform.localPosition + offset;

		return localPosition;
	}

	private void SetInitialCursor()
	{
		WeaponSlotNode weaponSlotNode = InitialWeaponSlotCursor.GetComponent<WeaponSlotNode>();
		SetCursor(weaponSlotNode);
	}

	private void SetCursor(WeaponSlotNode weaponSlotNode)
	{
		WeaponSlotCursor?.DisableHoverState();
		WeaponSlotCursor = weaponSlotNode;
		WeaponSlotCursor.EnableHoverState();
	}

	//   private void UpdateAvailableWeapons() {
	//     LightWeapons = LoadoutManager.FetchWeapons(WeaponSlotType.Light);
	//     MediumWeapons = LoadoutManager.FetchWeapons(WeaponSlotType.Medium);
	//     HeavyWeapons = LoadoutManager.FetchWeapons(WeaponSlotType.Heavy);
	//   }

	public override void HandleSelect()
	{
		WeaponSlotCursor?.HandleSelect();
	}

	public override void HandleBack()
	{
		if (WeaponSlotCursor.IsSelected) WeaponSlotCursor.HandleDeselect();
		// else Trigger termination animations and return to InterScene
	}

	public override void HandleBackClicked() { }

	public override void HandleMoveLeft()
	{
		if (!WeaponSlotCursor.IsSelected)
		{
			WeaponSlotNode leftWeaponNode = DetermineAppropriateHoizontalNode(true);
			if (leftWeaponNode == null) return;
			SetCursor(leftWeaponNode);
		}
	}

	public override void HandleMoveRight()
	{
		if (!WeaponSlotCursor.IsSelected)
		{
			WeaponSlotNode rightWeaponNode = DetermineAppropriateHoizontalNode(false);
			if (rightWeaponNode == null) return;
			SetCursor(rightWeaponNode);
		}
	}

	public override void HandleMoveUp()
	{
		if (!WeaponSlotCursor.IsSelected)
		{
			WeaponSlotNode aboveWeaponNode = DetermineAppropriateVerticalNode(true);
			if (aboveWeaponNode == null) return;
			SetCursor(aboveWeaponNode);
		}
		else
		{
			WeaponSlotCursor.WeaponSlotSelector.HandleScrollUp();
		}
	}

	public override void HandleMoveDown()
	{
		if (!WeaponSlotCursor.IsSelected)
		{
			WeaponSlotNode belowWeaponNode = DetermineAppropriateVerticalNode(false);
			if (belowWeaponNode == null) return;
			SetCursor(belowWeaponNode);
		}
		else
		{
			WeaponSlotCursor.WeaponSlotSelector.HandleScrollDown();
		}
	}
}