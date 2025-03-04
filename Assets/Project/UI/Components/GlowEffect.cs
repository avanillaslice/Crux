using Project.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Components
{
    public class GlowEffect : MonoBehaviour
    {
        private Material GlowMaterial;
        private Image ImageComponent;

        void Awake()
        {
            // Load the CellBorderGlowMat using AssetManager
            GlowMaterial = AssetManager.LoadAsset<Material>("CellBorderGlowMat");

            if (GlowMaterial) GlowMaterial = new Material(GlowMaterial); // Clone to avoid modifying all materials
            else Debug.LogError("CellBorderGlowMat not found in Addressables");
        }

        public void Init(Image image, Color defaultColor)
        {
            ImageComponent = image;
            ImageComponent.material = GlowMaterial;
            SetColor(defaultColor);
            SetGlow(1f);
        }

        public void SetColor(Color color)
        {
            GlowMaterial.SetColor("_BaseColor", color);
        }

        public void SetOpacity(float alpha)
        {
            GlowMaterial.SetFloat("_Alpha", alpha);
        }

        public void SetGlow(float glowAmt)
        {
            GlowMaterial.SetFloat("_GlowIntensity", glowAmt);
        }
    }
}
