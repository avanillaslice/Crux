using TMPro;
using UnityEngine;

public class WeaponNodeSelectorListCell : MonoBehaviour
{
	// Types
	public enum CellState
	{
		Default,
		Hover,
		Active,
	}

	// Inspector
	public SpriteRenderer IconComponent;
	public TextMeshProUGUI Name;
	public TextMeshProUGUI Description;

	// Data
	public int WeaponListIndex;
	private CellState State = CellState.Default;

	public void SetData(int index, string name, string description, Sprite icon)
	{
		Name.text = name;
		Description.text = description;
		IconComponent.sprite = icon;
		WeaponListIndex = index;
	}

	public void SetState(CellState state)
	{
		if (state == State) return;

		switch (state)
		{
			case CellState.Default: TransitionToDefault(); break;
			case CellState.Hover: TransitionToHover(); break;
			case CellState.Active: TransitionToActive(); break;
		}
	}

	private void TransitionToDefault()
	{
		if (State == CellState.Active) DisableActive();
		DisableHover();
	}

	private void TransitionToActive()
	{
		if (State == CellState.Default)
		{
			Debug.LogWarning("Cannot set state to Active when Default");
			return;
		}
		EnableActive();
	}

	private void TransitionToHover()
	{
		if (State == CellState.Active) DisableActive();
		else EnableHover();
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