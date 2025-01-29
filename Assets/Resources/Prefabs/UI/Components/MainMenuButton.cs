using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image buttonImage;
    private Color originalColor;

	private void Awake()
	{
	    // Get the Image component attached to the GameObject
	    buttonImage = GetComponent<Image>();
	    if (buttonImage != null)
	    {
	        // Store the original color of the Image
	        originalColor = buttonImage.color;
	        // Set the Image to transparent
	        buttonImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
	    }
	    else
	    {
	        Debug.LogError("Image component not found on the GameObject.");
	    }
	}
	
	public void OnPointerEnter(PointerEventData eventData)
	{
	    // Set the Image to its original color (visible)
	    if (buttonImage != null)
	    {
	        buttonImage.color = originalColor;
	    }
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{
	    // Set the Image to transparent
	    if (buttonImage != null)
	    {
	        buttonImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
	    }
	}
}