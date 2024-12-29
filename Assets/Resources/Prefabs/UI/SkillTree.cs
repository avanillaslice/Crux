using System.Collections.Generic;
using UnityEngine;

// NODES AND CONNECTIONS WILL BE ASSIGNED VIA PROJECT INSPECTOR
public class SkillTree : UIWindowBase
{
    public static SkillTree Inst { get; private set; }

    // Inspector
    public GameObject SkillNodeContainer; // Container for skill nodes
    public List<SkillNode> SkillNodes;
    public SkillNode InitialCursor;

    // State
    private Dictionary<int, List<SkillNode>> Tiers;
    private SkillNode Cursor;

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Debug.Log("SkillTree already exists");
            Destroy(gameObject);
            return;
        }
        Inst = this;
        InitializeSkillNodes();
        RefreshSkillNodes();
        SetCursor(InitialCursor);
    }

    void OnEnable()
    {
        RefreshSkillNodes();
        SetCursor(InitialCursor);
    }

    void OnDisable()
    {
        // Any cleanup if necessary
    }

    private void InitializeSkillNodes()
    {
        SkillNodes = new List<SkillNode>();
        Tiers = new Dictionary<int, List<SkillNode>>();

        foreach (Transform child in SkillNodeContainer.transform)
        {
            SkillNode skillNode = child.GetComponent<SkillNode>();
            if (skillNode != null)
            {
                skillNode.XPos = child.localPosition.x; // Set XPos based on local position
                SkillNodes.Add(skillNode);

                if (!Tiers.ContainsKey(skillNode.Tier))
                {
                    Tiers[skillNode.Tier] = new List<SkillNode>();
                }
                Tiers[skillNode.Tier].Add(skillNode);
            }
        }
    }

    private void RefreshSkillNodes()
    {
        ShipSkillManager.ShipSkills shipSkills = PlayerManager.Inst.ActivePlayerShip.ActiveSkills;
        foreach (SkillNode skillNode in SkillNodes) {
            if (shipSkills.Skills.ContainsKey(skillNode.SkillType)) {
                skillNode.Enable();
            } else {
                skillNode.Disable();
            }
        }
    }

    public override void HandleMoveLeft()
    {
        SkillNode leftNode = FetchLeftSkillNode(Cursor);
        if (leftNode != null && leftNode != Cursor) {
            SetCursor(leftNode);
        }
    }

    public override void HandleMoveRight()
    {
        SkillNode rightNode = FetchRightSkillNode(Cursor);
        if (rightNode != null && rightNode != Cursor) {
            SetCursor(rightNode);
        }
    }

    public override void HandleMoveUp()
    {
        SkillNode upNode = FetchUpSkillNode(Cursor);
        if (upNode != null && upNode != Cursor) {
            SetCursor(upNode);
        }
    }

    public override void HandleMoveDown()
    {
        SkillNode downNode = FetchDownSkillNode(Cursor);
        if (downNode != null && downNode != Cursor) {
            SetCursor(downNode);
        }
    }

    public override void HandleSelect()
    {
        Cursor?.AttemptSkillActivation();
    }

    public override void HandleBackClicked()
    {
        UIManager.Inst.DisableSkillTreeUI();
        UIManager.Inst.EnableInterStageUI();
    }

    // Methods
    public void SetCursor(SkillNode skillNode)
    {
        Cursor?.Deselect();
        Cursor = skillNode;
        Cursor.Select();
    }

    public SkillNode FetchLeftSkillNode(SkillNode skillNode)
    {
        SkillNode leftmostNode = null;
        foreach (SkillNode node in Tiers[skillNode.Tier])
        {
            if (node.XPos < skillNode.XPos)
            {
                if (leftmostNode == null || node.XPos < leftmostNode.XPos)
                {
                    leftmostNode = node;
                }
            }
        }
        return leftmostNode;
    }

    public SkillNode FetchRightSkillNode(SkillNode skillNode)
    {
        SkillNode rightmostNode = null;
        foreach (SkillNode node in Tiers[skillNode.Tier])
        {
            if (node.XPos > skillNode.XPos)
            {
                if (rightmostNode == null || node.XPos > rightmostNode.XPos)
                {
                    rightmostNode = node;
                }
            }
        }
        return rightmostNode;
    }

    public SkillNode FetchUpSkillNode(SkillNode skillNode)
    {
        if (Tiers.ContainsKey(skillNode.Tier + 1))
        {
            SkillNode leftmostNode = null;
            foreach (SkillNode node in Tiers[skillNode.Tier + 1])
            {
                if (leftmostNode == null || node.XPos < leftmostNode.XPos)
                {
                    leftmostNode = node;
                }
            }
            return leftmostNode;
        }
        return null;
    }

    public SkillNode FetchDownSkillNode(SkillNode skillNode)
    {
        if (Tiers.ContainsKey(skillNode.Tier - 1))
        {
            SkillNode leftmostNode = null;
            foreach (SkillNode node in Tiers[skillNode.Tier - 1])
            {
                if (leftmostNode == null || node.XPos < leftmostNode.XPos)
                {
                    leftmostNode = node;
                }
            }
            return leftmostNode;
        }
        return null;
    }
}

// Inspector
//public List<SkillTree> SkillTrees;

// SkillTree
//private SkillNode Cursor;
//private List


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
