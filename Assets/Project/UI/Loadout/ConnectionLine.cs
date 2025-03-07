using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crux.Utilities;

namespace Project.UI.Loadout
{
    /// <summary>
    /// A generic, reusable component for creating and managing connection lines.
    /// This base class handles the core line rendering functionality without any specific positioning logic.
    /// </summary>
    public class ConnectionLine : MonoBehaviour
    {
        [Header("Line Settings")]
        [Tooltip("Width of the connection line")]
        protected float LineWidth = 0.035f;
        
        [Tooltip("Duration of the line drawing animation")]
        public float DrawDuration = 1f;
        
        [Tooltip("Enable to visualize connection points for debugging")]
        public bool DebugConnectionPoints = false;

        // Line renderer components
        protected GameObject lineObject;
        protected LineRenderer lineRenderer;
        protected bool isLineActive = false;
        protected Coroutine activeLineCoroutine;
        
        // Line color settings
        protected Color lineDefaultColor;
        
        // Line positions
        protected Vector3[] linePositions = new Vector3[3]; // Start, middle, end
        
        /// <summary>
        /// Initialize the connection line with basic settings
        /// </summary>
        public virtual void Initialize(string lineId = "ConnectionLine")
        {
            // Initialize the default color from ColorManager
            lineDefaultColor = ColorManager.Instance.GetColor("WeaponNodeBorder");
            
            // Create the line renderer
            CreateConnectionLine(lineId);
        }
        
        /// <summary>
        /// Set the positions for the line
        /// </summary>
        public virtual void SetLinePositions(Vector3 start, Vector3 middle, Vector3 end)
        {
            linePositions[0] = start;
            linePositions[1] = middle;
            linePositions[2] = end;
            
            // If the line is active, update its positions immediately
            if (isLineActive && lineRenderer != null)
            {
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, middle);
                lineRenderer.SetPosition(2, end);
            }
        }
        
        /// <summary>
        /// Creates the LineRenderer component and configures its material
        /// </summary>
        protected virtual void CreateConnectionLine(string lineId)
        {
            // Create a new GameObject for the line
            lineObject = new GameObject(lineId);
            lineObject.transform.SetParent(transform);
            
            // Add the LineRenderer component
            lineRenderer = lineObject.AddComponent<LineRenderer>();
            
            // Set the line width using the LineWidth property
            lineRenderer.startWidth = LineWidth;
            lineRenderer.endWidth = LineWidth;
            
            // Ensure the LineRenderer uses world space positions
            lineRenderer.useWorldSpace = true;
            
            // Set the sorting layer to UI
            lineRenderer.sortingLayerName = "UI";
            
            // Set position count to 3 for the two-segment line
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
        
        /// <summary>
        /// Activate the line and start the drawing animation
        /// </summary>
        public virtual void ActivateLine()
        {
            // Check if lineRenderer exists
            if (lineRenderer == null)
            {
                Debug.LogWarning("LineRenderer is null when trying to activate line");
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
        
        /// <summary>
        /// Deactivate the line and hide it
        /// </summary>
        public virtual void DeactivateLine()
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
                
                // Immediately hide the line
                lineRenderer.enabled = false;
                
                // Reset the line renderer to its initial state
                Vector3 startPos = linePositions[0];
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, startPos);
                lineRenderer.SetPosition(2, startPos);
                
                // Reset colors
                lineRenderer.startColor = lineDefaultColor;
                lineRenderer.endColor = lineDefaultColor;
            }
        }
        
        /// <summary>
        /// Coroutine that animates the drawing of the line
        /// </summary>
        protected virtual IEnumerator DrawLineCoroutine()
        {
            float elapsedTime = 0f;
            
            // Get the three points for our line
            Vector3 startPos = linePositions[0];
            Vector3 middlePos = linePositions[1];
            Vector3 endPos = linePositions[2];
            
            // Debug the positions
            if (DebugConnectionPoints)
            {
                Debug.Log($"Drawing line from {startPos} to middle point at {middlePos} to end at {endPos}");
            }
            
            // Set initial positions (all at start)
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, startPos);
            lineRenderer.SetPosition(2, startPos);
            
            while (elapsedTime < DrawDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / DrawDuration);
                
                // First segment: from start to middle point
                lineRenderer.SetPosition(0, startPos);
                
                if (t <= 0.5f)
                {
                    // First half of the animation: draw from start to middle point
                    float segmentT = t * 2f; // Scale t to [0,1] for this segment
                    lineRenderer.SetPosition(1, Vector3.Lerp(startPos, middlePos, segmentT));
                    lineRenderer.SetPosition(2, Vector3.Lerp(startPos, middlePos, segmentT));
                }
                else
                {
                    // Second half of the animation: draw from middle point to end
                    float segmentT = (t - 0.5f) * 2f; // Scale t to [0,1] for this segment
                    lineRenderer.SetPosition(1, middlePos);
                    lineRenderer.SetPosition(2, Vector3.Lerp(middlePos, endPos, segmentT));
                }
                
                yield return null;
            }
            
            // Ensure the line is fully drawn
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, middlePos);
            lineRenderer.SetPosition(2, endPos);
            
            // Start the connection effect
            activeLineCoroutine = StartCoroutine(OnLineDrawComplete());
        }
        
        /// <summary>
        /// Coroutine that creates a visual effect on the line after it's drawn.
        /// Override this in derived classes to provide custom connection effects.
        /// </summary>
        protected virtual IEnumerator OnLineDrawComplete()
        {
            // This is an empty placeholder method that derived classes can override
            // to provide custom connection effects after the line is drawn.
            // By default, it does nothing.
            yield return null;
        }
        
        /// <summary>
        /// Set the color of the line
        /// </summary>
        public virtual void SetLineColor(Color color)
        {
            if (lineRenderer != null)
            {
                if (lineRenderer.material.HasProperty("_Color"))
                {
                    lineRenderer.material.SetColor("_Color", color);
                }
                
                if (lineRenderer.material.HasProperty("_EmissionColor"))
                {
                    lineRenderer.material.SetColor("_EmissionColor", color);
                }
            }
        }
        
        /// <summary>
        /// Set the emission intensity of the line
        /// </summary>
        public virtual void SetEmissionIntensity(float intensity)
        {
            if (lineRenderer != null && lineRenderer.material.HasProperty("_EmissionIntensity"))
            {
                lineRenderer.material.SetFloat("_EmissionIntensity", intensity);
            }
        }
        
        /// <summary>
        /// Clean up when the component is destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (lineObject != null)
            {
                Destroy(lineObject);
            }
        }
        
        /// <summary>
        /// Set the width of the line
        /// </summary>
        public virtual void SetLineWidth(float width)
        {
            LineWidth = width;
            
            // Update the line renderer if it exists
            if (lineRenderer != null)
            {
                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;
            }
        }
        
        /// <summary>
        /// Shortens the end point of the line by moving it towards the middle point by the specified amount.
        /// Returns a coroutine that can be awaited to know when the animation is complete.
        /// </summary>
        /// <param name="shortenAmount">Amount to shorten the line by</param>
        /// <param name="duration">Duration of the shortening animation</param>
        /// <returns>Coroutine that can be awaited</returns>
        public virtual IEnumerator ShortenEndPoint(float shortenAmount, float duration)
        {
            if (lineRenderer == null || !isLineActive)
            {
                yield break;
            }
            
            // Get the current positions
            Vector3 startPos = linePositions[0];
            Vector3 middlePos = linePositions[1];
            Vector3 endPos = linePositions[2];
            
            // Calculate the direction from end to middle
            Vector3 direction = (middlePos - endPos).normalized;
            
            // Calculate the target position (moved towards middle by shortenAmount)
            Vector3 targetEndPos = endPos + (direction * shortenAmount);
            
            // Animate the shortening
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                
                // Lerp from current end position to target end position
                Vector3 newEndPos = Vector3.Lerp(endPos, targetEndPos, t);
                
                // Update the line renderer
                lineRenderer.SetPosition(2, newEndPos);
                
                yield return null;
            }
            
            // Ensure we set the final position
            lineRenderer.SetPosition(2, targetEndPos);
            
            // Update the stored position
            linePositions[2] = targetEndPos;
        }
        
        /// <summary>
        /// Creates a new line that branches from a specified point in a specified direction.
        /// </summary>
        /// <param name="startPoint">The starting point of the branch</param>
        /// <param name="direction">The direction the branch should extend</param>
        /// <param name="length">The length of the branch</param>
        /// <param name="duration">Duration of the drawing animation</param>
        /// <param name="lineId">Identifier for the new line</param>
        /// <returns>The end point of the branch</returns>
        public virtual Vector3 CreateBranchingLine(Vector3 startPoint, Vector3 direction, float length, float duration, string lineId)
        {
            // Create a new GameObject for the branching line
            GameObject branchLineObject = new GameObject($"{lineId}_Branch");
            branchLineObject.transform.SetParent(transform);
            
            // Add the LineRenderer component
            LineRenderer branchLineRenderer = branchLineObject.AddComponent<LineRenderer>();
            
            // Set the line width using the LineWidth property
            branchLineRenderer.startWidth = LineWidth;
            branchLineRenderer.endWidth = LineWidth;
            
            // Ensure the LineRenderer uses world space positions
            branchLineRenderer.useWorldSpace = true;
            
            // Set the sorting layer to UI
            branchLineRenderer.sortingLayerName = "UI";
            
            // Set position count to 2 for a straight line
            branchLineRenderer.positionCount = 2;
            
            // Use the same material as the main line
            if (lineRenderer != null && lineRenderer.material != null)
            {
                branchLineRenderer.material = new Material(lineRenderer.material);
            }
            else
            {
                // Fallback to a default material
                branchLineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            }
            
            // Calculate the end point
            Vector3 endPoint = startPoint + (direction.normalized * length);
            
            // Set initial positions (both at start)
            branchLineRenderer.SetPosition(0, startPoint);
            branchLineRenderer.SetPosition(1, startPoint);
            
            // Enable the line renderer
            branchLineRenderer.enabled = true;
            
            // Start the animation coroutine
            StartCoroutine(AnimateBranchingLine(branchLineRenderer, startPoint, endPoint, duration));
            
            // Return the end point immediately
            return endPoint;
        }
        
        /// <summary>
        /// Animates the drawing of a branching line.
        /// </summary>
        private IEnumerator AnimateBranchingLine(LineRenderer branchLineRenderer, Vector3 startPoint, Vector3 endPoint, float duration)
        {
            // Animate the drawing of the line
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                
                // Lerp from start to end
                Vector3 currentEndPoint = Vector3.Lerp(startPoint, endPoint, t);
                branchLineRenderer.SetPosition(1, currentEndPoint);
                
                yield return null;
            }
            
            // Ensure the line is fully drawn
            branchLineRenderer.SetPosition(1, endPoint);
        }
        
        /// <summary>
        /// Creates a secondary branch from the end of an existing branch.
        /// </summary>
        /// <param name="startPoint">The starting point (end of previous branch)</param>
        /// <param name="direction">The direction the secondary branch should extend</param>
        /// <param name="length">The length of the secondary branch</param>
        /// <param name="duration">Duration of the drawing animation</param>
        /// <param name="lineId">Identifier for the new line</param>
        /// <returns>The end point of the secondary branch</returns>
        public virtual Vector3 CreateSecondaryBranch(Vector3 startPoint, Vector3 direction, float length, float duration, string lineId)
        {
            // This is essentially the same as CreateBranchingLine but with a different naming convention
            return CreateBranchingLine(startPoint, direction, length, duration, $"{lineId}_Secondary");
        }
    }
} 