using System.Collections;
using System.Collections.Generic;
using Project.Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Crux.Utilities;

namespace Project.UI.Loadout
{
	public class WeaponNode : MonoBehaviour
	{
		// Inspector
		public Color HoverColor;
		public Color SelectedColor;
		public TextMeshProUGUI ID;
		
		[Header("Connection Line Settings")]
		[Tooltip("Distance in world units for the horizontal segment of the connection line")]
		public float HorizontalLineDistance = 2f;
		[Tooltip("Enable to visualize connection points for debugging")]
		public bool DebugConnectionPoints = false;

		// Data
		private bool Initialised = false;
		[HideInInspector] public float XPos;
		[HideInInspector] public float YPos;
		[HideInInspector] public RelativeSide Side;
		[HideInInspector] public WeaponNodeSelector WeaponNodeSelector;
		[HideInInspector] public NodeState State { get; private set; }
		private Color DefaultColor;
		private List<WeaponNode> LinkedWeaponNodes = new List<WeaponNode>();
		private AttachPoint AttachPoint;
		public WeaponSlot WeaponSlot;
		public bool IsDrawingLine = false;
		
		// Connection Line
		private WeaponNodeConnection connectionLine;

		// Temp
		int NodeId;

		// Types
		public enum NodeState
		{
			Default,
			Hover,
			Selected
		}

		void Awake() {
			State = NodeState.Default;
		}

		public void Init(AttachPoint attachPoint, WeaponSlot weaponSlot, int nodeId)
		{
			NodeId = nodeId;
			ID.text = NodeId.ToString();
			WeaponSlot = weaponSlot;
			AttachPoint = attachPoint;
			Side = AttachPoint.Side;
			XPos = attachPoint.transform.localPosition.x;
			YPos = attachPoint.transform.localPosition.y;
		}

		public void AssignSelector(WeaponNodeSelector weaponNodeSelector)
		{
			if (weaponNodeSelector == null)
			{
				Debug.LogError("Cannot assign null WeaponNodeSelector");
				return;
			}

			if (WeaponSlot == null) Debug.LogWarning("WeaponSlot not set on Node");
			if (AttachPoint == null) Debug.LogWarning("AttachPoint not set on Node");

			WeaponNodeSelector = weaponNodeSelector;
			WeaponNodeSelector.UpdateContent(AttachPoint, WeaponSlot, this, NodeId);
			Initialised = true;
			
			// Create connection line
			CreateConnectionLine();
			
			// Connection line will be activated later during the animation sequence
			// connectionLine.ActivateLine();
		}
		
		private void CreateConnectionLine()
		{
			// Check if we already have a connection line
			if (connectionLine != null)
			{
				Destroy(connectionLine.gameObject);
			}
			
			// Create a new GameObject for the WeaponNodeConnection component
			GameObject connectionLineObject = new GameObject($"WeaponNodeConnection_{NodeId}");
			connectionLineObject.transform.SetParent(LoadoutUI.Inst.WeaponUIContainer.transform);
			
			// Add the WeaponNodeConnection component
			connectionLine = connectionLineObject.AddComponent<WeaponNodeConnection>();
			
			// Configure the WeaponNodeConnection
			connectionLine.HorizontalLineDistance = HorizontalLineDistance;
			connectionLine.DebugConnectionPoints = DebugConnectionPoints;
			
			// Initialize the WeaponNodeConnection with the node and selector transforms
			connectionLine.InitializeForWeaponNode(transform, WeaponNodeSelector, Side, NodeId);
		}

		public void RefreshSelector() {
			WeaponNodeSelector.UpdateContent(AttachPoint, WeaponSlot, this, NodeId);
		}

		public void SetRelatedNodes(List<WeaponNode> weaponNodes)
		{
			foreach (WeaponNode weaponNode in weaponNodes)
			{
				if (weaponNode != this) LinkedWeaponNodes.Add(weaponNode);
			}
		}

		public void HandlePointerEnter() {
			if (!Initialised) return;
			LoadoutUI.Inst.HandlePointerEnterOnNode(this);
		}

		public void HandlePointerExit() {
			if (!Initialised) return;
			LoadoutUI.Inst.HandlePointerExitOnNode(this);
		}

		public void HandlePointerClick() {
			if (!Initialised) return;
			LoadoutUI.Inst.HandlePointerClickOnNode(this);
		}

		public void EnableHover()
		{
			if (!Initialised || State == NodeState.Hover || State == NodeState.Selected) return;
			
			// Play the hover enable animation
			Animator animator = GetComponent<Animator>();
			if (animator != null)
			{
				animator.Play("WeaponNodeHoverEnable");
			}
			
			// Set the state after starting the animation
			SetState(NodeState.Hover);
		}

		public void DisableHover()
		{
			if (!Initialised || State != NodeState.Hover || State == NodeState.Selected) return;
			
			// Play the hover disable animation
			Animator animator = GetComponent<Animator>();
			if (animator != null)
			{
				animator.Play("WeaponNodeHoverDisable");
			}
			
			// Set the state after starting the animation
			SetState(NodeState.Default);
		}
	
		/// <summary>
		/// Handles selection logic for the weapon node based on its current state.
		/// </summary>
		public void HandleSelect()
		{
			if (!Initialised) return;
			
			switch (State)
			{
				case NodeState.Default:
					// Do nothing when in default state
					return;
					
				case NodeState.Hover:
					// Transition from hover to selected state
					SetState(NodeState.Selected);
					// This will activate the weapon list
					
					// Shorten the connection line before activating the list
					if (connectionLine != null)
					{
						// Start a coroutine to shorten the line and wait for it to complete
						StartCoroutine(ShortenLineAndActivateList());
					}
					else
					{
						// If no connection line, just activate the list directly
						WeaponNodeSelector.HandleSelect();
					}
					break;
					
				case NodeState.Selected:
					// Already selected, so this is a second click
					// This will select the current weapon and close the list
					WeaponNodeSelector.HandleSelect();
					
					// After selecting a weapon, we should remain in hover state
					// The list will be closed by WeaponNodeSelector.HandleSelect()
					break;
			}
		}
		
		/// <summary>
		/// Coroutine that shortens the connection line and then activates the weapon list
		/// </summary>
		private IEnumerator ShortenLineAndActivateList()
		{
			IsDrawingLine = true;
			// Define distances based on the node's side
			float shortenDistance;
			float branchLength;
			float secondaryBranchLength;
			
			// Adjust distances based on the node's side
			if (Side == RelativeSide.Center)
			{
				// For center nodes, use larger distances
				shortenDistance = 0.8f;
				branchLength = 1.1f;
				secondaryBranchLength = 0.1f;
			}
			else
			{
				// For left/right nodes, use the original distances
				shortenDistance = 0.1f;
				branchLength = 0.35f;
				secondaryBranchLength = 0.1f;
			}
			
			// Shorten the line with the appropriate distance over 0.2 seconds
			yield return StartCoroutine(connectionLine.ShortenConnectionLine(shortenDistance, 0.125f));
			
			// Create branching lines from the shortened end point with appropriate distances
			// Parameters: branchLength, secondaryBranchLength, firstBranchDuration, secondaryBranchDuration
			yield return StartCoroutine(connectionLine.CreateBranchingLines(
				branchLength, 
				secondaryBranchLength, 
				0.2f,  // Keep first branch duration the same
				0.075f  // Keep secondary branch duration the same
			));

			IsDrawingLine = false;
			
			// After the line is shortened and branches are created, activate the weapon list
			WeaponNodeSelector.HandleSelect();
		}

		/// <summary>
		/// Handles deselection logic for the weapon node.
		/// </summary>
		public void HandleDeselect()
		{
			if (!Initialised) return;
			
			// If we're in selected state, transition back to hover state
			if (State == NodeState.Selected)
			{
				// Reset all linked nodes to Default state first
				foreach (WeaponNode weaponNode in LinkedWeaponNodes) {
					weaponNode.SetState(NodeState.Default);
				}
				
				// Then set this node to Hover state
				SetState(NodeState.Hover);
				
				// Restore the connection line to its original length
				if (connectionLine != null)
				{
					StartCoroutine(RestoreLineAndDeactivateList());
				}
				else
				{
					// If no connection line, just deactivate the list directly
					WeaponNodeSelector.HandleDeselect();
				}
			}
		}
		
		/// <summary>
		/// Coroutine that restores the connection line and then deactivates the weapon list
		/// </summary>
		private IEnumerator RestoreLineAndDeactivateList()
		{
			IsDrawingLine = true;
			// First deactivate the list
			WeaponNodeSelector.HandleDeselect();
			
			// Define animation durations
			float secondaryBranchShortenDuration = 0.1f;
			float primaryBranchShortenDuration = 0.15f;
			float mainLineDuration = 0.2f;
			
			// First, animate the shortening of all branching lines
			// This will handle secondary branches first, then primary branches
			yield return StartCoroutine(connectionLine.ShortenBranchingLines(primaryBranchShortenDuration, secondaryBranchShortenDuration));
			
			// Then restore the main connection line
			yield return StartCoroutine(connectionLine.RestoreConnectionLine(mainLineDuration));

			IsDrawingLine = false;
		}

		/// <summary>
		/// Sets the visual state of the node and handles related state transitions.
		/// </summary>
		/// <param name="newState">The target state to transition to</param>
		internal void SetState(NodeState newState)
		{
			if (!Initialised) return;
			
			// Don't process if already in the target state
			if (State == newState) return;
			
			// Handle exit actions for current state
			switch (State)
			{
				case NodeState.Hover:
					if (newState == NodeState.Default)
						WeaponNodeSelector.DisableHoverState();
					break;
					
				case NodeState.Selected:
					// We'll handle deselection in the HandleDeselect method
					// to properly manage the connection line restoration
					// So we don't call WeaponNodeSelector.HandleDeselect() here
					break;
			}
			
			// Handle enter actions for new state
			switch (newState)
			{
				case NodeState.Default:
					break;
					
				case NodeState.Hover:
					WeaponNodeSelector.EnableHoverState();
					break;
					
				case NodeState.Selected:
					// When transitioning to selected, also select linked nodes
					if (State == NodeState.Hover) {
						foreach (WeaponNode weaponNode in LinkedWeaponNodes) 
							weaponNode.SetState(NodeState.Selected);
					}
					break;
			}
			
			// Update the state
			State = newState;
		}
		
		private void OnDestroy()
		{
			// Clean up the connection line when the node is destroyed
			if (connectionLine != null)
			{
				Destroy(connectionLine.gameObject);
				connectionLine = null;
			}
		}

		public void ActivateConnectionLine()
		{
			if (connectionLine != null)
			{
				// Set the flag to indicate that the line is being drawn
				IsDrawingLine = true;
				
				// Activate the connection line
				connectionLine.ActivateLine();
				
				// Start a coroutine to wait for the line drawing to complete
				StartCoroutine(WaitForLineDrawingComplete());
			}
		}
		
		private IEnumerator WaitForLineDrawingComplete()
		{
			if (connectionLine == null)
			{
				IsDrawingLine = false;
				yield break;
			}
			
			// Wait for the line to be fully drawn (DrawDuration + a small buffer)
			yield return new WaitForSeconds(connectionLine.DrawDuration + 0.1f);
			
			// Set the flag to indicate that the line drawing is complete
			IsDrawingLine = false;
		}
	}
}