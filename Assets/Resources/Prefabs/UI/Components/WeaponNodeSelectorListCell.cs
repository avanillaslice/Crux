using TMPro;
using UnityEngine;
using System.Collections;

public class WeaponNodeSelectorListCell : MonoBehaviour
{
	// Inspector
	public SpriteRenderer IconComponent;
	public TextMeshProUGUI Name;
	public TextMeshProUGUI ListIndexText;
	public TextMeshProUGUI Description;

	// Data
	public int ListPosition;
	public int WeaponListIndex;

	public void Init(WeaponNodeSelectorList.PosData posData)
	{
		gameObject.transform.localScale = new Vector3(posData.Scale, posData.Scale, gameObject.transform.localScale.z); // Set scale to 75%
		SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer renderer in spriteRenderers)
		{
			Color color = renderer.color;
			color.a = posData.Opacity; // Set opacity to 50%
			renderer.color = color;
		}
		Name.alpha = posData.Opacity;
		Debug.Log(ListIndexText.text);
		if (ListIndexText.text == "abc") ListIndexText.text = posData.Position.ToString();

		ListPosition = posData.Position;
		if (ListPosition == 1) EnableHover();
		else DisableHover();
	}

	public void SetData(int index, string name, string description, Sprite icon) {
		WeaponListIndex = index;
		Name.text = name;
		// Description.text = description;
		// IconComponent.sprite = icon;
	}

	public void Scroll(WeaponNodeSelectorList.PosData posData)
	{
		if (ListPosition == 1) DisableHover();
		StartCoroutine(TransitionToPosition(posData));
	}

	private IEnumerator TransitionToPosition(WeaponNodeSelectorList.PosData posData)
	{
	    float duration = 0.5f; // Duration of the transition
	    float elapsedTime = 0f;

	    SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
	    float initialOpacity = spriteRenderers[0].color.a;
	    Vector3 initialScale = gameObject.transform.localScale;

		ListPosition = posData.Position;
		// ListIndexText.text = posData.Position.ToString();
	    while (elapsedTime < duration)
	    {
	        elapsedTime += Time.deltaTime;
	        float t = elapsedTime / duration;

	        foreach (SpriteRenderer renderer in spriteRenderers)
	        {
	            Color color = renderer.color;
	            color.a = Mathf.Lerp(initialOpacity, posData.Opacity, t);
	            renderer.color = color;
	        }
			Name.alpha = Mathf.Lerp(initialOpacity, posData.Opacity, t);

	        gameObject.transform.localScale = Vector3.Lerp(initialScale, new Vector3(posData.Scale, posData.Scale, initialScale.z), t);

	        yield return null;
	    }
		
		if (ListPosition == 1) EnableHover();
	}

	// Shifts to Default style
	private void DisableHover()
	{
	}

	// Shifts to Hover from Default
	private void EnableHover()
	{
	}

	// Shifts to Hover from Active
	private void DisableActive()
	{

	}

	// Shifts to Active from Hover
	private void EnableActive()
	{

	}
}