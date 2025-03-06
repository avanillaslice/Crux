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
		private Color LineDefaultColor;
		private float LineWidth { get { return 0.035f; }}
		private float DrawDuration { get { return 1f; }}
		[Tooltip("Distance in world units for the horizontal segment of the connection line")]
		private float HorizontalLineDistance { get { return 2f; }}
		[Tooltip("Enable to visualize connection points for debugging")]
		private bool DebugConnectionPoints = false;

		[Header("Cell Size Settings")]
		[Tooltip("Base width of the selector cell for line connection calculations")]
		public float CellBaseWidth = 2.0f;
		[Tooltip("Base height of the selector cell for line connection calculations")]
		public float CellBaseHeight = 1.0f;

	
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
		private GameObject lineObject;
		private LineRenderer lineRenderer;
		private bool isLineActive = false;
		private Coroutine activeLineCoroutine;

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
			LineDefaultColor = ColorManager.Instance.GetColor("WeaponNodeBorder");
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
			ActivateLine();
		}
		
		private void CreateConnectionLine()
		{
			// Create a new GameObject for the line
			lineObject = new GameObject($"ConnectionLine_{NodeId}");
			lineObject.transform.SetParent(LoadoutUI.Inst.WeaponUIContainer.transform);
			
			// Add the LineRenderer component
			lineRenderer = lineObject.AddComponent<LineRenderer>();
			
			lineRenderer.startWidth = LineWidth;
			lineRenderer.endWidth = LineWidth;
			
			// Ensure the LineRenderer uses world space positions
			lineRenderer.useWorldSpace = true;
			
			// Set position count to 3 for the two-segment line (node -> horizontal point -> cell)
			lineRenderer.positionCount = 3;
			
			// Try to use our custom shader or fall back to a built-in shader
			Shader connectionLineShader = Shader.Find("Custom/ConnectionLine");
			if (connectionLineShader == null)
			{
				// Try to load from Resources folder as fallback
				Material connectionLineMaterial = Resources.Load<Material>("Materials/ConnectionLine");
				if (connectionLineMaterial != null)
				{
					lineRenderer.material = new Material(connectionLineMaterial);
				}
				else
				{
					// Fall back to built-in shader
					Debug.LogWarning("ConnectionLine shader not found. Using fallback shader.");
					lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
				}
			}
			else
			{
				lineRenderer.material = new Material(connectionLineShader);
				
				// Set up base material properties using WeaponNodeBorder color from ColorManager
				Color borderColor = ColorManager.Instance.GetColor("WeaponNodeBorder");
				lineRenderer.material.SetColor("_Color", borderColor);
				lineRenderer.material.SetColor("_EmissionColor", borderColor);
				lineRenderer.material.SetFloat("_EmissionIntensity", 1f);
			}
			
			// Initially hide the line
			lineRenderer.enabled = false;
		}
		
		private Vector3 CalculateLineStartPosition()
		{
			// Get the node's position (this WeaponNode) in world space
			Vector3 nodePosition = transform.position;
			
			// Get the node's size (assuming it has a RectTransform)
			RectTransform nodeRectTransform = GetComponent<RectTransform>();
			if (nodeRectTransform == null)
			{
				Debug.LogWarning("WeaponNode missing RectTransform component");
				return nodePosition;
			}
			
			// Convert the RectTransform size to world space units
			// For UI elements, we need to account for the Canvas scaling
			Canvas canvas = GetComponentInParent<Canvas>();
			float worldSpaceScaleFactor = 1f;
			if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
			{
				// For screen space canvases, we need to convert from screen to world units
				worldSpaceScaleFactor = 0.01f; // Approximate conversion factor
			}
			
			// Calculate the edge offset in world space
			float nodeWidth = nodeRectTransform.rect.width * 0.5f * worldSpaceScaleFactor;
			
			// Determine which side to start from based on the node's side
			// float xOffset = Side == RelativeSide.Left ? nodeWidth : -nodeWidth;
			
			// Start from the appropriate edge of the node
			Vector3 startPos = nodePosition;
			
			// Debug visualization
			if (DebugConnectionPoints)
			{
				Debug.DrawLine(nodePosition, startPos, Color.red, 0.5f);
				Debug.Log($"Node position: {nodePosition}, Start position: {startPos}, Node width: {nodeWidth}, Side: {Side}, Scale factor: {worldSpaceScaleFactor}");
			}
			
			return startPos;
		}
		
		private Vector3 CalculateHorizontalPoint()
		{
			// Get the start position (from the WeaponNode)
			Vector3 startPos = CalculateLineStartPosition();
			
			// Get the end position (at the WeaponNodeSelector)
			Vector3 endPos = CalculateLineEndPosition();
			
			// Get the selector's position
			Vector3 selectorPosition = WeaponNodeSelector.transform.position;
			
			// Calculate the intermediate point based on the RelativeSide
			Vector3 intermediatePoint;
			
			switch (Side)
			{
				case RelativeSide.Left:
					// For left side: Create a point that matches the selector's X coordinate
					// This creates a diagonal line to the left, then a vertical line to the selector
					intermediatePoint = new Vector3(
						selectorPosition.x + HorizontalLineDistance, // Match the selector's X coordinate
						selectorPosition.y,         // Keep the same Y as the start point
						selectorPosition.z
					);
					break;
					
				case RelativeSide.Right:
					// For right side: Create a point that matches the selector's X coordinate
					// This creates a diagonal line to the right, then a vertical line to the selector
					intermediatePoint = new Vector3(
						selectorPosition.x - HorizontalLineDistance, // Match the selector's X coordinate
						selectorPosition.y,         // Keep the same Y as the start point
						selectorPosition.z
					);
					break;
					
				case RelativeSide.Center:
					// For center: Create a point directly above/below the start point
					// This will be halfway between the start and end points
					float midY = (startPos.y + endPos.y) * 0.5f;
					intermediatePoint = new Vector3(
						startPos.x,  // Keep the same X as the start point
						midY,        // Halfway between start and end Y
						startPos.z
					);
					break;
					
				default:
					// Fallback case
					intermediatePoint = new Vector3(
						startPos.x + (Side == RelativeSide.Left ? HorizontalLineDistance : -HorizontalLineDistance),
						startPos.y,
						startPos.z
					);
					break;
			}
			
			// Debug visualization
			if (DebugConnectionPoints)
			{
				Debug.DrawLine(startPos, intermediatePoint, Color.yellow, 0.5f);
				Debug.DrawLine(intermediatePoint, endPos, Color.cyan, 0.5f);
				Debug.Log($"Intermediate point: {intermediatePoint}, Side: {Side}, Selector position: {selectorPosition}");
			}
			
			return intermediatePoint;
		}

		
		
		private Vector3 CalculateLineEndPosition()
		{
			WeaponNodeSelectorListCell activeCell = WeaponNodeSelector.List.ActiveCell;
			// Get the WeaponNodeSelector's position
			if (WeaponNodeSelector == null)
			{
				Debug.LogWarning("WeaponNodeSelector is null when calculating line end position");
				return transform.position; // Fallback to node position
			}
			
			// Use the appropriate port position based on the node's side
			if (Side == RelativeSide.Left)
			{
				// For nodes on the left side, connect to the left port of the selector
				
				return WeaponNodeSelector.GetRightPortPosition();
			}
			else if (Side == RelativeSide.Right)
			{
				// For nodes on the right side, connect to the right port of the selector
				return WeaponNodeSelector.GetLeftPortPosition();
			}
			else if (Side == RelativeSide.Center)
			{
				if (WeaponNodeSelector.transform.position.y > 0)
				{
					// Use the TopPort if the Y value is positive
					return WeaponNodeSelector.GetBottomPortPosition();
				}
				else
				{
					// Use the BottomPort if the Y value is not positive
					return WeaponNodeSelector.GetTopPortPosition();
				}
			}
			
			// Get the ActiveCell's position in world space
			Vector3 activeCellPosition = activeCell.transform.position;
			
			// Calculate the cell's dimensions based on its scale and base size
			Vector2 cellSize = EstimateCellSize(activeCell);
			
			// Convert the cell size to world space units
			Canvas canvas = activeCell.GetComponentInParent<Canvas>();
			float worldSpaceScaleFactor = 1f;
			if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
			{
				// For screen space canvases, we need to convert from screen to world units
				worldSpaceScaleFactor = 0.01f; // Approximate conversion factor
			}
			
			float cellWidth = cellSize.x * 0.5f * worldSpaceScaleFactor;
			float cellHeight = cellSize.y * 0.5f * worldSpaceScaleFactor;
			
			// Determine connection point based on the RelativeSide
			Vector3 endPos;
			
			switch (Side)
			{
				case RelativeSide.Left:
				case RelativeSide.Right:
					// For left/right sides: Connect to the left/right edge of the cell
					float xOffset = Side == RelativeSide.Left ? -cellWidth : cellWidth;
					endPos = activeCellPosition + new Vector3(xOffset, 0, 0);
					break;
					
				case RelativeSide.Center:
					// For center: Connect to the top/bottom edge of the cell
					// Determine if the node is above or below the cell
					bool isNodeAboveCell = transform.position.y > activeCellPosition.y;
					float yOffset = isNodeAboveCell ? -cellHeight : cellHeight;
					endPos = activeCellPosition + new Vector3(0, yOffset, 0);
					break;
					
				default:
					// Fallback case
					endPos = activeCellPosition;
					break;
			}
			
			// Debug visualization
			if (DebugConnectionPoints)
			{
				Debug.DrawLine(activeCellPosition, endPos, Color.green, 0.5f);
				VisualizeActiveCellBounds(activeCell, cellWidth, cellHeight);
				Debug.Log($"ActiveCell position: {activeCellPosition}, End position: {endPos}, Cell size: {cellWidth}x{cellHeight}, Side: {Side}");
			}
			
			return endPos;
		}
		
		private Vector2 EstimateCellSize(WeaponNodeSelectorListCell cell)
		{
			// Default to the inspector values if we can't find better estimates
			Vector2 size = new Vector2(CellBaseWidth * cell.transform.localScale.x, CellBaseHeight * cell.transform.localScale.y);
			
			// Try to find renderers to get a better estimate
			SpriteRenderer[] renderers = cell.GetComponentsInChildren<SpriteRenderer>();
			if (renderers.Length > 0)
			{
				// Find the bounds that encompass all renderers
				Bounds bounds = new Bounds(cell.transform.position, Vector3.zero);
				foreach (SpriteRenderer renderer in renderers)
				{
					bounds.Encapsulate(renderer.bounds);
				}
				
				// Use the bounds size for a more accurate estimate
				size.x = bounds.size.x;
				size.y = bounds.size.y;
				
				if (DebugConnectionPoints)
				{
					Debug.Log($"Estimated cell size from renderers: {size}");
				}
			}
			
			return size;
		}
		
		private void VisualizeActiveCellBounds(WeaponNodeSelectorListCell activeCell, float halfWidth, float halfHeight)
		{
			if (!DebugConnectionPoints || activeCell == null) return;
			
			Vector3 center = activeCell.transform.position;
			Vector3 topLeft = center + new Vector3(-halfWidth, halfHeight, 0);
			Vector3 topRight = center + new Vector3(halfWidth, halfHeight, 0);
			Vector3 bottomLeft = center + new Vector3(-halfWidth, -halfHeight, 0);
			Vector3 bottomRight = center + new Vector3(halfWidth, -halfHeight, 0);
			
			// Draw the rectangle with longer duration for better visibility
			Debug.DrawLine(topLeft, topRight, Color.blue, 0.5f);
			Debug.DrawLine(topRight, bottomRight, Color.blue, 0.5f);
			Debug.DrawLine(bottomRight, bottomLeft, Color.blue, 0.5f);
			Debug.DrawLine(bottomLeft, topLeft, Color.blue, 0.5f);
			
			// Draw the center point
			Debug.DrawLine(center + Vector3.up * 0.1f, center + Vector3.down * 0.1f, Color.red, 0.5f);
			Debug.DrawLine(center + Vector3.left * 0.1f, center + Vector3.right * 0.1f, Color.red, 0.5f);
			
			// Log the bounds
			Debug.Log($"Cell bounds: Center={center}, Width={halfWidth*2}, Height={halfHeight*2}");
		}
		
		private IEnumerator DrawLineCoroutine()
		{
			float elapsedTime = 0f;
			
			// Calculate the three points for our line
			Vector3 startPos = CalculateLineStartPosition();
			Vector3 endPos = CalculateLineEndPosition();
			Vector3 horizontalPoint = CalculateHorizontalPoint();
			
			// Debug the positions
			if (DebugConnectionPoints)
			{
				Debug.Log($"Drawing line from Node {NodeId} at {startPos} to horizontal point at {horizontalPoint} to ActiveCell at {endPos}");
			}
			
			// Set initial positions (all at start)
			lineRenderer.SetPosition(0, startPos);
			lineRenderer.SetPosition(1, startPos);
			lineRenderer.SetPosition(2, startPos);
			
			while (elapsedTime < DrawDuration)
			{
				elapsedTime += Time.deltaTime;
				float t = Mathf.Clamp01(elapsedTime / DrawDuration);
				
				// First segment: from start to horizontal point
				lineRenderer.SetPosition(0, startPos);
				
				if (t <= 0.5f)
				{
					// First half of the animation: draw from start to horizontal point
					float segmentT = t * 2f; // Scale t to [0,1] for this segment
					lineRenderer.SetPosition(1, Vector3.Lerp(startPos, horizontalPoint, segmentT));
					lineRenderer.SetPosition(2, Vector3.Lerp(startPos, horizontalPoint, segmentT));
				}
				else
				{
					// Second half of the animation: draw from horizontal point to end
					float segmentT = (t - 0.5f) * 2f; // Scale t to [0,1] for this segment
					lineRenderer.SetPosition(1, horizontalPoint);
					lineRenderer.SetPosition(2, Vector3.Lerp(horizontalPoint, endPos, segmentT));
				}
				
				yield return null;
			}
			
			// Ensure the line is fully drawn
			lineRenderer.SetPosition(0, startPos);
			lineRenderer.SetPosition(1, horizontalPoint);
			lineRenderer.SetPosition(2, endPos);
			
			// Start the connection effect
			activeLineCoroutine = StartCoroutine(ConnectionEffect());
		}
		
		private IEnumerator ConnectionEffect()
		{
			// Wait for the line to be fully drawn
			// yield return new WaitForSeconds(DrawDuration);
			
			if (isLineActive)
			{
				// Switch from WeaponNodeBorder to WeaponNodeBorderHover
				Color borderColor = ColorManager.Instance.GetColor("WeaponNodeBorder");
				Color borderHoverColor = ColorManager.Instance.GetColor("WeaponNodeBorderHover");
				
				// Set initial emission intensity values
				float initialIntensity = 0.5f;  // Starting intensity
				float peakIntensity = 1.25f;    // Peak intensity to ramp up to
				float finalIntensity = 0.25f;   // Final intensity to settle at
				
				// Duration settings
				float rampUpDuration = 0.2f;    // Quick ramp up time
				float fadeDownDuration = 0.75f; // Existing fade down time
				
				// Apply the hover color
				if (lineRenderer.material.HasProperty("_Color"))
				{
					lineRenderer.material.SetColor("_Color", borderHoverColor);
				}
				
				if (lineRenderer.material.HasProperty("_EmissionColor"))
				{
					lineRenderer.material.SetColor("_EmissionColor", borderHoverColor);
				}
				
				// Set the initial intensity
				if (lineRenderer.material.HasProperty("_EmissionIntensity"))
				{
					lineRenderer.material.SetFloat("_EmissionIntensity", initialIntensity);
				}
				
				// First phase: Quickly ramp up to peak intensity
				float elapsedTime = 0f;
				while (elapsedTime < rampUpDuration && isLineActive)
				{
					elapsedTime += Time.deltaTime;
					float t = Mathf.Clamp01(elapsedTime / rampUpDuration);
					
					// Lerp from initialIntensity to peakIntensity
					float currentIntensity = Mathf.Lerp(initialIntensity, peakIntensity, t);
					
					if (lineRenderer.material.HasProperty("_EmissionIntensity"))
					{
						lineRenderer.material.SetFloat("_EmissionIntensity", currentIntensity);
					}
					
					yield return null;
				}
				
				// Ensure we set the peak intensity value
				if (isLineActive && lineRenderer.material.HasProperty("_EmissionIntensity"))
				{
					lineRenderer.material.SetFloat("_EmissionIntensity", peakIntensity);
				}
				
				// Second phase: Gradually reduce from peak to final intensity
				elapsedTime = 0f;
				while (elapsedTime < fadeDownDuration && isLineActive)
				{
					elapsedTime += Time.deltaTime;
					float t = Mathf.Clamp01(elapsedTime / fadeDownDuration);
					
					// Lerp from peakIntensity to finalIntensity
					float currentIntensity = Mathf.Lerp(peakIntensity, finalIntensity, t);
					
					if (lineRenderer.material.HasProperty("_EmissionIntensity"))
					{
						lineRenderer.material.SetFloat("_EmissionIntensity", currentIntensity);
					}
					
					yield return null;
				}
				
				// Ensure we set the final intensity value
				if (isLineActive && lineRenderer.material.HasProperty("_EmissionIntensity"))
				{
					lineRenderer.material.SetFloat("_EmissionIntensity", finalIntensity);
				}
			}
		}
		
		private IEnumerator UpdateLinePositions()
		{
			while (isLineActive && lineRenderer != null)
			{
				// Calculate the three points for our line
				Vector3 startPos = CalculateLineStartPosition();
				Vector3 endPos = CalculateLineEndPosition();
				Vector3 horizontalPoint = CalculateHorizontalPoint();
				
				// Update line positions
				lineRenderer.SetPosition(0, startPos);
				lineRenderer.SetPosition(1, horizontalPoint);
				lineRenderer.SetPosition(2, endPos);
				
				yield return null;
			}
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
			ActivateLine();
		}

		public void DisableHover()
		{
			if (!Initialised) return;
			if (State != NodeState.Hover || State == NodeState.Selected) return;
			
			// No longer deactivate the line - we want it to remain visible permanently
			// DeactivateLine();
			
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
				ActivateLine();
				return;
			}
			
			WeaponNodeSelector.HandleSelect();
			AssignSelector(WeaponNodeSelector);
			SetState(NodeState.Hover);
			foreach (WeaponNode weaponNode in LinkedWeaponNodes) {
				weaponNode.RefreshSelector();
			}
			
			// Activate the line when selected
			ActivateLine();
		}

		public void HandleDeselect()
		{
			if (!Initialised) return;
			if (State != NodeState.Selected) return;
			
			SetState(NodeState.Hover);
			WeaponNodeSelector.HandleDeselect();
			
			// Keep the line active when returning to hover state
			ActivateLine();
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

		private void ActivateLine()
		{
			// Check if lineRenderer exists
			if (lineRenderer == null)
			{
				return;
			}
			
			// If the line is already active, don't restart the animation
			if (isLineActive && activeLineCoroutine != null)
			{
				return;
			}
			
			// Stop any existing coroutine
			if (activeLineCoroutine != null)
			{
				StopCoroutine(activeLineCoroutine);
				activeLineCoroutine = null;
			}
			
			// Reset the line state
			isLineActive = true;
			lineRenderer.enabled = true;
			
			// Start the drawing coroutine
			activeLineCoroutine = StartCoroutine(DrawLineCoroutine());
		}
		
		private void DeactivateLine()
		{
			if (isLineActive && lineRenderer != null)
			{
				isLineActive = false;
				
				// Stop any existing coroutine
				if (activeLineCoroutine != null)
				{
					StopCoroutine(activeLineCoroutine);
					activeLineCoroutine = null;
				}
				
				// Immediately hide the line instead of fading it out
				// This ensures it's ready to be drawn again immediately
				lineRenderer.enabled = false;
				
				// Reset the line renderer to its initial state
				Vector3 startPos = transform.position;
				lineRenderer.SetPosition(0, startPos);
				lineRenderer.SetPosition(1, startPos);
				lineRenderer.SetPosition(2, startPos);
				
				// Reset colors
				lineRenderer.startColor = LineDefaultColor;
				lineRenderer.endColor = LineDefaultColor;
			}
		}
	}
}