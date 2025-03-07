using System.Collections;
using System.Collections.Generic;
using Project.Combat.Weapons;
using Project.Core;
using Project.Ships;
using UnityEngine;

namespace Project.UI.Loadout
{
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

		private bool isInitializationComplete = false;
		private int pendingConnectionLines = 0;

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
			PlayerManager.Inst.ActivePlayerShip.DeactivateShield();
			UpdateAvailableWeapons();
			InitialiseLoadoutUI();
		}

		void OnDisable()
		{
			PlayerManager.Inst.ActivePlayerShip.transform.localScale -= new Vector3(1f, 1f, 1f);
			PlayerManager.Inst.ActivePlayerShip.ActivateShield();
			// Reset cursor state to avoid lingering visual effects
			if (WeaponNodeCursor != null)
			{
				WeaponNodeCursor.SetState(WeaponNode.NodeState.Default);
				WeaponNodeCursor = null;
			}
			
			// Clean up all UI elements
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
			
			// Start the animation sequence
			StartCoroutine(PlayStartupAnimations());
		}

		/// <summary>
		/// Plays the startup animations for the LoadoutUI in sequence.
		/// First animates each WeaponNode with a delay between them,
		/// then activates all connection lines simultaneously.
		/// </summary>
		private IEnumerator PlayStartupAnimations()
		{
			// Reset the initialization state
			isInitializationComplete = false;
			pendingConnectionLines = WeaponNodes.Count;
			
			// Start an isolated animation sequence for each weapon node
			foreach (WeaponNode weaponNode in WeaponNodes)
			{
				StartCoroutine(PlayNodeAnimation(weaponNode));
				
				// Wait for the specified delay before starting the next node's animation sequence
				yield return new WaitForSeconds(0.35f);
			}
		}
		
		/// <summary>
		/// Plays the initialization animation for a single weapon node and then activates its connection line.
		/// This ensures each node's connection line is activated only after its animation completes.
		/// </summary>
		/// <param name="weaponNode">The weapon node to animate</param>
		private IEnumerator PlayNodeAnimation(WeaponNode weaponNode)
		{
			// Get the Animator component from the WeaponNode
			Animator animator = weaponNode.GetComponent<Animator>();
			if (animator != null)
			{
				// Play the WeaponNodeInit animation
				animator.Play("WeaponNodeInit");
				
				// Wait for the animation to complete
				// Get the length of the current animation
				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
				float animationLength = stateInfo.length;
				
				// Wait for the animation to complete
				yield return new WaitForSeconds(animationLength);
				
				// Activate the connection line after the animation completes
				weaponNode.ActivateConnectionLine();
				
				// Start a coroutine to wait for the connection line to finish drawing
				StartCoroutine(WaitForConnectionLineComplete(weaponNode));
			}
			else
			{
				Debug.LogWarning("Animator component not found on WeaponNode");
				// Still activate the connection line even if there's no animator
				weaponNode.ActivateConnectionLine();
				
				// Start a coroutine to wait for the connection line to finish drawing
				StartCoroutine(WaitForConnectionLineComplete(weaponNode));
			}
		}

		// Add a new method to wait for connection lines to complete
		private IEnumerator WaitForConnectionLineComplete(WeaponNode weaponNode)
		{
			// Wait until the weapon node's connection line is no longer drawing
			// This assumes the WeaponNodeConnection sets IsDrawingLine to false when complete
			while (weaponNode.IsDrawingLine)
			{
				yield return null;
			}
			
			// Decrement the counter of pending connection lines
			pendingConnectionLines--;
			
			// If all connection lines are complete, mark initialization as complete
			if (pendingConnectionLines <= 0)
			{
				isInitializationComplete = true;
				Debug.Log("Loadout UI initialization complete - interactions now enabled");
			}
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
				for (int j = weaponSlot.AttachPoints.Count - 1; j >= 0; j--)
				{
					AttachPoint attachPoint = weaponSlot.AttachPoints[j];
					// FetchGameObject for transform position
					Vector3 posAbovePlayer = new Vector3(attachPoint.transform.position.x, attachPoint.transform.position.y + 0.05f, WeaponUIContainer.transform.position.z); // One unit above player
					WeaponNode weaponNode = Instantiate(AssetManager.WeaponNodePrefab, posAbovePlayer, Quaternion.identity, WeaponUIContainer.transform).GetComponent<WeaponNode>();
					weaponNode.Init(attachPoint, weaponSlot, i);

					// Add to group
					relatedWeaponNodes.Add(weaponNode);
					WeaponNodes.Add(weaponNode);
					i++;
				}


				// OLD LOOP 
				// foreach (AttachPoint attachPoint in weaponSlot.AttachPoints)
				// {
				// 	// FetchGameObject for transform position
				// 	Vector3 posAbovePlayer = new Vector3(attachPoint.transform.position.x, attachPoint.transform.position.y, WeaponUIContainer.transform.position.z); // One unit above player
				// 	WeaponNode weaponNode = Instantiate(AssetManager.WeaponNodePrefab, posAbovePlayer, Quaternion.identity, WeaponUIContainer.transform).GetComponent<WeaponNode>();
				// 	weaponNode.Init(attachPoint, weaponSlot, i);

				// 	// Add to group
				// 	relatedWeaponNodes.Add(weaponNode);
				// 	WeaponNodes.Add(weaponNode);
				// 	i++;
				// }

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
			// Debug.Log("WeaponNodes Instantated!");
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
				// Debug.Log($"New Selector Instantiated: {weaponNodeSelectorPosition}");
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
			WeaponNode RearAsymmetricalWeaponNode = null;
			// Debug.Log("Detected " + WeaponNodes.Count + " WeaponNodes");
			// Debug.Log("Detected " + WeaponNodeSelectors.Count + " WeaponNodeSelectors");
			foreach (WeaponNodeSelector selector in WeaponNodeSelectors)
			{
				// Debug.Log($"Selector Position: X: {selector.transform.position.x} Y: {selector.transform.position.y}");
			}
			while (nodeCounter < WeaponNodes.Count)
			{
				foreach (WeaponNode weaponNode in WeaponNodeGroups[nodeGroupCounter])
				{
					// Debug.Log($"Linking Node: {weaponNode.ID.text} X: {weaponNode.XPos} Y: {weaponNode.YPos} Side: {weaponNode.Side}");

					switch (weaponNode.Side)
					{
						case RelativeSide.Right:
						{
							int index = nodeGroupCounter - (RearAsymmetricalWeaponNode == null ? 0 : 1);
							weaponNode.AssignSelector(WeaponNodeSelectors[index]);
							break;
						}
						case RelativeSide.Left:
						{
							int index = WeaponNodes.Count - nodeGroupCounter + (RearAsymmetricalWeaponNode == null ? 0 : 1) - (FirstWeaponNodeIsCentered ? 0 : 1);
							weaponNode.AssignSelector(WeaponNodeSelectors[index]);
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
								if (RearAsymmetricalWeaponNode != null)
								{
									RearAsymmetricalWeaponNode.AssignSelector(WeaponNodeSelectors[WeaponNodes.Count - nodeGroupCounter]);
									weaponNode.AssignSelector(WeaponNodeSelectors[nodeGroupCounter]);
									RearAsymmetricalWeaponNode = null;
								}
								else
								{
									RearAsymmetricalWeaponNode = weaponNode; // Replace this with AddOrFetchRearAsymmetricalWeaponNode() logic
								}
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
			if (RearAsymmetricalWeaponNode != null)
			{
				int remainingSelectorIndex = FetchRemainingSelectorIndex();
				if (remainingSelectorIndex == -1) Debug.LogError("Could not find remainingSelector");
				else RearAsymmetricalWeaponNode.AssignSelector(WeaponNodeSelectors[remainingSelectorIndex]);
			}
			// Debug.Log("Nodes and Selectors Linked!");
		}

		private int FetchRemainingSelectorIndex()
		{
			foreach (WeaponNodeSelector selector in WeaponNodeSelectors)
			{
				if (selector.ID.text == "abc") return WeaponNodeSelectors.IndexOf(selector);
			}
			return -1;
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
				FirstWeaponNodeSelectorPosition = CalculateLocalPosition(anglePerSelector / 2);
			}

			// Calculate each position and add to list
			int i = 0;
			while (i < WeaponNodes.Count)
			{
				if (i == 0)
				{
					weaponNodeSelectorPositions.Add(FirstWeaponNodeSelectorPosition);
					// Debug.Log($"New Selector Position: {FirstWeaponNodeSelectorPosition}");
				}
				else
				{
					float originalWeaponNodeAngle = FirstWeaponNodeIsCentered ? 0 : (anglePerSelector / 2);
					// Debug.Log($"Iterator: {i}");
					// Debug.Log($"OriginalNodeAngle: {originalWeaponNodeAngle}");
					// Debug.Log($"anglePerSelector: {anglePerSelector}");
					Vector3 newPosition = CalculateLocalPosition(originalWeaponNodeAngle + anglePerSelector * i);
					weaponNodeSelectorPositions.Add(newPosition);
					// Debug.Log($"New Selector Position: {newPosition}");
				}
				i++;
			}
			// Debug.Log("Positions:");
			// foreach (Vector3 position in weaponNodeSelectorPositions)
			// {

			// 	Debug.Log(position);
			// }
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
			float offsetX = -Mathf.Cos(adjustedAngle * Mathf.Deg2Rad) * (WeaponNodeSelectorDistance * (1f + distanceModifier));
			float offsetY = Mathf.Sin(adjustedAngle * Mathf.Deg2Rad) * WeaponNodeSelectorDistance;
			offsetY *= angleDegrees % 90 == 0 ? 1f : 0.9f;

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

		public void HandlePointerEnterOnNode(WeaponNode weaponNode)
		{
			// Ignore interactions until initialization is complete
			if (!isInitializationComplete) return;
			
			// Set the cursor to this node
			SetCursor(weaponNode);
			
			// Enable hover state
			WeaponNodeCursor.EnableHover();
		}

		public void HandlePointerExitOnNode(WeaponNode weaponNode)
		{
			// Ignore interactions until initialization is complete
			if (!isInitializationComplete) return;
			
			// Disable hover state
			WeaponNodeCursor.DisableHover();
		}

		public void HandlePointerClickOnNode(WeaponNode weaponNode)
		{
			// Ignore interactions until initialization is complete
			if (!isInitializationComplete) return;
			
			// If another node is already selected, ignore clicks on other nodes
			if (WeaponNodeCursor != null && WeaponNodeCursor != weaponNode && 
			    WeaponNodeCursor.State == WeaponNode.NodeState.Selected) return;
			
			// Set as cursor if it's not already
			if (WeaponNodeCursor != weaponNode) SetCursor(weaponNode);
			
			// Handle the selection
			WeaponNodeCursor.HandleSelect();
		}

		public override void HandleSelect()
		{
			// Ignore interactions until initialization is complete
			if (!isInitializationComplete) return;
			
			// Forward the select action to the current cursor node
			WeaponNodeCursor?.HandleSelect();
		}

		public override void HandleBack()
		{
			// HandleBack should work even when initialization is not complete
			// If a node is selected, deselect it, otherwise handle normal back behavior
			if (WeaponNodeCursor != null && WeaponNodeCursor.State == WeaponNode.NodeState.Selected) 
				WeaponNodeCursor.HandleDeselect();
			else
				// Handle normal back behavior (e.g., exit the loadout screen)
				HandleExit();
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
			// Ignore interactions until initialization is complete
			if (!isInitializationComplete) return;
			
			if (WeaponNodeCursor.State != WeaponNode.NodeState.Selected)
			{
				WeaponNode leftWeaponNode = DetermineAppropriateHorizontalNode(true);
				if (leftWeaponNode == null) return;
				SetCursor(leftWeaponNode);
			}
		}

		public override void HandleMoveRight()
		{
			// Ignore interactions until initialization is complete
			if (!isInitializationComplete) return;
			
			if (WeaponNodeCursor.State != WeaponNode.NodeState.Selected)
			{
				WeaponNode rightWeaponNode = DetermineAppropriateHorizontalNode(true);
				if (rightWeaponNode == null) return;
				SetCursor(rightWeaponNode);
			}
		}

		public override void HandleMoveUp()
		{
			// Ignore interactions until initialization is complete
			if (!isInitializationComplete) return;
			
			if (WeaponNodeCursor.State != WeaponNode.NodeState.Selected)
			{
				WeaponNode aboveWeaponNode = DetermineAppropriateVerticalNode(true);
				if (aboveWeaponNode == null) return;
				SetCursor(aboveWeaponNode);
			}
			else
			{
				if (WeaponNodeCursor.IsDrawingLine) return;
				WeaponNodeCursor.WeaponNodeSelector.ScrollUp();
			}
		}

		public override void HandleMoveDown()
		{
			// Ignore interactions until initialization is complete
			if (!isInitializationComplete) return;
			
			if (WeaponNodeCursor.State != WeaponNode.NodeState.Selected)
			{
				WeaponNode belowWeaponNode = DetermineAppropriateVerticalNode(false);
				if (belowWeaponNode == null) return;
				SetCursor(belowWeaponNode);
			}
			else
			{
				if (WeaponNodeCursor.IsDrawingLine) return;
				WeaponNodeCursor.WeaponNodeSelector.ScrollDown();
			}
		}

		public WeaponNode DetermineAppropriateHorizontalNode(bool right)
		{
			if (WeaponNodeCursor.YPos == 0)
			{
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
			if (WeaponNodeCursor.XPos == 0)
			{
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
}