using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotNode : MonoBehaviour
{
	// Inspector
	public WeaponSlotSelector WeaponSlotSelector;
	public Color HoverColor;
	public Color SelectedColor;
	public Image ColorComponent;

	// Data
	public bool IsSelected;
	public bool IsHoverState;
	private Color DefaultColor;
	private List<WeaponSlotNode> RelatedWeaponSlotNodes;
	public AttachPoint AttachPoint;
	public WeaponSlot WeaponSlot;
	public float XPos;
	public float YPos;

	// Types
	internal enum ColorState
	{
		Default,
		Hover,
		Selected
	}

	public void Init(AttachPoint attachPoint, WeaponSlot weaponSlot)
	{
		WeaponSlot = weaponSlot;
		AttachPoint = attachPoint;
		XPos = attachPoint.transform.position.x;
		YPos = attachPoint.transform.position.y;
	}

	public void AssignSelector(WeaponSlotSelector weaponSlotSelector)
	{
		if (weaponSlotSelector == null)
		{
			Debug.LogError("Cannot assign null weaponSlotSelector");
			return;
		}
		WeaponSlotSelector = weaponSlotSelector;
		WeaponSlotSelector.UpdateWeaponDetails(AttachPoint, WeaponSlot);
	}

	public void SetRelatedNodes(List<WeaponSlotNode> weaponSlotNodes)
	{
		foreach (WeaponSlotNode weaponSlotNode in weaponSlotNodes)
		{
			if (weaponSlotNode != this) RelatedWeaponSlotNodes.Add(weaponSlotNode);
		}
	}

	public void EnableHoverState()
	{
		if (IsHoverState || IsSelected) return;
		WeaponSlotSelector.EnableHoverState();
		IsHoverState = true;
		SetColorState(ColorState.Hover);
	}

	public void DisableHoverState()
	{
		if (!IsHoverState || IsSelected) return;
		WeaponSlotSelector.DisableHoverState();
		IsHoverState = false;
		SetColorState(ColorState.Default);
	}
	public void HandleSelect()
	{
		if (!IsSelected)
		{
			SetColorState(ColorState.Selected);
			IsSelected = true;

			foreach (WeaponSlotNode weaponSlotNode in RelatedWeaponSlotNodes)
			{
				weaponSlotNode.SetColorState(ColorState.Selected);
			}
		}

		WeaponSlotSelector.HandleSelect();
	}

	public void HandleDeselect()
	{
		if (IsSelected)
		{
			SetColorState(ColorState.Default); ;
			IsSelected = false;

			foreach (WeaponSlotNode weaponSlotNode in RelatedWeaponSlotNodes)
			{
				weaponSlotNode.SetColorState(ColorState.Default);
			}
		}

		WeaponSlotSelector.HandleDeselect();
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