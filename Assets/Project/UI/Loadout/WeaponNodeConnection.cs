using System.Collections;
using Project.Ships;
using UnityEngine;
using Crux.Utilities;
using System.Collections.Generic;

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
        
        // Track if the line is in a shortened state
        private bool isShortened = false;
        private Vector3 shortenedEndPoint;
        private float shortenAmount = 0f;
        
        // Track branching lines
        private List<GameObject> branchingLines = new List<GameObject>();
        
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
            
            // If the line is shortened, adjust the end position
            if (isShortened && lineRenderer != null)
            {
                // Calculate the direction from end to middle
                Vector3 direction = (middlePos - endPos).normalized;
                
                // Apply the shortening
                endPos = endPos + (direction * shortenAmount);
                shortenedEndPoint = endPos;
            }
            
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
            
            // Calculate the adjusted horizontal distance based on selector's X position
            float adjustedHorizontalDistance = CalculateAdjustedHorizontalDistance(selectorPosition.x);
            
            // Calculate the intermediate point based on the RelativeSide
            Vector3 intermediatePoint;
            
            switch (side)
            {
                case RelativeSide.Left:
                    // For left side: Create a point that matches the selector's X coordinate
                    intermediatePoint = new Vector3(
                        selectorPosition.x + adjustedHorizontalDistance,
                        selectorPosition.y,
                        selectorPosition.z
                    );
                    break;
                    
                case RelativeSide.Right:
                    // For right side: Create a point that matches the selector's X coordinate
                    intermediatePoint = new Vector3(
                        selectorPosition.x - adjustedHorizontalDistance,
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
                        startPos.x + (side == RelativeSide.Left ? adjustedHorizontalDistance : -adjustedHorizontalDistance),
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
                Debug.Log($"Intermediate point: {intermediatePoint}, Side: {side}, Adjusted Distance: {adjustedHorizontalDistance}");
            }
            
            return intermediatePoint;
        }
        
        /// <summary>
        /// Calculates an adjusted horizontal distance based on the selector's X position.
        /// The further from X:0, the more the distance is shortened (max 50% reduction).
        /// </summary>
        private float CalculateAdjustedHorizontalDistance(float selectorX)
        {
            // Define the threshold at which we start applying the reduction
            float thresholdX = 2.0f;
            
            // Define the X value at which we reach maximum reduction (50%)
            float maxReductionX = 8.0f;
            
            // Calculate the absolute X position
            float absX = Mathf.Abs(selectorX);
            
            // If below threshold, use the full distance
            if (absX <= thresholdX)
                return HorizontalLineDistance;
                
            // If beyond max reduction point, use 50% of the distance
            if (absX >= maxReductionX)
                return HorizontalLineDistance * 0.5f;
                
            // Otherwise, linearly interpolate between full and half distance
            float t = (absX - thresholdX) / (maxReductionX - thresholdX);
            return Mathf.Lerp(HorizontalLineDistance, HorizontalLineDistance * 0.5f, t);
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
        
        /// <summary>
        /// Shortens the connection line by moving the end point towards the middle point.
        /// Returns a coroutine that can be awaited to know when the animation is complete.
        /// </summary>
        /// <param name="shortenAmount">Amount to shorten the line by</param>
        /// <param name="duration">Duration of the shortening animation</param>
        /// <returns>Coroutine that can be awaited</returns>
        public IEnumerator ShortenConnectionLine(float shortenAmount, float duration)
        {
            // Stop continuous updates temporarily
            if (activeLineCoroutine != null)
            {
                StopCoroutine(activeLineCoroutine);
                activeLineCoroutine = null;
            }
            
            // Store the shorten amount for future updates
            this.shortenAmount = shortenAmount;
            this.isShortened = true;
            
            // Call the base class method to shorten the end point
            yield return StartCoroutine(ShortenEndPoint(shortenAmount, duration));
            
            // Resume continuous updates after shortening is complete
            activeLineCoroutine = StartCoroutine(ContinuousPositionUpdates());
        }
        
        /// <summary>
        /// Creates branching lines from the shortened end point based on the node's side.
        /// </summary>
        /// <param name="branchLength">Length of the first branch</param>
        /// <param name="secondaryBranchLength">Length of the secondary branch</param>
        /// <param name="firstBranchDuration">Duration of the first branch drawing animation</param>
        /// <param name="secondaryBranchDuration">Duration of the secondary branch drawing animation</param>
        /// <returns>Coroutine that can be awaited</returns>
        public IEnumerator CreateBranchingLines(float branchLength, float secondaryBranchLength, float firstBranchDuration, float secondaryBranchDuration)
        {
            if (!isShortened || lineRenderer == null)
            {
                yield break;
            }
            
            // Clear any existing branching lines
            ClearBranchingLines();
            
            // Get the shortened end point
            Vector3 branchStartPoint = linePositions[2];
            
            // Determine the directions for the branches based on the node's side
            Vector3 firstBranchDir1, firstBranchDir2;
            Vector3 secondaryBranchDir1, secondaryBranchDir2;
            
            if (side == RelativeSide.Left || side == RelativeSide.Right)
            {
                // For Left or Right sides, branches go up and down
                firstBranchDir1 = Vector3.up;
                firstBranchDir2 = Vector3.down;
                
                // Secondary branches direction depends on the side
                if (side == RelativeSide.Left)
                {
                    // For Left side, secondary branches go left (negative X)
                    secondaryBranchDir1 = Vector3.left;
                    secondaryBranchDir2 = Vector3.left;
                }
                else
                {
                    // For Right side, secondary branches go right (positive X)
                    secondaryBranchDir1 = Vector3.right;
                    secondaryBranchDir2 = Vector3.right;
                }
            }
            else // Center
            {
                // For Center, branches go left and right
                firstBranchDir1 = Vector3.left;
                firstBranchDir2 = Vector3.right;
                
                // Secondary branches direction depends on the Y position of the selector
                if (selectorTransform.position.y > 0)
                {
                    // If Y is positive, secondary branches go up
                    secondaryBranchDir1 = Vector3.up;
                    secondaryBranchDir2 = Vector3.up;
                }
                else
                {
                    // If Y is negative or zero, secondary branches go down
                    secondaryBranchDir1 = Vector3.down;
                    secondaryBranchDir2 = Vector3.down;
                }
            }
            
            // Create the first branch (up or left)
            Vector3 branch1EndPoint = CreateBranchingLine(
                branchStartPoint, 
                firstBranchDir1, 
                branchLength, 
                firstBranchDuration, 
                $"Branch1_{nodeId}");
            
            // Create the second branch (down or right)
            Vector3 branch2EndPoint = CreateBranchingLine(
                branchStartPoint, 
                firstBranchDir2, 
                branchLength, 
                firstBranchDuration, 
                $"Branch2_{nodeId}");
            
            // Wait for the first branch animation to complete fully
            yield return new WaitForSeconds(firstBranchDuration);
            
            // Create the first secondary branch
            CreateSecondaryBranch(
                branch1EndPoint, 
                secondaryBranchDir1, 
                secondaryBranchLength, 
                secondaryBranchDuration, 
                $"SecBranch1_{nodeId}");
            
            // Create the second secondary branch
            CreateSecondaryBranch(
                branch2EndPoint, 
                secondaryBranchDir2, 
                secondaryBranchLength, 
                secondaryBranchDuration, 
                $"SecBranch2_{nodeId}");
            
            // Wait for the secondary branch animations to complete
            yield return new WaitForSeconds(secondaryBranchDuration);
        }
        
        /// <summary>
        /// Clears all branching lines
        /// </summary>
        private void ClearBranchingLines()
        {
            // Find all child objects with names containing "Branch"
            foreach (Transform child in transform)
            {
                if (child.name.Contains("Branch"))
                {
                    Destroy(child.gameObject);
                }
            }
            
            branchingLines.Clear();
        }
        
        /// <summary>
        /// Animates the shortening of branching lines before removing them
        /// </summary>
        /// <param name="primaryDuration">Duration of the primary branch shortening animation</param>
        /// <param name="secondaryDuration">Duration of the secondary branch shortening animation</param>
        /// <returns>Coroutine that can be awaited</returns>
        public IEnumerator ShortenBranchingLines(float primaryDuration, float secondaryDuration)
        {
            // Find all secondary branch objects first
            List<LineRenderer> secondaryBranchRenderers = new List<LineRenderer>();
            Dictionary<LineRenderer, Vector3> secondaryStartPositions = new Dictionary<LineRenderer, Vector3>();
            Dictionary<LineRenderer, Vector3> secondaryEndPositions = new Dictionary<LineRenderer, Vector3>();
            
            // Find all primary branch objects
            List<LineRenderer> primaryBranchRenderers = new List<LineRenderer>();
            Dictionary<LineRenderer, Vector3> primaryStartPositions = new Dictionary<LineRenderer, Vector3>();
            Dictionary<LineRenderer, Vector3> primaryEndPositions = new Dictionary<LineRenderer, Vector3>();
            
            foreach (Transform child in transform)
            {
                if (child.name.Contains("Secondary"))
                {
                    // This is a secondary branch
                    LineRenderer branchRenderer = child.GetComponent<LineRenderer>();
                    if (branchRenderer != null)
                    {
                        secondaryBranchRenderers.Add(branchRenderer);
                        
                        // Store the start and end positions
                        secondaryStartPositions[branchRenderer] = branchRenderer.GetPosition(0);
                        secondaryEndPositions[branchRenderer] = branchRenderer.GetPosition(1);
                    }
                }
                else if (child.name.Contains("Branch"))
                {
                    // This is a primary branch
                    LineRenderer branchRenderer = child.GetComponent<LineRenderer>();
                    if (branchRenderer != null)
                    {
                        primaryBranchRenderers.Add(branchRenderer);
                        
                        // Store the start and end positions
                        primaryStartPositions[branchRenderer] = branchRenderer.GetPosition(0);
                        primaryEndPositions[branchRenderer] = branchRenderer.GetPosition(1);
                    }
                }
            }
            
            // Step 1: Animate the shortening of secondary branching lines
            if (secondaryBranchRenderers.Count > 0)
            {
                float elapsedTime = 0f;
                while (elapsedTime < secondaryDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / secondaryDuration);
                    
                    // For each secondary branch, shorten it by moving the end point towards the start point
                    foreach (LineRenderer branchRenderer in secondaryBranchRenderers)
                    {
                        if (branchRenderer != null)
                        {
                            Vector3 startPos = secondaryStartPositions[branchRenderer];
                            Vector3 endPos = secondaryEndPositions[branchRenderer];
                            
                            // Lerp from end position to start position
                            Vector3 newEndPos = Vector3.Lerp(endPos, startPos, t);
                            
                            // Update the line renderer
                            branchRenderer.SetPosition(1, newEndPos);
                        }
                    }
                    
                    yield return null;
                }
                
                // After animation is complete, destroy all secondary branch objects
                foreach (LineRenderer branchRenderer in secondaryBranchRenderers)
                {
                    if (branchRenderer != null)
                    {
                        Destroy(branchRenderer.gameObject);
                    }
                }
            }
            
            // Step 2: Animate the shortening of primary branching lines
            if (primaryBranchRenderers.Count > 0)
            {
                float elapsedTime = 0f;
                while (elapsedTime < primaryDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / primaryDuration);
                    
                    // For each primary branch, shorten it by moving the end point towards the start point
                    foreach (LineRenderer branchRenderer in primaryBranchRenderers)
                    {
                        if (branchRenderer != null)
                        {
                            Vector3 startPos = primaryStartPositions[branchRenderer];
                            Vector3 endPos = primaryEndPositions[branchRenderer];
                            
                            // Lerp from end position to start position
                            Vector3 newEndPos = Vector3.Lerp(endPos, startPos, t);
                            
                            // Update the line renderer
                            branchRenderer.SetPosition(1, newEndPos);
                        }
                    }
                    
                    yield return null;
                }
                
                // After animation is complete, destroy all primary branch objects
                foreach (LineRenderer branchRenderer in primaryBranchRenderers)
                {
                    if (branchRenderer != null)
                    {
                        Destroy(branchRenderer.gameObject);
                    }
                }
            }
            
            // Clear the list of branching lines
            branchingLines.Clear();
        }
        
        /// <summary>
        /// Restores the connection line to its original length
        /// </summary>
        /// <param name="duration">Duration of the restoration animation</param>
        /// <returns>Coroutine that can be awaited</returns>
        public IEnumerator RestoreConnectionLine(float duration)
        {
            // Don't clear branching lines immediately anymore, we'll animate them first
            
            if (!isShortened)
            {
                yield break;
            }
            
            // Stop continuous updates temporarily
            if (activeLineCoroutine != null)
            {
                StopCoroutine(activeLineCoroutine);
                activeLineCoroutine = null;
            }
            
            // Get the current positions
            Vector3 startPos = linePositions[0];
            Vector3 middlePos = linePositions[1];
            Vector3 shortenedEndPos = linePositions[2];
            
            // Calculate the original end position
            Vector3 originalEndPos = CalculateEndPosition();
            
            // Animate the restoration
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                
                // Lerp from shortened end position to original end position
                Vector3 newEndPos = Vector3.Lerp(shortenedEndPos, originalEndPos, t);
                
                // Update the line renderer
                lineRenderer.SetPosition(2, newEndPos);
                
                yield return null;
            }
            
            // Ensure we set the final position
            lineRenderer.SetPosition(2, originalEndPos);
            
            // Update the stored position
            linePositions[2] = originalEndPos;
            
            // Reset the shortened state
            isShortened = false;
            shortenAmount = 0f;
            
            // Resume continuous updates
            activeLineCoroutine = StartCoroutine(ContinuousPositionUpdates());
        }
    }
} 