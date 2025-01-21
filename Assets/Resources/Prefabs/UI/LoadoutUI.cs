using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LoadoutUI : UIWindowBase
{
	public static LoadoutUI Inst { get; private set; }

	// Inspector
	public GameObject WeaponUIContainer;
	public GameObject InitialWeaponSlotCursor;

	// Data
	private float WeaponNodeSelectorDistance = 2.5f;
	private bool FirstWeaponNodeIsCentered = true;
	private List<WeaponNode> WeaponNodes = new List<WeaponNode>();
	private List<WeaponNodeSelector> WeaponNodeSelectors = new List<WeaponNodeSelector>();
	private List<List<WeaponNode>> WeaponNodeGroups = new List<List<WeaponNode>>();
	private List<GameObject> ActiveLoadout;

	private WeaponNode WeaponSlotCursor;

	public Dictionary<SlotType, List<WeaponBase>> AvailableWeapons { get; private set; }

	void Awake()
	{
		Debug.Log("LOADOUT UI AWAKE");
		if (Inst != null && Inst != this)
		{
			Debug.Log("Loadout already exists");
			Destroy(gameObject);
			return;  // Ensure no further code execution in this instance
		}
		Inst = this;
		InitializeAvailableWeapons();
	}

	private void InitializeAvailableWeapons()
	{
		AvailableWeapons = new Dictionary<SlotType, List<WeaponBase>>
		{
			{ SlotType.Single, new List<WeaponBase>() },
			{ SlotType.Dual, new List<WeaponBase>() },
			{ SlotType.System, new List<WeaponBase>() }
		};
	}

	void OnEnable()
	{
		PlayerManager.Inst.ActivePlayerShip.transform.localScale += new Vector3(1f, 1f, 1f);
		UpdateAvailableWeapons();
		InitialiseLoadoutUI();
		// SetInitialCursor();
	}

	void OnDisable()
	{
		PlayerManager.Inst.ActivePlayerShip.transform.localScale -= new Vector3(1f, 1f, 1f);
		foreach (Transform child in WeaponUIContainer.transform)
		{
			Destroy(child.gameObject);
		}
		WeaponNodes = new List<WeaponNode>();
		WeaponNodeSelectors = new List<WeaponNodeSelector>();
		WeaponNodeGroups = new List<List<WeaponNode>>();
	}

	private void InitialiseLoadoutUI()
	{
		InstantiateWeaponNodes();
		if (WeaponNodeGroups.Count == 0)
		{
			Debug.LogError("NO NODES FOUND");
			return;
		}
		InitialiseWeaponNodeSelectors();
	}

	// Fetch AttachPoints, Sort by YPOS, Sort into Left/Right/Middle
	private void InstantiateWeaponNodes()
	{
		// Find all AttachPoints from PlayerShip
		List<WeaponSlot> weaponSlots = PlayerManager.Inst.ActivePlayerShip.GetActiveWeaponSlots();

		// Sort attachPoints by YPOS (what kind of list do I use for static order?)
		foreach (WeaponSlot weaponSlot in weaponSlots)
		{
			List<WeaponNode> relatedWeaponNodes = new List<WeaponNode>();

			// Instantiate and group WeaponNodes
			foreach (AttachPoint attachPoint in weaponSlot.AttachPoints)
			{
				// FetchGameObject for transform position
				Vector3 posAbovePlayer = new Vector3(attachPoint.transform.position.x, attachPoint.transform.position.y, WeaponUIContainer.transform.position.z); // One unit above player
				WeaponNode weaponNode = Instantiate(AssetManager.WeaponNodePrefab, posAbovePlayer, Quaternion.identity, WeaponUIContainer.transform).GetComponent<WeaponNode>();
				weaponNode.Init(attachPoint, weaponSlot);

				// Add to group
				relatedWeaponNodes.Add(weaponNode);
				WeaponNodes.Add(weaponNode);
			}

			// Assign related nodes to each group
			foreach (WeaponNode weaponNode in relatedWeaponNodes)
			{
				weaponNode.SetRelatedNodes(relatedWeaponNodes);
			}
			WeaponNodeGroups.Add(relatedWeaponNodes);
		}
		// Sort WeaponNodes by YPos
		// Does this mean I dont need to for WeaponNodeGroups?
		// WeaponNodes.Sort((a, b) => b.YPos.CompareTo(a.YPos));
		WeaponNodeGroups.Sort((listA, listB) => listB[0].YPos.CompareTo(listA[0].YPos));
		Debug.Log("WeaponSlotNodes Instantated:");
		foreach (List<WeaponNode> weaponNodeGroup in WeaponNodeGroups) Debug.Log("YPOSITION: " + weaponNodeGroup[0].YPos);
	}

	private void InitialiseWeaponNodeSelectors()
	{
		List<Vector3> WeaponNodeSelectorPositions = DetermineWeaponNodeSelectorPositions();
		InstantiateWeaponNodeSelectors(WeaponNodeSelectorPositions);
		LinkNodesToSelectors();
	}

	private List<Vector3> DetermineWeaponNodeSelectorPositions()
	{
		List<Vector3> weaponNodeSelectorPositions = new List<Vector3>();

		// Angle between each WeaponNodeSelector
		float anglePerSelector = 360 / WeaponNodes.Count;

		// Determine first WeaponNodeSelector position
		Vector3 FirstWeaponNodeSelectorPosition;
		if (WeaponNodes[0].XPos == 0)
		{
			FirstWeaponNodeIsCentered = true;
			FirstWeaponNodeSelectorPosition = CalculateLocalPosition(0);
		}
		else
		{
			FirstWeaponNodeIsCentered = false;
			FirstWeaponNodeSelectorPosition = CalculateLocalPosition(-(anglePerSelector / 2));
		}

		// Calculate each position and add to list
		int i = 0;
		while (i < WeaponNodes.Count)
		{
			if (i == 0) weaponNodeSelectorPositions.Add(FirstWeaponNodeSelectorPosition);
			else
			{
				float originalWeaponNodeAngle = FirstWeaponNodeIsCentered ? 0 : -(anglePerSelector / 2);
				Vector3 newPosition = CalculateLocalPosition(originalWeaponNodeAngle + anglePerSelector * i);
				weaponNodeSelectorPositions.Add(newPosition);
			}
			i++;
		}

		return weaponNodeSelectorPositions;
	}

	private void InstantiateWeaponNodeSelectors(List<Vector3> WeaponNodeSelectorPositions)
	{
		if (WeaponNodeSelectorPositions.Count == 0)
		{
			Debug.LogError("No WeaponNodeSelectorPositions have been set");
			// This should throw error to cease other functions
			return;
		}

		foreach (Vector3 WeaponNodeSelectorPosition in WeaponNodeSelectorPositions)
		{
			GameObject WeaponNodeSelector = Instantiate(AssetManager.WeaponNodeSelectorPrefab, WeaponNodeSelectorPosition, Quaternion.identity, WeaponUIContainer.transform);

			WeaponNodeSelectors.Add(WeaponNodeSelector.GetComponent<WeaponNodeSelector>());
		}
	}

	private void LinkNodesToSelectors()
	{
		// WeaponNodeGroups and WeaponNodes have been sorted by YPos
		int nodeCounter = 0;
		int nodeGroupCounter = 0;
		WeaponNode RearAsymmetricalWeaponNode;
		Debug.Log("Detected " + WeaponNodes.Count + " WeaponNodes");
		Debug.Log("Detected " + WeaponNodeSelectors.Count + " WeaponNodeSelectors");
		while (nodeCounter < WeaponNodes.Count)
		{
			Debug.Log("Placing Group: " + nodeGroupCounter);
			foreach (WeaponNode weaponNode in WeaponNodeGroups[nodeGroupCounter])
			{
				Debug.Log("Placing Node: " + nodeCounter + " X: " + weaponNode.XPos + " Y: " + weaponNode.YPos + " Side: " + weaponNode.Side);

				switch (weaponNode.Side)
				{
					case RelativeSide.Right:
						{
							Debug.Log("ON THE RIGHT");
							Debug.Log("Fetching WeaponNodeSelector: " + nodeGroupCounter);
							weaponNode.AssignSelector(WeaponNodeSelectors[nodeGroupCounter]);
							break;
						}
					case RelativeSide.Left:
						{
							Debug.Log("ON THE LEFT");
							Debug.Log("Fetching WeaponNodeSelector: " + (WeaponNodeGroups.Count - nodeGroupCounter));
							weaponNode.AssignSelector(WeaponNodeSelectors[WeaponNodeGroups.Count - nodeGroupCounter]);
							break;
						}
					case RelativeSide.Center:
						{
							Debug.Log("CENTERED");
							if (nodeGroupCounter == 0)
							{
								Debug.Log("FIRSTWEAPONNODE IS CENTERED");
								WeaponNodes[0].AssignSelector(WeaponNodeSelectors[0]);
							}
							else
							{
								Debug.Log("SETTING RearAsymmetricalWeaponNode FOR LATER");
								RearAsymmetricalWeaponNode = weaponNode; // Replace this with AddOrFetchRearAsymmetricalWeaponNode() logic
							}
							break;
						}
					default:
						{
							Debug.LogWarning("Unexpected Side value: " + weaponNode.Side);
							break;
						}
				}
				nodeCounter++;
			}
			nodeGroupCounter++;
		}
	}

	private void UpdateAvailableWeapons()
	{
		// Update the dictionary with the new lists
		AvailableWeapons[SlotType.Single] = LoadoutManager.GetInventoryByType(SlotType.Single);
		AvailableWeapons[SlotType.Dual] = LoadoutManager.GetInventoryByType(SlotType.Dual);
		AvailableWeapons[SlotType.System] = LoadoutManager.GetInventoryByType(SlotType.System);
	}

	Vector3 CalculateLocalPosition(float angleDegrees)
	{
		// Convert angle to radians
		float angleRadians = (angleDegrees + 90) * Mathf.Deg2Rad;

		// Calculate offsets
		float offsetX = Mathf.Cos(angleRadians) * WeaponNodeSelectorDistance;
		float offsetY = Mathf.Sin(angleRadians) * WeaponNodeSelectorDistance;

		// Create the new position vector
		Vector3 offset = new Vector3(offsetX, offsetY, 0);

		// Transform the offset to local coordinates
		Vector3 localPosition = PlayerManager.Inst.ActivePlayerShip.transform.localPosition + offset;
		localPosition.z = WeaponUIContainer.transform.position.z;

		return localPosition;
	}

	private void SetInitialCursor()
	{
		if (InitialWeaponSlotCursor != null) SetCursor(InitialWeaponSlotCursor.GetComponent<WeaponNode>());
		else SetCursor(WeaponNodes[0]);
	}

	private void SetCursor(WeaponNode weaponNode)
	{
		WeaponSlotCursor?.DisableHoverState();
		WeaponSlotCursor = weaponNode;
		WeaponSlotCursor.EnableHoverState();
	}

	public override void HandleSelect()
	{
		WeaponSlotCursor?.HandleSelect();
	}

	public override void HandleBack()
	{
		if (WeaponSlotCursor.IsSelected) WeaponSlotCursor.HandleDeselect();
		else HandleExit();
	}

	public override void HandleExit()
	{
		// Animation stuff goes here

		// This should probably use UIManager.Inst.HandleExit()
		UIManager.Inst.DisableLoadoutUI();
		UIManager.Inst.EnableInterStageUI();
	}

	public override void HandleMoveLeft()
	{
		if (!WeaponSlotCursor.IsSelected)
		{
			WeaponNode leftWeaponNode = DetermineAppropriateHoizontalNode(true);
			if (leftWeaponNode == null) return;
			SetCursor(leftWeaponNode);
		}
	}

	public override void HandleMoveRight()
	{
		if (!WeaponSlotCursor.IsSelected)
		{
			WeaponNode rightWeaponNode = DetermineAppropriateHoizontalNode(false);
			if (rightWeaponNode == null) return;
			SetCursor(rightWeaponNode);
		}
	}

	public override void HandleMoveUp()
	{
		if (!WeaponSlotCursor.IsSelected)
		{
			WeaponNode aboveWeaponNode = DetermineAppropriateVerticalNode(true);
			if (aboveWeaponNode == null) return;
			SetCursor(aboveWeaponNode);
		}
		else
		{
			WeaponSlotCursor.WeaponNodeSelector.ScrollUp();
		}
	}

	public override void HandleMoveDown()
	{
		if (!WeaponSlotCursor.IsSelected)
		{
			WeaponNode belowWeaponNode = DetermineAppropriateVerticalNode(false);
			if (belowWeaponNode == null) return;
			SetCursor(belowWeaponNode);
		}
		else
		{
			WeaponSlotCursor.WeaponNodeSelector.ScrollDown();
		}
	}

	private WeaponNode DetermineAppropriateHoizontalNode(bool up)
	{
		return new WeaponNode();
	}

	private WeaponNode DetermineAppropriateVerticalNode(bool left)
	{
		return new WeaponNode();
	}
}