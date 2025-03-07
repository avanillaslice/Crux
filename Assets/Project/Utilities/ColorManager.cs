using System.Collections.Generic;
using UnityEngine;

namespace Crux.Utilities
{
    /// <summary>
    /// Manages color schemes throughout the game.
    /// Access colors via ColorManager.Instance.GetColor("colorName")
    /// </summary>
    public class ColorManager : MonoBehaviour
    {
        public static ColorManager Instance { get; private set; }

        [System.Serializable]
        public class ColorScheme
        {
            public string schemeName;
            public List<ColorEntry> colors = new List<ColorEntry>();
        }

        [System.Serializable]
        public class ColorEntry
        {
            public string colorName;
            public Color color;
        }

        [Tooltip("Define your color schemes here")]
        public List<ColorScheme> colorSchemes = new List<ColorScheme>();

        // Default scheme to use if none is specified
        [Tooltip("The default color scheme to use")]
        public string defaultSchemeName = "Crux";

        // Dictionary for fast color lookups
        private Dictionary<string, Dictionary<string, Color>> colorDictionary = new Dictionary<string, Dictionary<string, Color>>();

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeColorDictionary();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeColorDictionary()
        {
            colorDictionary.Clear();
            
            // Initialize with some default colors if needed
            Dictionary<string, Color> defaultColors = new Dictionary<string, Color>
            {
                { "WeaponNodeBorder", new Color(0.2f, 0.5f, 0.8f) },
                { "WeaponNodeBorderHover", new Color(0.4f, 0.7f, 1.0f) }
            };

            // Add default colors to dictionary
            colorDictionary["Default"] = defaultColors;
            
            // Add all color schemes from the inspector
            foreach (var scheme in colorSchemes)
            {
                if (!colorDictionary.ContainsKey(scheme.schemeName))
                {
                    Dictionary<string, Color> schemeColors = new Dictionary<string, Color>();
                    
                    foreach (var entry in scheme.colors)
                    {
                        schemeColors[entry.colorName] = entry.color;
                    }
                    
                    colorDictionary[scheme.schemeName] = schemeColors;
                }
            }

            // Ensure all schemes have the required colors
            EnsureRequiredColors();
        }

        /// <summary>
        /// Ensures that all color schemes have the required colors
        /// </summary>
        private void EnsureRequiredColors()
        {
            // List of required color names
            string[] requiredColors = new string[] 
            { 
                "Primary", "Secondary", "Accent", "Background", 
                "Text", "TextDisabled", "Warning", "Error", "Success",
                "WeaponNodeHover", "WeaponNodeBorder", "WeaponNodeBorderHover"
            };

            // Get the default scheme
            if (colorDictionary.TryGetValue("Default", out Dictionary<string, Color> defaultScheme))
            {
                // For each scheme
                foreach (var schemePair in colorDictionary)
                {
                    string schemeName = schemePair.Key;
                    Dictionary<string, Color> scheme = schemePair.Value;

                    // For each required color
                    foreach (string colorName in requiredColors)
                    {
                        // If the scheme doesn't have this color
                        if (!scheme.ContainsKey(colorName))
                        {
                            // Add it from the default scheme if possible
                            if (defaultScheme.ContainsKey(colorName))
                            {
                                scheme[colorName] = defaultScheme[colorName];
                                Debug.Log($"Added missing color '{colorName}' to scheme '{schemeName}'");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a color from the current color scheme
        /// </summary>
        /// <param name="colorName">Name of the color to retrieve</param>
        /// <param name="schemeName">Optional scheme name, uses default if not specified</param>
        /// <returns>The requested color, or white if not found</returns>
        public Color GetColor(string colorName, string schemeName = "")
        {
            if (string.IsNullOrEmpty(schemeName))
            {
                schemeName = defaultSchemeName;
            }

            if (colorDictionary.TryGetValue(schemeName, out Dictionary<string, Color> scheme))
            {
                if (scheme.TryGetValue(colorName, out Color color))
                {
                    return color;
                }
                
                Debug.LogWarning($"Color '{colorName}' not found in scheme '{schemeName}'. Returning white.");
                return Color.white;
            }
            
            Debug.LogWarning($"Color scheme '{schemeName}' not found. Returning white.");
            return Color.white;
        }

        /// <summary>
        /// Adds or updates a color in a specific scheme
        /// </summary>
        public void SetColor(string colorName, Color color, string schemeName = "")
        {
            if (string.IsNullOrEmpty(schemeName))
            {
                schemeName = defaultSchemeName;
            }

            if (!colorDictionary.ContainsKey(schemeName))
            {
                colorDictionary[schemeName] = new Dictionary<string, Color>();
            }

            colorDictionary[schemeName][colorName] = color;
        }

        /// <summary>
        /// Refreshes the color dictionary from the inspector values
        /// Call this after making changes to the colorSchemes list at runtime
        /// </summary>
        public void RefreshColorDictionary()
        {
            InitializeColorDictionary();
        }

        /// <summary>
        /// Lists all available colors in the console for debugging
        /// </summary>
        public void DebugListAllColors()
        {
            Debug.Log("=== ColorManager: Available Colors ===");
            
            foreach (var schemePair in colorDictionary)
            {
                string schemeName = schemePair.Key;
                Dictionary<string, Color> scheme = schemePair.Value;
                
                Debug.Log($"Scheme: {schemeName} ({scheme.Count} colors)");
                
                foreach (var colorPair in scheme)
                {
                    Color c = colorPair.Value;
                    Debug.Log($"  - {colorPair.Key}: RGB({c.r:F2}, {c.g:F2}, {c.b:F2}, {c.a:F2})");
                }
            }
            
            Debug.Log("=====================================");
        }

        /// <summary>
        /// Ensures that a specific color exists in the default scheme
        /// </summary>
        public void EnsureColorExists(string colorName, Color defaultValue)
        {
            if (string.IsNullOrEmpty(defaultSchemeName))
            {
                defaultSchemeName = "Default";
            }

            // Make sure the default scheme exists
            if (!colorDictionary.ContainsKey(defaultSchemeName))
            {
                colorDictionary[defaultSchemeName] = new Dictionary<string, Color>();
            }

            // Check if the color exists in the default scheme
            if (!colorDictionary[defaultSchemeName].ContainsKey(colorName))
            {
                // Add it if it doesn't exist
                colorDictionary[defaultSchemeName][colorName] = defaultValue;
                Debug.Log($"Added missing color '{colorName}' to default scheme");
            }
        }
    }
} 