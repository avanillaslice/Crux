using System;
using System.Collections;
using System.Collections.Generic;
using Crux.Utilities;
using Project.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.Loadout
{
	/// <summary>
	/// Represents a cell in a weapon selector list.
	/// </summary>
	public class WeaponNodeSelectorListCell : MonoBehaviour
	{
		// Inspector
		[Header("UI References")]
		[SerializeField] private SpriteRenderer iconComponent;
		[SerializeField] private TextMeshProUGUI nameText;
		[SerializeField] private TextMeshProUGUI descriptionText;
		[SerializeField] private List<GameObject> backgroundComponentContainer;
		[SerializeField] public Animator Animator;
		[SerializeField] private TextMeshProUGUI listIndexText;

		// Data properties
		public int ListId { get; private set; }
		public int ListPosition { get; private set; }
		public int WeaponListIndex { get; private set; }

		// Events
		public event Action<WeaponNodeSelectorListCell> OnScrollComplete;

		// Private fields
		private List<BorderComponent> borderComponents;
		private List<Image> backgroundComponents;
		private SpriteRenderer[] spriteRenderers;
		private Color defaultNameColor;
		private float defaultNameGlow;

		/// <summary>
		/// Helper class for managing border components.
		/// </summary>
		private class BorderComponent
		{
			private Image component;
			private Color defaultColor;
			private GlowEffect glowShader;

			public BorderComponent(Transform uiBorderTransform)
			{
				component = uiBorderTransform.GetComponent<Image>();
				if (component != null) defaultColor = component.color;
				else Debug.LogWarning($"No Image component found on {uiBorderTransform.name}");

				glowShader = uiBorderTransform.GetComponent<GlowEffect>();
				if (glowShader == null) glowShader = uiBorderTransform.gameObject.AddComponent<GlowEffect>();

				glowShader.Init(component, defaultColor);
			}

			public void EnableGlow()
			{
				glowShader.SetGlow(1.5f);
			}

			public void DisableGlow()
			{
				glowShader.SetGlow(1f);
			}

			public void SetColor(Color? newColor)
			{
				Color targetColor = newColor ?? defaultColor;
				glowShader.SetColor(targetColor);
			}

			public void SetOpacity(float newOpacity)
			{
				glowShader.SetOpacity(newOpacity);
			}
		}

		void Awake()
		{
			spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
			InitializeBackgroundComponents();

			// Set default name color and glow
			defaultNameColor = nameText.color;
			defaultNameGlow = nameText.fontMaterial.GetFloat("_GlowPower");
		}

		/// <summary>
		/// Initializes the border components.
		/// </summary>
		private void InitializeBackgroundComponents()
		{
			borderComponents = new List<BorderComponent>();
			backgroundComponents = new List<Image>();
			foreach (GameObject child in backgroundComponentContainer) {
				FindUIBackgroundComponents(child.transform);
			}
		}

		/// <summary>
		/// Recursively finds UI border components in the hierarchy.
		/// </summary>
		/// <param name="parent">The parent transform to search</param>
		private void FindUIBackgroundComponents(Transform parent)
		{
			foreach (Transform child in parent)
			{
				if (child.CompareTag("UIBorder"))
				{
					borderComponents.Add(new BorderComponent(child));
				}
				else
				{
					Image image = child.GetComponent<Image>();
					if (image != null) backgroundComponents.Add(image);
				}
			}
		}

		/// <summary>
		/// Initializes the cell with position data.
		/// </summary>
		/// <param name="posData">The position data</param>
		public void Init(WeaponNodeSelectorList.PosData posData)
		{
			gameObject.transform.localScale = new Vector3(posData.Scale, posData.Scale, gameObject.transform.localScale.z);
			SetCellOpacity(posData.Opacity);

			if (listIndexText != null && listIndexText.text == "abc") listIndexText.text = posData.Position.ToString();

			ListPosition = posData.Position;
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, posData.YPos, gameObject.transform.localPosition.z);
		}

		/// <summary>
		/// Sets the data for this cell.
		/// </summary>
		/// <param name="index">The weapon index</param>
		/// <param name="name">The weapon name</param>
		/// <param name="description">The weapon description</param>
		/// <param name="icon">The weapon icon</param>
		/// <param name="listId">The list ID</param>
		public void SetData(int index, string name, string description, Sprite icon, int listId)
		{
			WeaponListIndex = index;
			nameText.text = name;
			ListId = listId;

			// Optionally set description and icon if needed
			if (descriptionText != null) descriptionText.text = description;
			if (iconComponent != null) iconComponent.sprite = icon;
		}

		/// <summary>
		/// Scrolls the cell to a new position.
		/// </summary>
		/// <param name="posData">The target position data</param>
		public void Scroll(WeaponNodeSelectorList.PosData posData)
		{
			StartCoroutine(TransitionToPosition(posData));
		}

		/// <summary>
		/// Transitions the cell to a new position.
		/// </summary>
		/// <param name="posData">The target position data</param>
		private IEnumerator TransitionToPosition(WeaponNodeSelectorList.PosData posData)
		{
			float duration = 0.25f; // Duration of the transition
			float elapsedTime = 0f;

			float initialOpacity = spriteRenderers[0].color.a;
			Vector3 initialScale = gameObject.transform.localScale;
			Vector3 initialPosition = gameObject.transform.localPosition;

			if (posData.Position == 3)
			{
				// If shifting to active cell position
				Animator.Play("ActivateCell");
			}
			else if (ListPosition == 3)
			{
				// If shifting from active cell position
				Animator.Play("DeactivateCell");
				DisableHover();
			}

			ListPosition = posData.Position;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / duration;

				// Lerp opacity, scale, and position
				SetCellOpacity(Mathf.Lerp(initialOpacity, posData.Opacity, t));
				gameObject.transform.localScale = Vector3.Lerp(initialScale, new Vector3(posData.Scale, posData.Scale, initialScale.z), t);
				gameObject.transform.localPosition = new Vector3(initialPosition.x, Mathf.Lerp(initialPosition.y, posData.YPos, t), initialPosition.z);

				yield return null;
			}

			if (ListPosition == 3) EnableHover();

			OnScrollComplete?.Invoke(this);
		}

		/// <summary>
		/// Sets the opacity of all cell components.
		/// </summary>
		/// <param name="targetOpacity">The target opacity</param>
		private void SetCellOpacity(float targetOpacity)
		{
			// Set sprite renderer opacity
			foreach (SpriteRenderer renderer in spriteRenderers)
			{
				Color color = renderer.color;
				renderer.color = new Color(color.r, color.g, color.b, targetOpacity);
			}

			// Set border component opacity
			foreach (BorderComponent borderComponent in borderComponents)
			{
				borderComponent.SetOpacity(targetOpacity);
			}

			// Set background component opacity
			foreach (Image backgroundComponent in backgroundComponents)
			{
				Color color = backgroundComponent.color;
				backgroundComponent.color = new Color(color.r, color.g, color.b, targetOpacity);
			}

			// Set text opacity
			nameText.alpha = targetOpacity;
			if (descriptionText != null) descriptionText.alpha = targetOpacity;
		}

		/// <summary>
		/// Enables the hover state for this cell.
		/// </summary>
		public void EnableHover()
		{
			// Get the hover color from ColorManager
			Color hoverColor = ColorManager.Instance.GetColor("WeaponNodeBorderHover");

			// Set the text color
			nameText.color = hoverColor;

			// Set the glow color and intensity
			Material textMaterial = nameText.fontMaterial;
			textMaterial.SetColor("_GlowColor", hoverColor);
			textMaterial.SetFloat("_GlowPower", 1.0f);

			// Set the border color and glow
			foreach (BorderComponent borderComponent in borderComponents)
			{
				borderComponent.SetColor(hoverColor);
				borderComponent.EnableGlow();
			}
		}

		/// <summary>
		/// Disables the hover state for this cell.
		/// </summary>
		public void DisableHover()
		{
			// Reset the text color
			nameText.color = defaultNameColor;

			// Reset the glow color and intensity
			Material textMaterial = nameText.fontMaterial;
			textMaterial.SetColor("_GlowColor", defaultNameColor);
			textMaterial.SetFloat("_GlowPower", defaultNameGlow);

			// Reset the border color and glow
			foreach (BorderComponent borderComponent in borderComponents)
			{
				borderComponent.SetColor(null);
				borderComponent.DisableGlow();
			}
		}
	}
}