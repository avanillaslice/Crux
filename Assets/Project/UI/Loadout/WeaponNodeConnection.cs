using System.Collections;
using Project.Ships;
using UnityEngine;
using Crux.Utilities;

namespace Project.UI.Loadout
{
    /// <summary>
    /// Specialized connection line for weapon nodes that handles the specific
    /// positioning and visual effects needed for weapon node connections.
    /// </summary>
    public class WeaponNodeConnection : ConnectionLine
    {
        [Header("Weapon Node Connection Settings")]
        [Tooltip("Distance in world units for the horizontal segment of the connection line")]
        public float HorizontalLineDistance = 2f;
        
        [Tooltip("Width of the weapon node connection line")]
        public float WeaponNodeLineWidth = 0.035f;
        
        // Connection points
        private Transform nodeTransform;
        private Transform selectorTransform;
        private RelativeSide side;
        private int nodeId;
        private WeaponNodeSelector selector;
        
        /// <summary>
        /// Initialize the weapon node connection with the necessary transforms and side information
        /// </summary>
        public void InitializeForWeaponNode(
            Transform node, 
            WeaponNodeSelector weaponSelector, 
            RelativeSide nodeSide, 
            int id)
        {
            nodeTransform = node;
            selectorTransform = weaponSelector.transform;
            selector = weaponSelector;
            side = nodeSide;
            nodeId = id;
            
            // Call the base initialization
            base.Initialize($"WeaponNodeConnection_{id}");
            
            // Set the line width
            SetLineWidth(WeaponNodeLineWidth);
            
            // Calculate and set the initial line positions
            UpdateLinePositions();
        }
        
        /// <summary>
        /// Update the line positions based on the current state of the node and selector
        /// </summary>
        public void UpdateLinePositions()
        {
            Vector3 startPos = CalculateStartPosition();
            Vector3 middlePos = CalculateMiddlePosition(startPos);
            Vector3 endPos = CalculateEndPosition();
            
            // Set the three positions for our weapon node connection
            SetLinePositions(startPos, middlePos, endPos);
        }
        
        /// <summary>
        /// Calculate the start position of the line (at the weapon node)
        /// </summary>
        private Vector3 CalculateStartPosition()
        {
            // Simply use the node's position
            return nodeTransform.position;
        }
        
        /// <summary>
        /// Calculate the middle position for the line based on the node's side
        /// </summary>
        private Vector3 CalculateMiddlePosition(Vector3 startPos)
        {
            // Get the end position
            Vector3 endPos = CalculateEndPosition();
            
            // Get the selector's position
            Vector3 selectorPosition = selectorTransform.position;
            
            // Calculate the intermediate point based on the RelativeSide
            Vector3 intermediatePoint;
            
            switch (side)
            {
                case RelativeSide.Left:
                    // For left side: Create a point that matches the selector's X coordinate
                    intermediatePoint = new Vector3(
                        selectorPosition.x + HorizontalLineDistance,
                        selectorPosition.y,
                        selectorPosition.z
                    );
                    break;
                    
                case RelativeSide.Right:
                    // For right side: Create a point that matches the selector's X coordinate
                    intermediatePoint = new Vector3(
                        selectorPosition.x - HorizontalLineDistance,
                        selectorPosition.y,
                        selectorPosition.z
                    );
                    break;
                    
                case RelativeSide.Center:
                    // For center: Create a point directly above/below the start point
                    float midY = (startPos.y + endPos.y) * 0.5f;
                    intermediatePoint = new Vector3(
                        startPos.x,
                        midY,
                        startPos.z
                    );
                    break;
                    
                default:
                    // Fallback case
                    intermediatePoint = new Vector3(
                        startPos.x + (side == RelativeSide.Left ? HorizontalLineDistance : -HorizontalLineDistance),
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
                Debug.Log($"Intermediate point: {intermediatePoint}, Side: {side}");
            }
            
            return intermediatePoint;
        }
        
        /// <summary>
        /// Calculate the end position of the line (at the weapon node selector)
        /// </summary>
        private Vector3 CalculateEndPosition()
        {
            if (selector == null)
            {
                Debug.LogWarning("WeaponNodeSelector is null when calculating line end position");
                return nodeTransform.position; // Fallback to node position
            }
            
            // Use the appropriate port position based on the node's side
            if (side == RelativeSide.Left)
            {
                return selector.GetRightPortPosition();
            }
            else if (side == RelativeSide.Right)
            {
                return selector.GetLeftPortPosition();
            }
            else if (side == RelativeSide.Center)
            {
                if (selector.transform.position.y > 0)
                {
                    // Use the TopPort if the Y value is positive
                    return selector.GetBottomPortPosition();
                }
                else
                {
                    // Use the BottomPort if the Y value is not positive
                    return selector.GetTopPortPosition();
                }
            }
            
            // Fallback to the selector's position
            return selector.transform.position;
        }
        
        /// <summary>
        /// Override to implement the weapon node specific connection effect
        /// </summary>
        protected override IEnumerator OnLineDrawComplete()
        {
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
                
                // Start continuous position updates
                activeLineCoroutine = StartCoroutine(ContinuousPositionUpdates());
            }
        }
        
        /// <summary>
        /// Continuously update the line positions to follow the node and selector
        /// </summary>
        private IEnumerator ContinuousPositionUpdates()
        {
            while (isLineActive && lineRenderer != null)
            {
                UpdateLinePositions();
                yield return null;
            }
        }
    }
} 