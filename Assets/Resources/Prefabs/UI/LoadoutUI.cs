using System.Collections.Generic;
using UnityEngine;

public class LoadoutUI : UIWindowBase
{
	public static LoadoutUI Inst { get; private set; }

	// Inspector
	public GameObject WeaponUIContainer;

	// Data
	private float WeaponNodeSelectorDistance = 3f;
	private bool FirstWeaponNodeIsCentered = true;
	private List<WeaponNode> WeaponNodes = new List<WeaponNode>();
	private List<WeaponNodeSelector> WeaponNodeSelectors = new List<WeaponNodeSelector>();
	private List<List<WeaponNode>> WeaponNodeGroups = new List<List<WeaponNode>>();
	private List<GameObject> ActiveLoadout;

	public WeaponNode WeaponNodeCursor;

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
		SetCursor(WeaponNodes[0]);
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
		InstantiateWeaponNodeSelectors();
		LinkNodesToSelectors();
	}

	// Fetch AttachPoints, Sort by YPOS, Sort into Left/Right/Middle
	private void InstantiateWeaponNodes()
	{
		// Find all AttachPoints from PlayerShip
		List<WeaponSlot> weaponSlots = PlayerManager.Inst.ActivePlayerShip.GetActiveWeaponSlots();

		// Sort attachPoints by YPOS (what kind of list do I use for static order?)
		int i = 0;
		foreach (WeaponSlot weaponSlot in weaponSlots)
		{
			List<WeaponNode> relatedWeaponNodes = new List<WeaponNode>();

			// Instantiate and group WeaponNodes
			foreach (AttachPoint attachPoint in weaponSlot.AttachPoints)
			{
				// FetchGameObject for transform position
				Vector3 posAbovePlayer = new Vector3(attachPoint.transform.position.x, attachPoint.transform.position.y, WeaponUIContainer.transform.position.z); // One unit above player
				WeaponNode weaponNode = Instantiate(AssetManager.WeaponNodePrefab, posAbovePlayer, Quaternion.identity, WeaponUIContainer.transform).GetComponent<WeaponNode>();
				weaponNode.Init(attachPoint, weaponSlot, i);

				// Add to group
				relatedWeaponNodes.Add(weaponNode);
				WeaponNodes.Add(weaponNode);
				i++;
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
		// foreach (List<WeaponNode> weaponNodeGroup in WeaponNodeGroups) Debug.Log("YPOSITION: " + weaponNodeGroup[0].YPos);
		Debug.Log("WeaponNodes Instantated!");
	}

	private void InstantiateWeaponNodeSelectors()
	{
		List<Vector3> weaponNodeSelectorPositions = DetermineWeaponNodeSelectorPositions();
		if (weaponNodeSelectorPositions.Count == 0)
		{
			Debug.LogError("No WeaponNodeSelectorPositions have been set");
			// This should throw error to cease other functions
			return;
		}

		foreach (Vector3 weaponNodeSelectorPosition in weaponNodeSelectorPositions)
		{
			GameObject WeaponNodeSelector = Instantiate(AssetManager.WeaponNodeSelectorPrefab, weaponNodeSelectorPosition, Quaternion.identity, WeaponUIContainer.transform);

			WeaponNodeSelectors.Add(WeaponNodeSelector.GetComponent<WeaponNodeSelector>());
		}
		// Debug.Log("WeaponNodeSelectors Instantiated!");
	}

	private void LinkNodesToSelectors()
	{
		// WeaponNodeGroups and WeaponNodes have been sorted by YPos
		int nodeCounter = 0;
		int nodeGroupCounter = 0;
		WeaponNode RearAsymmetricalWeaponNode;
		// Debug.Log("Detected " + WeaponNodes.Count + " WeaponNodes");
		// Debug.Log("Detected " + WeaponNodeSelectors.Count + " WeaponNodeSelectors");
		while (nodeCounter < WeaponNodes.Count)
		{
			foreach (WeaponNode weaponNode in WeaponNodeGroups[nodeGroupCounter])
			{
				// Debug.Log("Placing Node: " + nodeCounter + " X: " + weaponNode.XPos + " Y: " + weaponNode.YPos + " Side: " + weaponNode.Side);

				switch (weaponNode.Side)
				{
					case RelativeSide.Right:
						{
							weaponNode.AssignSelector(WeaponNodeSelectors[nodeGroupCounter]);
							break;
						}
					case RelativeSide.Left:
						{
							weaponNode.AssignSelector(WeaponNodeSelectors[WeaponNodes.Count - nodeGroupCounter]);
							break;
						}
					case RelativeSide.Center:
						{
							if (nodeGroupCounter == 0)
							{
								WeaponNodes[0].AssignSelector(WeaponNodeSelectors[0]);
							}
							else
							{
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
		// Debug.Log("Nodes and Selectors Linked!");
	}

	private void UpdateAvailableWeapons()
	{
		// Ideally LoadoutManager keeps a persistent list that can be referenced, rather than refreshing logic elsewhere
		// Update the dictionary with the new lists
		AvailableWeapons[SlotType.Single] = LoadoutManager.GetInventoryByType(SlotType.Single);
		AvailableWeapons[SlotType.Dual] = LoadoutManager.GetInventoryByType(SlotType.Dual);
		AvailableWeapons[SlotType.Dual].AddRange(LoadoutManager.GetInventoryByType(SlotType.Single));
		AvailableWeapons[SlotType.System] = LoadoutManager.GetInventoryByType(SlotType.System);
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

	private Vector3 CalculateLocalPosition(float angleDegrees)
	{
		// Calculate distance modifier based on the original angle
		float normalisedAngle = Mathf.Abs((angleDegrees % 180) - 90);
		
		// Use a non-linear function to adjust the distanceModifier
		float t = normalisedAngle / 90;
		float distanceModifier = Mathf.SmoothStep(0, 1f, t); // Smoothly transition from 0 to 0.9

		// Apply a 90-degree counterclockwise offset for positioning
		float adjustedAngle = angleDegrees + 90;

		// Calculate offsets using the adjusted angle
		// Debug.Log("ANGLEDEGREES: " + angleDegrees + " DISTANCE MODIFIER: " + distanceModifier);
		float offsetX = Mathf.Cos(adjustedAngle * Mathf.Deg2Rad) * (WeaponNodeSelectorDistance * (1f + distanceModifier));
		float offsetY = Mathf.Sin(adjustedAngle * Mathf.Deg2Rad) * WeaponNodeSelectorDistance;

		// Create the new position vector
		Vector3 offset = new Vector3(offsetX, offsetY, 0);

		// Transform the offset to local coordinates
		Vector3 localPosition = PlayerManager.Inst.ActivePlayerShip.transform.localPosition + offset;
		localPosition.z = WeaponUIContainer.transform.position.z;

		return localPosition;
	}

	public void SetCursor(WeaponNode weaponNode)
	{
		WeaponNodeCursor?.DisableHover();
		WeaponNodeCursor = weaponNode;
		WeaponNodeCursor.EnableHover();
	}

	public void HandlePointerEnterOnNode(WeaponNode weaponNode) {
		if (WeaponNodeCursor.State == WeaponNode.NodeState.Selected) return;
		SetCursor(weaponNode);
	}

	public void HandlePointerExitOnNode(WeaponNode weaponNode) {
		if (WeaponNodeCursor != weaponNode || WeaponNodeCursor.State == WeaponNode.NodeState.Selected) return;
		WeaponNodeCursor.DisableHover();
	}

	public void HandlePointerClickOnNode(WeaponNode weaponNode) {
		if (WeaponNodeCursor.State == WeaponNode.NodeState.Selected) return;
		if (WeaponNodeCursor != weaponNode) SetCursor(weaponNode);
		WeaponNodeCursor.HandleSelect();
	}

	public override void HandleSelect()
	{
		WeaponNodeCursor?.HandleSelect();
	}

	public override void HandleBack()
	{
		Debug.Log("LOUDOUT HANDLE BACK");
		if (WeaponNodeCursor.State == WeaponNode.NodeState.Selected) WeaponNodeCursor.HandleDeselect();
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
		if (WeaponNodeCursor.State != WeaponNode.NodeState.Selected)
		{
			WeaponNode leftWeaponNode = DetermineAppropriateHorizontalNode(true);
			if (leftWeaponNode == null) return;
			SetCursor(leftWeaponNode);
		}
	}

	public override void HandleMoveRight()
	{
		if (WeaponNodeCursor.State != WeaponNode.NodeState.Selected)
		{
			WeaponNode rightWeaponNode = DetermineAppropriateHorizontalNode(true);
			if (rightWeaponNode == null) return;
			SetCursor(rightWeaponNode);
		}
	}

	public override void HandleMoveUp()
	{
		if (WeaponNodeCursor.State != WeaponNode.NodeState.Selected)
		{
			WeaponNode aboveWeaponNode = DetermineAppropriateVerticalNode(true);
			if (aboveWeaponNode == null) return;
			SetCursor(aboveWeaponNode);
		}
		else
		{
			WeaponNodeCursor.WeaponNodeSelector.ScrollUp();
		}
	}

	public override void HandleMoveDown()
	{
		if (WeaponNodeCursor.State != WeaponNode.NodeState.Selected)
		{
			WeaponNode belowWeaponNode = DetermineAppropriateVerticalNode(false);
			if (belowWeaponNode == null) return;
			SetCursor(belowWeaponNode);
		}
		else
		{
			WeaponNodeCursor.WeaponNodeSelector.ScrollDown();
		}
	}

	public WeaponNode DetermineAppropriateHorizontalNode(bool right)
	{
		if (WeaponNodeCursor.YPos == 0) {
			foreach (WeaponNode node in WeaponNodes)
			{
				if (node.YPos == 0 && ((right && node.XPos > WeaponNodeCursor.XPos) || (!right && node.XPos < WeaponNodeCursor.XPos))) return node;
			}
		}
		int currentIndex = WeaponNodes.IndexOf(WeaponNodeCursor);
		if (WeaponNodeCursor.YPos < 0 && currentIndex + 1 < WeaponNodes.Count)
		{
			return WeaponNodes[currentIndex + 1];
		}
		else if (WeaponNodeCursor.YPos > 0 && currentIndex - 1 >= 0)
		{
			return WeaponNodes[currentIndex - 1];
		}
		return WeaponNodes[0];
	}

	public WeaponNode DetermineAppropriateVerticalNode(bool up)
	{
		if (WeaponNodeCursor.XPos == 0) {
			foreach (WeaponNode node in WeaponNodes)
			{
				if (node.XPos == 0 && ((up && node.YPos > WeaponNodeCursor.YPos) || (!up && node.YPos < WeaponNodeCursor.YPos))) return node;
			}
		}
		int currentIndex = WeaponNodes.IndexOf(WeaponNodeCursor);
		if (WeaponNodeCursor.XPos < 0 && currentIndex + 1 < WeaponNodes.Count)
		{
			return WeaponNodes[currentIndex + 1];
		}
		else if (WeaponNodeCursor.XPos > 0 && currentIndex - 1 >= 0)
		{
			return WeaponNodes[currentIndex - 1];
		}
		return WeaponNodes[0];
	}
}