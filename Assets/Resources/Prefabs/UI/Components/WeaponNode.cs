using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class WeaponNode : MonoBehaviour
{
	// Inspector
	public Color HoverColor;
	public Color SelectedColor;
	public TextMeshProUGUI ID;

	// Data
	private bool Initialised = false;
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
		ID.text = NodeId.ToString();
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
		Initialised = true;
	}

	public void SetRelatedNodes(List<WeaponNode> weaponNodes)
	{
		foreach (WeaponNode weaponNode in weaponNodes)
		{
			if (weaponNode != this) LinkedWeaponNodes.Add(weaponNode);
		}
	}

	public void HandlePointerEnter() {
		if (!Initialised) return;
		LoadoutUI.Inst.HandlePointerEnterOnNode(this);
	}

	public void HandlePointerExit() {
		if (!Initialised) return;
		LoadoutUI.Inst.HandlePointerExitOnNode(this);
	}

	public void HandlePointerClick() {
		if (!Initialised) return;
		LoadoutUI.Inst.HandlePointerClickOnNode(this);
	}

	public void EnableHover()
	{
		if (!Initialised) return;
		if (State == NodeState.Hover || State == NodeState.Selected) return;
		SetState(NodeState.Hover);
	}

	public void DisableHover()
	{
		if (!Initialised) return;
		if (State != NodeState.Hover || State == NodeState.Selected) return;
		SetState(NodeState.Default);
	}
	
	public void HandleSelect()
	{
		if (!Initialised) return;
		if (State != NodeState.Hover) return;
		SetState(NodeState.Selected);
		WeaponNodeSelector.HandleSelect();
	}

	public void HandleDeselect()
	{
		if (!Initialised) return;
		if (State != NodeState.Selected) return;
		SetState(NodeState.Hover);
		WeaponNodeSelector.HandleDeselect();
	}

	internal void SetState(NodeState state)
	{
		if (!Initialised) return;
		switch (state)
		{
			case NodeState.Default: {
				ColorComponent.color = DefaultColor;
				if (State == NodeState.Hover) WeaponNodeSelector.DisableHoverState();
				if (State == NodeState.Selected) WeaponNodeSelector.HandleDeselect();
				break;
			}
			case NodeState.Hover: {
				if (State == NodeState.Selected) {
					WeaponNodeSelector.HandleDeselect();
					foreach (WeaponNode weaponNode in LinkedWeaponNodes) weaponNode.SetState(NodeState.Default);
				}
				ColorComponent.color = HoverColor;
				WeaponNodeSelector.EnableHoverState();
				break;
			}
			case NodeState.Selected: {
				ColorComponent.color = SelectedColor;
				if (State == NodeState.Hover) {
					foreach (WeaponNode weaponNode in LinkedWeaponNodes) weaponNode.SetState(NodeState.Selected);
				}
				break;
			}
		}
		State = state;
	}
}