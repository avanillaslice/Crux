using System.Collections.Generic;
using UnityEngine;

public class SkillNode {

    // Inspector
    public SkillBase Skill;
    public int Tier;
    public List<SkillConnection> SkillConnectionsIN;
    public List<SkillConnection> SkillConnectionsOUT;

    // State
    public bool IsEnabled;

    public void AttemptSkillActivation() {
        if (IsEnabled) {
            Debug.LogError("Skill already enabled");
            return;
        }

        SkillConnection validSkillConnection = CheckRequirements();
        if (validSkillConnection != null) {
            ShipSkillManager.UnlockSkill(PlayerManager.Inst.ActivePlayerShip, Skill, 1);
            validSkillConnection.Enable();
            Enable();
        }
    }

    private SkillConnection CheckRequirements() {
        // If any IN Connections have a SkillNodeIN that's enabled, return true
        foreach (SkillConnection skillConnection in SkillConnectionsIN) {
            if (skillConnection.SkillNodeOut.IsEnabled) return skillConnection;
        }
        return null;
    }

    private void Enable() {
        // Set Sprite to BRIGHT
        IsEnabled = true;
    }

    private void Disable() {
        // Set Sprite to DIM
        IsEnabled = false;
    }
}