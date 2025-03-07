using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.UI.MainMenu
{
	public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		private Image buttonImage;
		private Color originalColor;
		public TextMeshProUGUI textComponent;

		private void Awake()
		{
			// Store the original vertex color of the text component
			if (textComponent != null)
			{
				originalColor = textComponent.color; // Assuming textComponent is of type TextMeshProUGUI
			}
			else
			{
				Debug.LogError("TextMeshProUGUI component not found on the GameObject.");
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			// Set the text component's color to white
			if (textComponent != null)
			{
				textComponent.color = Color.white;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			// Reset the text component's color to the original color
			if (textComponent != null)
			{
				textComponent.color = originalColor;
			}
		}
	}
}