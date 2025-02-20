using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class WeaponNodeSelectorListCell : MonoBehaviour
{
	// Inspector
	public SpriteRenderer IconComponent;
	public TextMeshProUGUI Name;
	public TextMeshProUGUI Description;
	public List<Image> BackgroundComponents;
	public Animator Animator;
	// TEMP
	public TextMeshProUGUI ListIndexText;
	public int ListId;
	// Data
	public int ListPosition;
	public int WeaponListIndex;
	private List<BorderComponent> BorderComponents;
	private SpriteRenderer[] SpriteRenderers;
	private Color DefaultNameColor;
	private float DefaultNameGlow;
	// Events
	public event Action<WeaponNodeSelectorListCell> OnScrollComplete;

	// BorderComponent - Handles Color Swaps
	private class BorderComponent {
		private Image Component;
		private Color DefaultColor;
		private GlowEffect GlowShader;

		public BorderComponent(Transform uiBorderTransform) {
			Component = uiBorderTransform.GetComponent<Image>();
			if (Component != null) DefaultColor = Component.color;
			else Debug.LogWarning($"No Image component found on {uiBorderTransform.name}");

			GlowShader = uiBorderTransform.GetComponent<GlowEffect>();
			if (GlowShader == null) GlowShader = uiBorderTransform.gameObject.AddComponent<GlowEffect>();

			GlowShader.Init(Component, DefaultColor);
		}

		public void EnableGlow() {
			GlowShader.SetGlow(1.5f);
		}

		public void DisableGlow() {
			GlowShader.SetGlow(1f);
		}

		public void SetColor(Color? newColor) {
			Color targetColor = newColor ?? DefaultColor;
			// targetColor.a = Component.color.a; // Preserve existing alpha
			// Component.color = targetColor;
			GlowShader.SetColor(targetColor);
		}

		public void SetOpacity(float newOpacity) {
			// Color color = Component.color;
			// color.a = newOpacity;
			// Component.color = color;
			// GlowShader.SetColor(color);
			GlowShader.SetOpacity(newOpacity);
		}
	}

    void Awake()
    {
        SpriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        InitialiseBorder();

        // Set DefaultNameColor and DefaultNameGlow
        DefaultNameColor = Name.color;
        DefaultNameGlow = Name.fontMaterial.GetFloat("_GlowPower");
    }

	private void InitialiseBorder() {
		BorderComponents = new List<BorderComponent>();
		FindUIBorderComponents(transform);
	}

	private void FindUIBorderComponents(Transform parent) {
		foreach (Transform child in parent)
		{
			if (child.CompareTag("UIBorder"))
			{
				BorderComponents.Add(new BorderComponent(child));
			}
			// Recursively check the children of this child
			FindUIBorderComponents(child);
		}
	}

    public void Init(WeaponNodeSelectorList.PosData posData)
	{
		gameObject.transform.localScale = new Vector3(posData.Scale, posData.Scale, gameObject.transform.localScale.z); // Set scale to 75%
		SetCellOpacity(posData.Opacity);
		if (ListIndexText.text == "abc") ListIndexText.text = posData.Position.ToString();

		ListPosition = posData.Position;
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, posData.YPos, gameObject.transform.localPosition.z); // Update Y position
		// Debug.Log("Cell ListPosition: " + ListPosition);
		// if (ListPosition == 3) EnableActive();
		// else DisableActive();
	}

	public void SetData(int index, string name, string description, Sprite icon, int listId)
	{
		WeaponListIndex = index;
		Name.text = name;
		ListId = listId;
		// Debug.Log($"ListID: {ListId} Cell: {Name.text} ListPosition: {ListPosition}");
		// Description.text = description;
		// IconComponent.sprite = icon;
	}

	public void Scroll(WeaponNodeSelectorList.PosData posData)
	{
		StartCoroutine(TransitionToPosition(posData));
	}

	private IEnumerator TransitionToPosition(WeaponNodeSelectorList.PosData posData)
	{
		float duration = 0.25f; // Duration of the transition
		float elapsedTime = 0f;

		float initialOpacity = SpriteRenderers[0].color.a;
		Vector3 initialScale = gameObject.transform.localScale;
		Vector3 initialPosition = gameObject.transform.localPosition; // Store initial position

		if (posData.Position == 3) { // If shifting to ActiveCell positon
			Animator.Play("ActivateCell");
		}
		else if (ListPosition == 3) { // If shifting from ActiveCell position
			Animator.Play("DeactivateCell");
			DisableHover();
		}

		ListPosition = posData.Position;
		// ListIndexText.text = posData.Position.ToString();

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;

			// Lerp Opacity, Scale, and YPos
			SetCellOpacity(Mathf.Lerp(initialOpacity, posData.Opacity, t));
			gameObject.transform.localScale = Vector3.Lerp(initialScale, new Vector3(posData.Scale, posData.Scale, initialScale.z), t);
			gameObject.transform.localPosition = new Vector3(initialPosition.x, Mathf.Lerp(initialPosition.y, posData.YPos, t), initialPosition.z);

			yield return null;
		}

		if (ListPosition == 3) EnableHover();

		OnScrollComplete?.Invoke(this);
	}

	private void SetCellOpacity(float targetOpacity) {
		foreach (SpriteRenderer renderer in SpriteRenderers)
		{
			Color rcolor = renderer.color;
			renderer.color = new Color(rcolor.r, rcolor.g, rcolor.b, targetOpacity);
		}

		foreach (BorderComponent borderComponent in BorderComponents)
		{
			borderComponent.SetOpacity(targetOpacity);
		}

		foreach (var backgroundComponent in BackgroundComponents)
		{
			Color bgcolor = backgroundComponent.color;
			backgroundComponent.color = new Color(bgcolor.r, bgcolor.g, bgcolor.b, targetOpacity);
		}

		Name.alpha = targetOpacity;
	}

	public void EnableHover()
	{
		Debug.Log("Enabling Hover State");
		// Set the vertex color of the TextMeshPro component
		Name.color = Color.white;

		// Set the glow color in the default TextMeshPro shader
		Material textMaterial = Name.fontMaterial;
		textMaterial.SetColor("_GlowColor", Color.white);
		textMaterial.SetFloat("_GlowPower", 1.0f); // Set the glow intensity to full
		foreach (var borderComponent in BorderComponents)
		{
			borderComponent.SetColor(Color.white);
			borderComponent.EnableGlow();
		}
	}

	public void DisableHover()
	{
		// Set the vertex color of the TextMeshPro component
		Name.color = DefaultNameColor;

		// Set the glow color in the default TextMeshPro shader
		Material textMaterial = Name.fontMaterial;
		textMaterial.SetColor("_GlowColor", DefaultNameColor);
		textMaterial.SetFloat("_GlowPower", DefaultNameGlow); // Set the glow intensity to full
		foreach (var borderComponent in BorderComponents)
		{
			borderComponent.SetColor(null);
			borderComponent.DisableGlow();
		}
	}

	public void DisableActive()
	{
	}


	public void EnableActive()
	{
	}
}