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
		private Image ColorComponent;
		private Color DefaultColor;
		private List<WeaponNode> LinkedWeaponNodes = new List<WeaponNode>();
		private AttachPoint AttachPoint;
		public WeaponSlot WeaponSlot;
		
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
			ColorComponent = gameObject.GetComponent<Image>();
			DefaultColor = ColorComponent.color;
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
			
			// Activate the line and keep it permanently drawn
			connectionLine.ActivateLine();
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
			SetState(NodeState.Hover);
		}

		public void DisableHover()
		{
			if (!Initialised || State != NodeState.Hover || State == NodeState.Selected) return;
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
					WeaponNodeSelector.HandleSelect();
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
		/// Handles deselection logic for the weapon node.
		/// </summary>
		public void HandleDeselect()
		{
			if (!Initialised || State != NodeState.Selected) return;
			
			// Reset all linked nodes to Default state first
			foreach (WeaponNode weaponNode in LinkedWeaponNodes) {
				weaponNode.SetState(NodeState.Default);
			}
			
			// Then set this node to Hover state
			SetState(NodeState.Hover);
			WeaponNodeSelector.HandleDeselect();
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
					if (newState != NodeState.Hover)
						WeaponNodeSelector.DisableHoverState();
					break;
					
				case NodeState.Selected:
					if (newState != NodeState.Selected)
						WeaponNodeSelector.HandleDeselect();
					break;
			}
			
			// Handle enter actions for new state
			switch (newState)
			{
				case NodeState.Default:
					ColorComponent.color = DefaultColor;
					break;
					
				case NodeState.Hover:
					ColorComponent.color = HoverColor;
					WeaponNodeSelector.EnableHoverState();
					break;
					
				case NodeState.Selected:
					ColorComponent.color = SelectedColor;
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
	}
}