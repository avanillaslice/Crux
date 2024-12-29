using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour {

    // Inspector
    public SkillType SkillType;
    public int Tier;
    public List<SkillConnection> InputConnections;
    public List<SkillConnection> OutputConnections;
    public Color DefaultColor;
    public Color EnabledColor;
    

    // State
    [HideInInspector]
    public float XPos;
    [HideInInspector]
    private bool IsEnabled;
    private bool IsSelected;
    private Color SelectedColor = Color.white;
    private Image imageComponent;

    void Awake() {
        imageComponent = GetComponent<Image>();
        if (imageComponent == null) {
            Debug.LogError("Image component not found on the GameObject.");
        }
    }

    public void AttemptSkillActivation() {
        if (IsEnabled) {
            Debug.LogError("Skill already enabled");
            return;
        }

        SkillConnection validSkillConnection = CheckRequirements();
        if (validSkillConnection != null) {
            ShipSkillManager.UnlockSkill(PlayerManager.Inst.ActivePlayerShip, SkillType, 1);
            validSkillConnection.Enable();
            Enable();
        }
    }

    private SkillConnection CheckRequirements() {
        // If any IN Connections have a SkillNodeIN that's enabled, return true
        foreach (SkillConnection skillConnection in InputConnections) {
            if (skillConnection.SkillNodeOutput.IsEnabled) return skillConnection;
        }
        return null;
    }

    public void Enable() {
        if (IsEnabled) return;
        if (imageComponent == null) Debug.LogError("Image component not found on the GameObject: " + gameObject.name);
        imageComponent.color = EnabledColor;
        IsEnabled = true;
    }

    public void Disable() {
        if (!IsEnabled) return;
        imageComponent.color = IsSelected ? SelectedColor : DefaultColor;
        IsEnabled = false;
    }

    public void Select() {
        if (IsSelected) return;
        imageComponent.color = SelectedColor;
        IsSelected = true;
    }

    public void Deselect() {
        if (!IsSelected) return;
        imageComponent.color = IsEnabled ? EnabledColor : DefaultColor;
        IsSelected = false;
    }
}