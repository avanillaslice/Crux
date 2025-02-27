using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour {

    // Inspector
    public SkillType SkillType;
    public int Tier;
    public List<SkillConnection> InputConnections;
    public List<SkillConnection> OutputConnections;
    public Image BaseColorComponent;
    

    // State
    [HideInInspector]
    public float XPos;
    [HideInInspector]
    public bool SkillNodeActive;
    private SkillBase ActiveSkill;
    private bool IsSelected;
    private Color DisabledColor;
    private Color EnabledColor = Color.yellow;
    private Color SelectedColor = Color.white;
    private Color DeselectedColor;
    private Image BorderColorImageComponent;
    
    public void Initialize() {
        BorderColorImageComponent = GetComponent<Image>();
        DisabledColor = BorderColorImageComponent.color;
        DeselectedColor = BaseColorComponent.color;
        XPos = transform.position.x;
        SetActiveSkill();
    }

    private void SetActiveSkill() {
        ActiveSkill = PlayerManager.Inst.ActivePlayerShip.ActiveSkills.FetchSkill(SkillType);
        if (ActiveSkill != null) {
            Enable();
        }
    }

    public void AttemptSkillActivationOrUpgrade() {
        // Attempt to Upgrade
        if (SkillNodeActive) {
            if (AllowedToUpgrade()) {
                try {
                    ActiveSkill.Upgrade();
                } catch (Exception e) {
                    Debug.Log($"{SkillType}: {e.Message}");
                }
            } else {
                Debug.Log("Skill already enabled");
            }
            return;
        }

        // Attempt to Activate
        if (AllowedToActivate()) {
            ActiveSkill = ShipSkillManager.UnlockSkill(PlayerManager.Inst.ActivePlayerShip, SkillType, 1);

            if (ActiveSkill == null) return;
            
            Enable();
            EnableValidConnections();
            return;
        }

        Debug.Log("Skill requirements not met");
    }

    private void EnableValidConnections() {
        List<SkillConnection> allConnections = new List<SkillConnection>();
        allConnections.AddRange(InputConnections);
        allConnections.AddRange(OutputConnections);

        foreach (SkillConnection skillConnection in allConnections) {
            if (skillConnection.IsEnabled) continue;
            if (skillConnection.Input.SkillNodeActive && skillConnection.Output.SkillNodeActive) {
                skillConnection.Enable();
            }
        }
    }

    private bool AllowedToUpgrade() {
        if (ActiveSkill == null) {
            Debug.LogError($"ActiveSkill is null");
            return false;
        }
        // Upgrade Cost Logic goes here
        return true;
    }

    private bool AllowedToActivate() {
        if (InputConnections.Count == 0) {
            // Activate Cost Logic goes here
            return true;
        }

        foreach (SkillConnection inputConnection in InputConnections) {
            SkillNode requiredSkillNode = inputConnection.Input;
            // Activate Cost AND Prerequisite Logic goes here
            if (requiredSkillNode.SkillNodeActive && requiredSkillNode.ActiveSkill.Level >= inputConnection.PrerequisiteLevel) return true;
        }

        return false;
    }

    public void Enable() {
        if (SkillNodeActive) return;
        BorderColorImageComponent.color = EnabledColor;
        SkillNodeActive = true;
    }

    public void Disable() {
        if (!SkillNodeActive) return;
        BorderColorImageComponent.color = DisabledColor;
        SkillNodeActive = false;
    }

    public void Select() {
        if (IsSelected) return;
        BaseColorComponent.color = SelectedColor;
        IsSelected = true;
    }

    public void Deselect() {
        if (!IsSelected) return;
        BaseColorComponent.color = DeselectedColor;
        IsSelected = false;
    }
}