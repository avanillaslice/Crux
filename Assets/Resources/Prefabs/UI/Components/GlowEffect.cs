using UnityEngine;
using UnityEngine.UI;

public class GlowEffect : MonoBehaviour
{
    private Material GlowMaterial;
    private Image ImageComponent;

    void Awake()
    {
        // Load the CellBorderGlowMat from Resources
        GlowMaterial = Resources.Load<Material>("Prefabs/UI/Loadout/CellBorderGlowMat");
        if (GlowMaterial)
        {
            GlowMaterial = new Material(GlowMaterial); // Clone to avoid modifying all materials
        }
        else
        {
            Debug.LogError("CellBorderGlowMat not found in Resources/Prefabs/UI/Loadout");
        }
    }

	public void Init(Image image, Color defaultColor) {
		ImageComponent = image;
		ImageComponent.material = GlowMaterial;
		Debug.Log("Set ImageComponent Material!");
		SetColor(defaultColor);
		SetGlow(1f);
	}

	public void SetColor(Color color) {
		GlowMaterial.SetColor("_BaseColor", color);
	}

	public void SetGlow(float glowAmt) {
		GlowMaterial.SetFloat("_GlowIntensity", glowAmt);
	}
}
