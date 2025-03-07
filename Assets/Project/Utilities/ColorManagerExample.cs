using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Crux.Utilities
{
    /// <summary>
    /// Example script demonstrating how to use the ColorManager
    /// </summary>
    public class ColorManagerExample : MonoBehaviour
    {
        [Header("UI References")]
        public Image backgroundImage;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public Button primaryButton;
        public Button secondaryButton;
        
        [Header("Color Scheme")]
        public string colorSchemeName = ""; // Leave empty to use default

        private void Start()
        {
            ApplyColorScheme();
        }

        /// <summary>
        /// Applies the selected color scheme to all UI elements
        /// </summary>
        public void ApplyColorScheme()
        {
            if (ColorManager.Instance == null)
            {
                Debug.LogError("ColorManager instance not found. Make sure it exists in the scene.");
                return;
            }

            // Apply colors using extension methods
            if (backgroundImage != null)
                backgroundImage.SetNamedColor("Background", colorSchemeName);
            
            if (titleText != null)
                titleText.SetNamedColor("Text", colorSchemeName);
            
            if (descriptionText != null)
                descriptionText.SetNamedColor("TextDisabled", colorSchemeName);
            
            if (primaryButton != null)
            {
                Image buttonImage = primaryButton.GetComponent<Image>();
                if (buttonImage != null)
                    buttonImage.SetNamedColor("Primary", colorSchemeName);
                
                TextMeshProUGUI buttonText = primaryButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.SetNamedColor("Text", colorSchemeName);
            }
            
            if (secondaryButton != null)
            {
                Image buttonImage = secondaryButton.GetComponent<Image>();
                if (buttonImage != null)
                    buttonImage.SetNamedColor("Secondary", colorSchemeName);
                
                TextMeshProUGUI buttonText = secondaryButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.SetNamedColor("Text", colorSchemeName);
            }
        }

        /// <summary>
        /// Switches between available color schemes
        /// </summary>
        public void SwitchColorScheme(string newSchemeName)
        {
            colorSchemeName = newSchemeName;
            ApplyColorScheme();
        }

        /// <summary>
        /// Example of how to get a color directly
        /// </summary>
        public void ExampleOfDirectColorUsage()
        {
            // Direct access to a color
            Color primaryColor = ColorManager.Instance.GetColor("Primary", colorSchemeName);
            
            // Using extension method
            Color secondaryColor = this.GetNamedColor("Secondary", colorSchemeName);
            
            Debug.Log($"Primary color: {primaryColor}, Secondary color: {secondaryColor}");
        }
    }
} 