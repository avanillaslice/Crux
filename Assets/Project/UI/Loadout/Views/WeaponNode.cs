using System.Collections;
using System.Collections.Generic;
using Project.Ships;
using Project.UI.Loadout.Events;
using TMPro;
using UnityEngine;

namespace Project.UI.Loadout
{
	/// <summary>
	/// Represents a weapon attachment point in the Loadout UI.
	/// </summary>
	public class WeaponNode : MonoBehaviour
	{
		// Inspector
		[Header("Visual Settings")]
		[SerializeField] private Color hoverColor = Color.yellow;
		[SerializeField] private Color selectedColor = Color.green;
		[SerializeField] private TextMeshProUGUI idText;

		[Header("Connection Line Settings")]
		[SerializeField] private float horizontalLineDistance = 2f;
		[SerializeField] private bool debugConnectionPoints = false;

		// Data properties
		public int NodeId { get; private set; }
		public float XPos { get; private set; }
		public float YPos { get; private set; }
		public RelativeSide Side { get; private set; }
		public WeaponNodeSelector WeaponNodeSelector { get; private set; }
		public NodeState State { get; private set; }
		public WeaponSlot WeaponSlot { get; private set; }
		public bool IsDrawingLine { get; private set; }

		// Private fields
		private Color defaultColor;
		private List<WeaponNode> linkedWeaponNodes = new List<WeaponNode>();
		private AttachPoint attachPoint;
		private WeaponNodeConnection connectionLine;
		private bool isInitialized = false;

		// Types
		public enum NodeState
		{
			Default,
			Hover,
			Selected
		}

		void Awake()
		{
			State = NodeState.Default;

			// Validate references
			if (idText == null)
			{
				Debug.LogError("ID TextMeshProUGUI reference is missing on WeaponNode");
			}
		}

		void OnDestroy()
		{
			// Clean up the connection line when the node is destroyed
			if (connectionLine != null)
			{
				Destroy(connectionLine.gameObject);
				connectionLine = null;
			}
		}

		/// <summary>
		/// Initializes the weapon node with the necessary data.
		/// </summary>
		/// <param name="attachPoint">The attach point this node represents</param>
		/// <param name="weaponSlot">The weapon slot this node belongs to</param>
		/// <param name="nodeId">The unique ID for this node</param>
		public void Init(AttachPoint attachPoint, WeaponSlot weaponSlot, int nodeId)
		{
			if (attachPoint == null)
			{
				Debug.LogError("AttachPoint is null in WeaponNode.Init");
				return;
			}

			if (weaponSlot == null)
			{
				Debug.LogError("WeaponSlot is null in WeaponNode.Init");
				return;
			}

			NodeId = nodeId;

			if (idText != null)
			{
				idText.text = NodeId.ToString();
			}

			WeaponSlot = weaponSlot;
			this.attachPoint = attachPoint;
			Side = attachPoint.Side;
			XPos = attachPoint.transform.localPosition.x;
			YPos = attachPoint.transform.localPosition.y;
		}

		/// <summary>
		/// Assigns a selector to this node and creates a connection line.
		/// </summary>
		/// <param name="weaponNodeSelector">The selector to assign</param>
		public void AssignSelector(WeaponNodeSelector weaponNodeSelector)
		{
			if (weaponNodeSelector == null)
			{
				Debug.LogError("Cannot assign null WeaponNodeSelector");
				return;
			}

			if (WeaponSlot == null)
			{
				Debug.LogWarning("WeaponSlot not set on Node");
				return;
			}

			if (attachPoint == null)
			{
				Debug.LogWarning("AttachPoint not set on Node");
				return;
			}

			WeaponNodeSelector = weaponNodeSelector;
			WeaponNodeSelector.UpdateContent(attachPoint, WeaponSlot, this, NodeId);
			isInitialized = true;

			// Create connection line
			CreateConnectionLine();
		}

		/// <summary>
		/// Sets the related nodes for this node.
		/// </summary>
		/// <param name="weaponNodes">The list of related nodes</param>
		public void SetRelatedNodes(List<WeaponNode> weaponNodes)
		{
			if (weaponNodes == null)
			{
				Debug.LogError("weaponNodes is null in SetRelatedNodes");
				return;
			}

			linkedWeaponNodes.Clear();
			foreach (WeaponNode weaponNode in weaponNodes)
			{
				if (weaponNode != null && weaponNode != this)
				{
					linkedWeaponNodes.Add(weaponNode);
				}
			}
		}

		/// <summary>
		/// Refreshes the selector with updated data.
		/// </summary>
		public void RefreshSelector()
		{
			if (WeaponNodeSelector != null)
			{
				WeaponNodeSelector.UpdateContent(attachPoint, WeaponSlot, this, NodeId);
			}
		}

		/// <summary>
		/// Creates a connection line between this node and its selector.
		/// </summary>
		private void CreateConnectionLine()
		{
			if (WeaponNodeSelector == null)
			{
				Debug.LogError("WeaponNodeSelector is null in CreateConnectionLine");
				return;
			}

			// Check if we already have a connection line
			if (connectionLine != null)
			{
				Destroy(connectionLine.gameObject);
			}

			// Create a new GameObject for the WeaponNodeConnection component
			GameObject connectionLineObject = new GameObject($"WeaponNodeConnection_{NodeId}");
			if (connectionLineObject == null)
			{
				Debug.LogError("Failed to create connection line object");
				return;
			}

			connectionLineObject.transform.SetParent(transform.parent);

			// Add the WeaponNodeConnection component
			connectionLine = connectionLineObject.AddComponent<WeaponNodeConnection>();
			if (connectionLine == null)
			{
				Debug.LogError("Failed to add WeaponNodeConnection component");
				Destroy(connectionLineObject);
				return;
			}

			// Configure the WeaponNodeConnection
			connectionLine.HorizontalLineDistance = horizontalLineDistance;
			connectionLine.DebugConnectionPoints = debugConnectionPoints;

			// Initialize the WeaponNodeConnection with the node and selector transforms
			connectionLine.InitializeForWeaponNode(transform, WeaponNodeSelector, Side, NodeId);
		}

		/// <summary>
		/// Activates the connection line for this node.
		/// </summary>
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

		/// <summary>
		/// Waits for the connection line drawing to complete.
		/// </summary>
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

			// Trigger the event
			LoadoutEvents.TriggerConnectionLineDrawComplete(this);
		}

		/// <summary>
		/// Handles pointer enter events.
		/// </summary>
		public void HandlePointerEnter()
		{
			if (!isInitialized) return;
			LoadoutEvents.TriggerNodeHoverEnter(this);
		}

		/// <summary>
		/// Handles pointer exit events.
		/// </summary>
		public void HandlePointerExit()
		{
			if (!isInitialized) return;
			LoadoutEvents.TriggerNodeHoverExit(this);
		}

		/// <summary>
		/// Handles pointer click events.
		/// </summary>
		public void HandlePointerClick()
		{
			if (!isInitialized) return;

			if (State == NodeState.Hover)
			{
				HandleSelect();
			}
		}

		/// <summary>
		/// Enables the hover state for this node.
		/// </summary>
		public void EnableHover()
		{
			if (!isInitialized || State == NodeState.Hover || State == NodeState.Selected) return;

			// Play the hover enable animation
			Animator animator = GetComponent<Animator>();
			if (animator != null)
			{
				animator.Play("WeaponNodeHoverEnable");
			}

			// Set the state after starting the animation
			SetState(NodeState.Hover);
		}

		/// <summary>
		/// Disables the hover state for this node.
		/// </summary>
		public void DisableHover()
		{
			if (!isInitialized || State != NodeState.Hover || State == NodeState.Selected) return;

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
		/// Handles selection of this node.
		/// </summary>
		public void HandleSelect()
		{
			if (!isInitialized) return;

			switch (State)
			{
				case NodeState.Default:
					// Do nothing when in default state
					return;

				case NodeState.Hover:
					// Transition from hover to selected state
					SetState(NodeState.Selected);

					// Shorten the connection line before activating the list
					if (connectionLine != null)
					{
						// Start a coroutine to shorten the line and wait for it to complete
						StartCoroutine(ShortenLineAndActivateList());
					}
					else
					{
						// If no connection line, just activate the list directly
						if (WeaponNodeSelector != null)
						{
							WeaponNodeSelector.HandleSelect();
						}
					}

					// Trigger the event
					LoadoutEvents.TriggerNodeSelected(this);
					break;

				case NodeState.Selected:
					// Already selected, so this is a second click
					// This will select the current weapon and close the list
					if (WeaponNodeSelector != null)
					{
						WeaponNodeSelector.HandleSelect();
					}
					break;
			}
		}

		/// <summary>
		/// Handles deselection of this node.
		/// </summary>
		public void HandleDeselect()
		{
			if (!isInitialized) return;

			// If we're in selected state, transition back to hover state
			if (State == NodeState.Selected)
			{
				// Reset all linked nodes to Default state first
				foreach (WeaponNode weaponNode in linkedWeaponNodes)
				{
					if (weaponNode != null)
					{
						weaponNode.SetState(NodeState.Default);
					}
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
					if (WeaponNodeSelector != null)
					{
						WeaponNodeSelector.HandleDeselect();
					}
				}

				// Trigger the event
				LoadoutEvents.TriggerNodeDeselected(this);
			}
		}

		/// <summary>
		/// Shortens the connection line and activates the weapon list.
		/// </summary>
		private IEnumerator ShortenLineAndActivateList()
		{
			if (connectionLine == null || WeaponNodeSelector == null)
			{
				yield break;
			}

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

			// Shorten the line with the appropriate distance over 0.125 seconds
			yield return StartCoroutine(connectionLine.ShortenConnectionLine(shortenDistance, 0.125f));

			// Create branching lines from the shortened end point with appropriate distances
			yield return StartCoroutine(connectionLine.CreateBranchingLines(
				branchLength,
				secondaryBranchLength,
				0.2f,
				0.075f
			));

			IsDrawingLine = false;

			// After the line is shortened and branches are created, activate the weapon list
			WeaponNodeSelector.HandleSelect();
		}

		/// <summary>
		/// Restores the connection line and deactivates the weapon list.
		/// </summary>
		private IEnumerator RestoreLineAndDeactivateList()
		{
			if (connectionLine == null || WeaponNodeSelector == null)
			{
				yield break;
			}

			IsDrawingLine = true;

			// First deactivate the list
			WeaponNodeSelector.HandleDeselect();

			// Define animation durations
			float secondaryBranchShortenDuration = 0.1f;
			float primaryBranchShortenDuration = 0.15f;
			float mainLineDuration = 0.2f;

			// First, animate the shortening of all branching lines
			yield return StartCoroutine(connectionLine.ShortenBranchingLines(primaryBranchShortenDuration, secondaryBranchShortenDuration));

			// Then restore the main connection line
			yield return StartCoroutine(connectionLine.RestoreConnectionLine(mainLineDuration));

			IsDrawingLine = false;
		}

		/// <summary>
		/// Sets the state of this node.
		/// </summary>
		/// <param name="newState">The new state</param>
		internal void SetState(NodeState newState)
		{
			if (!isInitialized) return;

			// Don't process if already in the target state
			if (State == newState) return;

			// Handle exit actions for current state
			switch (State)
			{
				case NodeState.Hover:
					if (newState == NodeState.Default && WeaponNodeSelector != null)
						WeaponNodeSelector.DisableHoverState();
					break;

				case NodeState.Selected:
					// We'll handle deselection in the HandleDeselect method
					break;
			}

			// Handle enter actions for new state
			switch (newState)
			{
				case NodeState.Default:
					break;

				case NodeState.Hover:
					if (WeaponNodeSelector != null)
						WeaponNodeSelector.EnableHoverState();
					break;

				case NodeState.Selected:
					// When transitioning to selected, also select linked nodes
					if (State == NodeState.Hover)
					{
						foreach (WeaponNode weaponNode in linkedWeaponNodes)
						{
							if (weaponNode != null)
								weaponNode.SetState(NodeState.Selected);
						}
					}
					break;
			}

			// Update the state
			State = newState;
		}
	}
}