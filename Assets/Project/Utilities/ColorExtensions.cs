using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Crux.Utilities
{
    /// <summary>
    /// Extension methods for easily applying colors from the ColorManager
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Sets the color of an Image component using a named color from ColorManager
        /// </summary>
        public static void SetNamedColor(this Image image, string colorName, string schemeName = "")
        {
            if (ColorManager.Instance != null)
            {
                image.color = ColorManager.Instance.GetColor(colorName, schemeName);
            }
        }

        /// <summary>
        /// Sets the color of a TextMeshProUGUI component using a named color from ColorManager
        /// </summary>
        public static void SetNamedColor(this TextMeshProUGUI text, string colorName, string schemeName = "")
        {
            if (ColorManager.Instance != null)
            {
                text.color = ColorManager.Instance.GetColor(colorName, schemeName);
            }
        }

        /// <summary>
        /// Sets the color of a SpriteRenderer component using a named color from ColorManager
        /// </summary>
        public static void SetNamedColor(this SpriteRenderer renderer, string colorName, string schemeName = "")
        {
            if (ColorManager.Instance != null)
            {
                renderer.color = ColorManager.Instance.GetColor(colorName, schemeName);
            }
        }

        /// <summary>
        /// Sets the color of a Material using a named color from ColorManager
        /// </summary>
        public static void SetNamedColor(this Material material, string colorName, string propertyName = "_Color", string schemeName = "")
        {
            if (ColorManager.Instance != null)
            {
                material.SetColor(propertyName, ColorManager.Instance.GetColor(colorName, schemeName));
            }
        }

        /// <summary>
        /// Gets a named color from the ColorManager
        /// </summary>
        public static Color GetNamedColor(this MonoBehaviour behaviour, string colorName, string schemeName = "")
        {
            if (ColorManager.Instance != null)
            {
                return ColorManager.Instance.GetColor(colorName, schemeName);
            }
            return Color.white;
        }
    }
} 