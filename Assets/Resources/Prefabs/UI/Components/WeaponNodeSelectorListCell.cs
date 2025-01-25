using TMPro;
using System;
using UnityEngine;
using System.Collections;

public class WeaponNodeSelectorListCell : MonoBehaviour
{
	// Inspector
	public SpriteRenderer IconComponent;
	public TextMeshProUGUI Name;
	public TextMeshProUGUI Description;
	// TEMP
	public TextMeshProUGUI ListIndexText;
	public int ListId;
	// Data
	public int ListPosition;
	public int WeaponListIndex;
	// Events
	public event Action<WeaponNodeSelectorListCell> OnScrollComplete;


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
		if (ListIndexText.text == "abc") ListIndexText.text = posData.Position.ToString();

		ListPosition = posData.Position;
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, posData.YPos, gameObject.transform.localPosition.z); // Update Y position
		// Debug.Log("Cell ListPosition: " + ListPosition);
		if (ListPosition == 1) EnableHover();
		else DisableHover();
	}

	public void SetData(int index, string name, string description, Sprite icon, int listId)
	{
		WeaponListIndex = index;
		Name.text = name;
		ListId = listId;
		Debug.Log($"ListID: {ListId} Cell: {Name.text} ListPosition: {ListPosition}");
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
		Vector3 initialPosition = gameObject.transform.localPosition; // Store initial position

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
			
			// Update position to the new Y position
			gameObject.transform.localPosition = new Vector3(initialPosition.x, Mathf.Lerp(initialPosition.y, posData.YPos, t), initialPosition.z);

			yield return null;
		}

		if (ListPosition == 1) EnableHover();
		OnScrollComplete?.Invoke(this);
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