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
    }
} 