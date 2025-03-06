using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace Crux.Utilities.Editor
{
    /// <summary>
    /// Editor utility for debugging and fixing ColorManager issues
    /// </summary>
    public static class ColorManagerDebugger
    {
        [MenuItem("Crux/Debug/List All Colors")]
        public static void ListAllColors()
        {
            if (ColorManager.Instance == null)
            {
                Debug.LogError("ColorManager instance not found. Create one first using Crux > Setup > Create Color Manager");
                return;
            }

            ColorManager.Instance.DebugListAllColors();
        }

        [MenuItem("Crux/Debug/Fix WeaponNodeHover Color")]
        public static void FixWeaponNodeHoverColor()
        {
            if (ColorManager.Instance == null)
            {
                Debug.LogError("ColorManager instance not found. Create one first using Crux > Setup > Create Color Manager");
                return;
            }

            // Add the WeaponNodeHover color to the default scheme with a cyan color
            ColorManager.Instance.EnsureColorExists("WeaponNodeHover", new Color(0f, 0.8f, 1f));
            
            // Refresh the color dictionary
            ColorManager.Instance.RefreshColorDictionary();
            
            Debug.Log("WeaponNodeHover color has been added/fixed in the ColorManager");
            
            // List all colors to verify
            ColorManager.Instance.DebugListAllColors();
        }

        [MenuItem("Crux/Debug/Add WeaponNodeHover To All Schemes")]
        public static void AddWeaponNodeHoverToAllSchemes()
        {
            if (ColorManager.Instance == null)
            {
                Debug.LogError("ColorManager instance not found. Create one first using Crux > Setup > Create Color Manager");
                return;
            }

            // Get the ColorManager component
            ColorManager colorManager = ColorManager.Instance;
            
            // Default color to use
            Color defaultHoverColor = new Color(0f, 0.8f, 1f);
            
            // Add the color to each scheme in the inspector
            bool colorFound = false;
            foreach (var scheme in colorManager.colorSchemes)
            {
                bool schemeHasColor = false;
                
                // Check if the scheme already has the color
                foreach (var entry in scheme.colors)
                {
                    if (entry.colorName == "WeaponNodeHover")
                    {
                        schemeHasColor = true;
                        colorFound = true;
                        break;
                    }
                }
                
                // Add the color if it doesn't exist
                if (!schemeHasColor)
                {
                    scheme.colors.Add(new ColorManager.ColorEntry { 
                        colorName = "WeaponNodeHover", 
                        color = defaultHoverColor 
                    });
                    
                    Debug.Log($"Added WeaponNodeHover color to scheme '{scheme.schemeName}'");
                }
            }
            
            // If no schemes had the color, add a new scheme with the color
            if (!colorFound && colorManager.colorSchemes.Count == 0)
            {
                ColorManager.ColorScheme defaultScheme = new ColorManager.ColorScheme
                {
                    schemeName = "Default"
                };
                
                defaultScheme.colors.Add(new ColorManager.ColorEntry { 
                    colorName = "WeaponNodeHover", 
                    color = defaultHoverColor 
                });
                
                colorManager.colorSchemes.Add(defaultScheme);
                Debug.Log("Created a new Default scheme with WeaponNodeHover color");
            }
            
            // Refresh the color dictionary
            colorManager.RefreshColorDictionary();
            
            Debug.Log("WeaponNodeHover color has been added to all schemes");
            
            // Mark the object as dirty so Unity saves the changes
            EditorUtility.SetDirty(colorManager);
            
            // List all colors to verify
            colorManager.DebugListAllColors();
        }
    }
}
#endif 