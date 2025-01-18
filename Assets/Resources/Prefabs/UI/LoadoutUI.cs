using System.Collections.Generic;
using UnityEngine;

public class LoadoutUI : UIWindowBase
{
	public static LoadoutUI Inst { get; private set; }

	// Inspector
	public GameObject WeaponSlotUIContainer;
	public GameObject InitialWeaponSlotCursor;

	// Data
	private float WeaponNodeSelectorDistance = 4f;
	private bool FirstWeaponNodeIsCentered = true;
	private List<WeaponNode> WeaponNodes = new List<WeaponNode>();
	private List<WeaponNodeSelector> WeaponNodeSelectors = new List<WeaponNodeSelector>();
	private List<List<WeaponNode>> WeaponNodeGroups = new List<List<WeaponNode>>();
	private List<GameObject> ActiveLoadout;

	private WeaponNode WeaponSlotCursor;

	public List<WeaponBase> LightWeapons;
	public List<WeaponBase> MediumWeapons;
	public List<WeaponBase> HeavyWeapons;

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
		InitialiseLoadoutUI();
	}

	void OnEnable()
	{
		UpdateAvailableWeapons();
		SetInitialCursor();
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
		UpdateAvailableWeapons();
	}

	// Fetch AttachPoints, Sort by YPOS, Sort into Left/Right/Middle
	private void InstantiateWeaponNodes()
	{
		Debug.Log("InstantiateWeaponNodes");
		// Find all AttachPoints from PlayerShip
		List<WeaponSlot> weaponSlots = PlayerManager.Inst.ActivePlayerShip.WeaponSlots;

		// Sort attachPoints by YPOS (what kind of list do I use for static order?)
		foreach (WeaponSlot weaponSlot in weaponSlots)
		{
			List<WeaponNode> relatedWeaponNodes = new List<WeaponNode>();

			// Instantiate and group WeaponNodes
			foreach (AttachPoint attachPoint in weaponSlot.AttachPoints)
			{
				// FetchGameObject for transform position
				Vector3 posAbovePlayer = new Vector3(attachPoint.transform.position.x, attachPoint.transform.position.y, 11); // One unit above player
				WeaponNode weaponNode = Instantiate(AssetManager.WeaponNodePrefab, attachPoint.transform.position, Quaternion.identity).GetComponent<WeaponNode>();
				weaponNode.Init(attachPoint, weaponSlot);

				// Add to group
				relatedWeaponNodes.Add(weaponNode);
				WeaponNodes.Add(weaponNode);

				// Not sure if I need this now that they are partnered up
				// if (WeaponNode.transform.position.x > 0) LeftWeaponNodes.Add(WeaponNode);
				// else if (WeaponNode.transform.position.x < 0) RightWeaponNodes.Add(WeaponNode);
				// else CentralWeaponNodes.Add(WeaponNode); // Maybe? Just felt weird it not being in a list
			}

			// Assign related nodes to each group
			foreach (WeaponNode weaponNode in relatedWeaponNodes)
			{
				weaponNode.SetRelatedNodes(relatedWeaponNodes);
				WeaponNodeGroups.Add(relatedWeaponNodes);
			}
		}

		// Sort WeaponNodes by YPos
		// Does this mean I dont need to for WeaponNodeGroups?
		WeaponNodes.Sort((x, y) => x.YPos.CompareTo(y.YPos));
		WeaponNodeGroups.Sort((listA, listB) => listA[0].YPos.CompareTo(listB[0].YPos));
	}

	private void UpdateAvailableWeapons() {
		// LoadoutManager should return a pre sorted loadout package
		List<GameObject> ActiveLoadout = LoadoutManager.GetInventory();

		// So we dont have to do this....
	    // LightWeapons = LoadoutManager.FetchWeapons(WeaponSlotType.Light);
	    // MediumWeapons = LoadoutManager.FetchWeapons(WeaponSlotType.Medium);
	    // HeavyWeapons = LoadoutManager.FetchWeapons(WeaponSlotType.Heavy);
	}

	private void InitialiseWeaponNodeSelectors()
	{
		Debug.Log("InitialiseWeaponNodeSelectors");
		List<Vector3> WeaponNodeSelectorPositions = DetermineWeaponNodeSelectorPositions();
		InstantiateWeaponNodeSelectors(WeaponNodeSelectorPositions);
		LinkNodesToSelectors();
	}

	private List<Vector3> DetermineWeaponNodeSelectorPositions()
	{
		Debug.Log("DetermineWeaponNodeSelectorPositions");
		List<Vector3> weaponNodeSelectorPositions = new List<Vector3>();

		// Angle between each WeaponNodeSelector
		float anglePerSelector = 360 / WeaponNodes.Count;

		// Determine first WeaponNodeSelector position
		Vector3 FirstWeaponNodeSelectorPosition;
		if (WeaponNodes[0].XPos == 0)
		{
			Debug.Log("FIRST NODE IS CENTERED");
			FirstWeaponNodeIsCentered = true;
			FirstWeaponNodeSelectorPosition = CalculateLocalPosition(0);
		}
		else
		{
			Debug.Log("FIRST NODE IS NOT CENTERED");
			FirstWeaponNodeIsCentered = false;
			FirstWeaponNodeSelectorPosition = CalculateLocalPosition(anglePerSelector / 2);
		}

		// Calculate each position and add to list
		int i = 0;
		while (i < WeaponNodes.Count)
		{
			if (i == 0) weaponNodeSelectorPositions.Add(FirstWeaponNodeSelectorPosition);
			else
			{
				Vector3 newPosition = CalculateLocalPosition(anglePerSelector * i);
				weaponNodeSelectorPositions.Add(newPosition);
			}
			i++;
		}

		return weaponNodeSelectorPositions;
	}

	private void InstantiateWeaponNodeSelectors(List<Vector3> WeaponNodeSelectorPositions)
	{
		Debug.Log("InstantiateWeaponNodeSelectors");
		if (WeaponNodeSelectorPositions.Count == 0)
		{
			Debug.LogError("No WeaponNodeSelectorPositions have been set");
			// This should throw error to cease other functions
			return;
		}

		foreach (Vector3 WeaponNodeSelectorPosition in WeaponNodeSelectorPositions)
		{
			GameObject WeaponNodeSelector = Instantiate(AssetManager.WeaponNodeSelectorPrefab, WeaponNodeSelectorPosition, Quaternion.identity);

			WeaponNodeSelectors.Add(WeaponNodeSelector.GetComponent<WeaponNodeSelector>());
		}
	}

	private void LinkNodesToSelectors()
	{
		Debug.Log("LinkNodesToSelectors");
		// WeaponNodeGroups and WeaponNodes have been sorted by YPos
		int nodeCounter = 0;
		int nodeGroupCounter = 0;
		WeaponNode RearAsymmetricalWeaponNode;
		Debug.Log("Detected " + WeaponNodes.Count + " WeaponNodes");
		Debug.Log("Detected " + WeaponNodeSelectors.Count + " WeaponNodeSelectors");
		while (nodeCounter < WeaponNodes.Count)
		{
			Debug.Log("Placing Node: " + nodeCounter);
			Debug.Log("Placing Group: " + nodeGroupCounter);
			// Handle first WeaponNodeGroup
			if (nodeCounter == 0 && FirstWeaponNodeIsCentered)
			{
				Debug.Log("FIRSTWEAPONNODE IS CENTERED");
				WeaponNodes[0].AssignSelector(WeaponNodeSelectors[0]);
				nodeCounter++;
			}
			else
			{
				foreach (WeaponNode weaponNode in WeaponNodeGroups[nodeGroupCounter])
				{
					if (weaponNode.XPos > 0)
					{ // On the right
						weaponNode.AssignSelector(WeaponNodeSelectors[nodeCounter]);
					}
					else if (weaponNode.XPos < 0)
					{ // On the left
						weaponNode.AssignSelector(WeaponNodeSelectors[WeaponNodes.Count - nodeCounter]);
					}
					else
					{ // Centered
						// Needs to save it until last, OR divide the total count by two to get the lowest selector...
						// Log error if already set, can only have one of these...
						RearAsymmetricalWeaponNode = weaponNode;
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
		float offsetX = Mathf.Cos(angleRadians) * WeaponNodeSelectorDistance;
		float offsetY = Mathf.Sin(angleRadians) * WeaponNodeSelectorDistance;

		// Create the new position vector
		Vector3 offset = new Vector3(offsetX, offsetY, 0);

		// Transform the offset to local coordinates
		Vector3 localPosition = PlayerManager.Inst.ActivePlayerShip.transform.localPosition + offset;

		return localPosition;
	}

	private void SetInitialCursor()
	{
		WeaponNode weaponNode = InitialWeaponSlotCursor.GetComponent<WeaponNode>();
		SetCursor(weaponNode);
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
		// else Trigger termination animations and return to InterScene
	}

	public override void HandleExit() {
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

	private WeaponNode DetermineAppropriateHoizontalNode(bool up) {
		return new WeaponNode();
	}

	private WeaponNode DetermineAppropriateVerticalNode(bool left) {
		return new WeaponNode();
	}
}