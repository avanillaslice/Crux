using System.Collections.Generic;
using UnityEngine;

public class WeaponNode : MonoBehaviour
{
	// Inspector
	public Color HoverColor;
	public Color SelectedColor;

	// Data
	[HideInInspector] public float XPos;
	[HideInInspector] public float YPos;
	[HideInInspector] public RelativeSide Side;
	[HideInInspector] public WeaponNodeSelector WeaponNodeSelector;
	[HideInInspector] public bool IsSelected;
	[HideInInspector] public bool IsHoverState;
	private SpriteRenderer ColorComponent;
	private Color DefaultColor;
	private List<WeaponNode> LinkedWeaponNodes = new List<WeaponNode>();
	private AttachPoint AttachPoint;
	private WeaponSlot WeaponSlot;

	// Types
	internal enum ColorState
	{
		Default,
		Hover,
		Selected
	}

	void Awake() {
		ColorComponent = gameObject.GetComponent<SpriteRenderer>();
	}

	public void Init(AttachPoint attachPoint, WeaponSlot weaponSlot)
	{
		WeaponSlot = weaponSlot;
		AttachPoint = attachPoint;
		Side = AttachPoint.Side;
		XPos = attachPoint.transform.localPosition.x;
		YPos = attachPoint.transform.localPosition.y;
		Debug.Log("NEW NODE POSTION X: " + XPos + " Y: " + YPos);
	}

	public void AssignSelector(WeaponNodeSelector weaponNodeSelector)
	{
		if (weaponNodeSelector == null)
		{
			Debug.LogError("Cannot assign null WeaponNodeSelector");
			return;
		}

		if (WeaponSlot == null) Debug.LogWarning("WeaponSlot not set on Node");
		if (AttachPoint == null) Debug.LogWarning("AttachPoint not set on Node");

		WeaponNodeSelector = weaponNodeSelector;
		WeaponNodeSelector.UpdateContent(AttachPoint, WeaponSlot);
	}

	public void SetRelatedNodes(List<WeaponNode> weaponNodes)
	{
		foreach (WeaponNode weaponNode in weaponNodes)
		{
			if (weaponNode != this) LinkedWeaponNodes.Add(weaponNode);
		}
	}

	public void EnableHoverState()
	{
		if (IsHoverState || IsSelected) return;
		WeaponNodeSelector.EnableHoverState();
		IsHoverState = true;
		SetColorState(ColorState.Hover);
	}

	public void DisableHoverState()
	{
		if (!IsHoverState || IsSelected) return;
		WeaponNodeSelector.DisableHoverState();
		IsHoverState = false;
		SetColorState(ColorState.Default);
	}
	public void HandleSelect()
	{
		if (!IsSelected)
		{
			SetColorState(ColorState.Selected);
			IsSelected = true;

			foreach (WeaponNode weaponNode in LinkedWeaponNodes)
			{
				weaponNode.SetColorState(ColorState.Selected);
			}
		}

		WeaponNodeSelector.HandleSelect();
	}

	public void HandleDeselect()
	{
		if (IsSelected)
		{
			SetColorState(ColorState.Default); ;
			IsSelected = false;

			foreach (WeaponNode weaponNode in LinkedWeaponNodes)
			{
				weaponNode.SetColorState(ColorState.Default);
			}
		}

		WeaponNodeSelector.HandleDeselect();
	}

	internal void SetColorState(ColorState colorState)
	{
		switch (colorState)
		{
			case ColorState.Default:
				{
					ColorComponent.color = DefaultColor;
					break;
				}
			case ColorState.Hover:
				{
					ColorComponent.color = HoverColor;
					break;
				}
			case ColorState.Selected:
				{
					ColorComponent.color = SelectedColor;
					break;
				}
		}
	}

}