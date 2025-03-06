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
		[HideInInspector] public NodeState State;
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
			// Debug.Log("NEW NODE POSTION X: " + XPos + " Y: " + YPos);
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
			if (!Initialised) return;
			if (State == NodeState.Hover || State == NodeState.Selected) return;
			
			// Set state first
			SetState(NodeState.Hover);
			
			// Then activate the line - this ensures the line is drawn even if we quickly hover in and out
			if (connectionLine != null)
			{
				connectionLine.ActivateLine();
			}
		}

		public void DisableHover()
		{
			if (!Initialised) return;
			if (State != NodeState.Hover || State == NodeState.Selected) return;
			
			// No longer deactivate the line - we want it to remain visible permanently
			// if (connectionLine != null)
			// {
			//     connectionLine.DeactivateLine();
			// }
			
			// Then set state
			SetState(NodeState.Default);
		}
	
		public void HandleSelect()
		{
			if (!Initialised || State == NodeState.Default) return;
			else if (State == NodeState.Hover) {
				SetState(NodeState.Selected);
				WeaponNodeSelector.HandleSelect();
				
				// Keep the line active when selected
				if (connectionLine != null)
				{
					connectionLine.ActivateLine();
				}
				return;
			}
			
			WeaponNodeSelector.HandleSelect();
			AssignSelector(WeaponNodeSelector);
			SetState(NodeState.Hover);
			foreach (WeaponNode weaponNode in LinkedWeaponNodes) {
				weaponNode.RefreshSelector();
			}
			
			// Activate the line when selected
			if (connectionLine != null)
			{
				connectionLine.ActivateLine();
			}
		}

		public void HandleDeselect()
		{
			if (!Initialised) return;
			if (State != NodeState.Selected) return;
			
			SetState(NodeState.Hover);
			WeaponNodeSelector.HandleDeselect();
			
			// Keep the line active when returning to hover state
			if (connectionLine != null)
			{
				connectionLine.ActivateLine();
			}
		}

		internal void SetState(NodeState state)
		{
			if (!Initialised) return;
			switch (state)
			{
				case NodeState.Default: {
					ColorComponent.color = DefaultColor;
					if (State == NodeState.Hover) WeaponNodeSelector.DisableHoverState();
					if (State == NodeState.Selected) WeaponNodeSelector.HandleDeselect();
					break;
				}
				case NodeState.Hover: {
					if (State == NodeState.Selected) {
						WeaponNodeSelector.HandleDeselect();
						foreach (WeaponNode weaponNode in LinkedWeaponNodes) weaponNode.SetState(NodeState.Default);
					}
					ColorComponent.color = HoverColor;
					WeaponNodeSelector.EnableHoverState();
					break;
				}
				case NodeState.Selected: {
					ColorComponent.color = SelectedColor;
					if (State == NodeState.Hover) {
						foreach (WeaponNode weaponNode in LinkedWeaponNodes) weaponNode.SetState(NodeState.Selected);
					}
					break;
				}
			}
			State = state;
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