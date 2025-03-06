using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace Crux.Utilities.Editor
{
    /// <summary>
    /// Editor utility for setting up the ColorManager in the scene
    /// </summary>
    public static class ColorManagerSetup
    {
        [MenuItem("Crux/Setup/Create Color Manager")]
        public static void CreateColorManager()
        {
            // Check if a ColorManager already exists in the scene
            if (ColorManager.Instance != null)
            {
                Debug.Log("A ColorManager already exists in the scene.");
                Selection.activeGameObject = ColorManager.Instance.gameObject;
                return;
            }

            // Create a new GameObject with the ColorManager component
            GameObject colorManagerObject = new GameObject("ColorManager");
            ColorManager colorManager = colorManagerObject.AddComponent<ColorManager>();

            // Set up default color schemes
            SetupDefaultColorSchemes(colorManager);

            // Select the new GameObject in the hierarchy
            Selection.activeGameObject = colorManagerObject;
            
            Debug.Log("ColorManager created successfully. Configure your color schemes in the Inspector.");
        }

        private static void SetupDefaultColorSchemes(ColorManager colorManager)
        {
            // Create a default color scheme
            ColorManager.ColorScheme defaultScheme = new ColorManager.ColorScheme
            {
                schemeName = "Default"
            };

            // Add some default colors
            defaultScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Primary", color = new Color(0.0f, 0.6f, 1.0f) });
            defaultScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Secondary", color = new Color(1.0f, 0.6f, 0.0f) });
            defaultScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Accent", color = new Color(1.0f, 0.2f, 0.4f) });
            defaultScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Background", color = new Color(0.1f, 0.1f, 0.2f) });
            defaultScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Text", color = Color.white });
            defaultScheme.colors.Add(new ColorManager.ColorEntry { colorName = "TextDisabled", color = new Color(0.5f, 0.5f, 0.5f) });
            defaultScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Warning", color = Color.yellow });
            defaultScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Error", color = Color.red });
            defaultScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Success", color = Color.green });

            // Add a dark theme color scheme
            ColorManager.ColorScheme darkScheme = new ColorManager.ColorScheme
            {
                schemeName = "Dark"
            };

            darkScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Primary", color = new Color(0.0f, 0.4f, 0.8f) });
            darkScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Secondary", color = new Color(0.8f, 0.4f, 0.0f) });
            darkScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Accent", color = new Color(0.8f, 0.1f, 0.3f) });
            darkScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Background", color = new Color(0.05f, 0.05f, 0.1f) });
            darkScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Text", color = new Color(0.9f, 0.9f, 0.9f) });
            darkScheme.colors.Add(new ColorManager.ColorEntry { colorName = "TextDisabled", color = new Color(0.4f, 0.4f, 0.4f) });
            darkScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Warning", color = new Color(0.9f, 0.9f, 0.2f) });
            darkScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Error", color = new Color(0.9f, 0.2f, 0.2f) });
            darkScheme.colors.Add(new ColorManager.ColorEntry { colorName = "Success", color = new Color(0.2f, 0.9f, 0.2f) });

            // Add the schemes to the ColorManager
            colorManager.colorSchemes.Add(defaultScheme);
            colorManager.colorSchemes.Add(darkScheme);
            
            // Set the default scheme
            colorManager.defaultSchemeName = "Default";
        }
    }
}
#endif 