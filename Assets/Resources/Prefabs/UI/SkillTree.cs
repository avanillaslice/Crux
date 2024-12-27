// Use the connecting lines to determine the neighbours.
// How to connect without it being lame?
// Maybe overlap the nodes and connect them that way?

// NODES AND CONNECTIONS WILL BE ASSIGNED VIA PROJECT INSPECTOR

/*

// SkillTree
private class SkillTree
{
    // Inspector
    public List<SkillNode> SkillNodes;

    // State
    private class Dictionary<int, List<SkillNode>> Tiers;

    // Constructor
    private SkillTree(List<SkillNode> skillNodes) { // Can be private constructor?
        Tiers = new Dictionary<int, List<SkillNode>>();
        foreach (SkillNode skillNode in skillNodes) {
            if (Tiers.ContainsKey(skillNode.Tier)) {
                Tiers[skillNode.Tier].Add(skillNode);
            } else {
                Tiers.Add(skillNode.Tier, new List<SkillNode> { skillNode });
            }
        }
    }

    // Methods
    public SkillNode FetchLeftSkillNode(SkillNode skillNode) {
        // Fetch the leftmost skill node in the same tier
        foreach (SkillNode node in Tiers[skillNode.Tier]) {
            if (node.XPos < skillNode.XPos) return node;
        }
        return null;
    }

    public SkillNode FetchRightSkillNode(SkillNode skillNode) {
        // Fetch the rightmost skill node in the same tier
        foreach (SkillNode node in Tiers[skillNode.Tier]) {
            if (node.XPos > skillNode.XPos) return node;
        }
        return null;
    }

    public SkillNode FetchUpSkillNode(SkillNode skillNode) {
        // Fetch the skill node in the tier above
        if (Tiers.ContainsKey(skillNode.Tier + 1)) {
            return Tiers[skillNode.Tier + 1].First();
        }
        return null;
    }

    public SkillNode FetchDownSkillNode(SkillNode skillNode) {
        // Fetch the skill node in the tier below
        if (Tiers.ContainsKey(skillNode.Tier - 1)) {
            return Tiers[skillNode.Tier - 1].First();
        }
        return null;
    }
}

// Inspector
public List<SkillTree> SkillTrees;

// SkillTree
private SkillNode Cursor;
private List

*/

// OnAwake & OnEnable will be used to SetState of nodes and connections
// It will also be used to fetch/assign Descriptions and EffectAmounts for each skill
// It will also set the default cursor position, unsure of best way to do this

// HandleMoveUp, HandleMoveDown, HandleMoveLeft, HandleMoveRight, and HandleSelect will exist

// HandleSelect will trigger skill requirement verification, fetch the PlayerSkillTree, and then attempt to activate the skill

// HandleMoveLeft will subtract 1 from the current cursor row... do i need to fetch coords?
// If another node on the same tier AND in the same SkillTree
// If this node is to the left (A.XPOS > B.XPOS)

// HandleMoveUp will add 1 to the current teir and check if any nodes exist
// If one or more nodes exist, move to the leftmost
