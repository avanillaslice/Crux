using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
	[HideInInspector] public NodeState State;
	private Image ColorComponent;
	private Color DefaultColor;
	private List<WeaponNode> LinkedWeaponNodes = new List<WeaponNode>();
	private AttachPoint AttachPoint;
	private WeaponSlot WeaponSlot;

	//Temp
	int NodeId;

	// Types
	public enum NodeState
	{
		Default,
		Hover,
		Selected
	}

	void Awake() {
		ColorComponent = gameObject.GetComponent<Image>();
		DefaultColor = ColorComponent.color;
		State = NodeState.Default;
	}

	public void Init(AttachPoint attachPoint, WeaponSlot weaponSlot, int nodeId)
	{
		NodeId = nodeId;
		WeaponSlot = weaponSlot;
		AttachPoint = attachPoint;
		Side = AttachPoint.Side;
		XPos = attachPoint.transform.localPosition.x;
		YPos = attachPoint.transform.localPosition.y;
		// Debug.Log("NEW NODE POSTION X: " + XPos + " Y: " + YPos);
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
		WeaponNodeSelector.UpdateContent(AttachPoint, WeaponSlot, NodeId);
	}

	public void SetRelatedNodes(List<WeaponNode> weaponNodes)
	{
		foreach (WeaponNode weaponNode in weaponNodes)
		{
			if (weaponNode != this) LinkedWeaponNodes.Add(weaponNode);
		}
	}

	public void HandlePointerEnter() {
		LoadoutUI.Inst.HandlePointerEnterOnNode(this);
	}

	public void HandlePointerExit() {
		LoadoutUI.Inst.HandlePointerExitOnNode(this);
	}

	public void HandlePointerClick() {
		LoadoutUI.Inst.HandlePointerClickOnNode(this);
	}

	public void EnableHover()
	{
		if (State == NodeState.Hover || State == NodeState.Selected) return;
		SetState(NodeState.Hover);
	}

	public void DisableHover()
	{
		if (State != NodeState.Hover || State == NodeState.Selected) return;
		SetState(NodeState.Default);
	}
	
	public void HandleSelect()
	{
		if (State != NodeState.Hover) return;
		SetState(NodeState.Selected);
	}

	public void HandleDeselect()
	{
		if (State != NodeState.Selected) return;
		SetState(NodeState.Hover);
	}

	internal void SetState(NodeState state)
	{
		switch (state)
		{
			case NodeState.Default: {
				ColorComponent.color = DefaultColor;
				if (State == NodeState.Hover) WeaponNodeSelector.DisableHoverState();
				if (State == NodeState.Selected) WeaponNodeSelector.HandleDeselect();
				break;
			}
			case NodeState.Hover: {
				ColorComponent.color = HoverColor;
				WeaponNodeSelector.EnableHoverState();
				if (State == NodeState.Selected) {
					foreach (WeaponNode weaponNode in LinkedWeaponNodes) weaponNode.SetState(NodeState.Default);
				}
				WeaponNodeSelector.HandleDeselect();
				break;
			}
			case NodeState.Selected: {
				ColorComponent.color = SelectedColor;
				if (State == NodeState.Hover) {
					foreach (WeaponNode weaponNode in LinkedWeaponNodes) weaponNode.SetState(NodeState.Selected);
				}
				WeaponNodeSelector.HandleSelect();
				break;
			}
		}
		State = state;
	}
}