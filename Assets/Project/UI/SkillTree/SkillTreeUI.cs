using System.Collections.Generic;
using UnityEngine;

// SkillTree is a UIWindowBase that handles the skill tree UI.
public class SkillTreeUI : UIWindowBase
{
    public static SkillTreeUI Inst { get; private set; }

    // Inspector
    public List<GameObject> SkillTreeContainers; // Container for skill tree nodes and connections
    public SkillNode InitialCursor;
    
    // State
    private Dictionary<int, List<SkillNode>> Tiers;
    private SkillNode Cursor;
    private List<SkillNode> SkillNodes;
    private List<SkillConnection> SkillConnections;

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Debug.LogError("SkillTree already exists");
            Destroy(gameObject);
            return;
        }
        Inst = this;
        InitializeSkillTrees();
    }

    void OnEnable()
    {
        SetCursor(InitialCursor);
    }

    private void InitializeSkillTrees()
    {
        SkillNodes = new List<SkillNode>();
        SkillConnections = new List<SkillConnection>();
        Tiers = new Dictionary<int, List<SkillNode>>();

        foreach (GameObject skillTreeContainer in SkillTreeContainers) {
            // Temporary lists to hold relevant SkillConnections and SkillNodes
            List<SkillConnection> skillTreeConnections = new List<SkillConnection>();
            List<SkillNode> skillTreeNodes = new List<SkillNode>();

            // Fetch all relevant SkillNodes and SkillConnections
            foreach (Transform child in skillTreeContainer.transform)
            {
                SkillNode skillNode = child.GetComponent<SkillNode>();

                if (skillNode != null) {
                    skillTreeNodes.Add(skillNode);
                    continue;
                }
                SkillConnection skillConnection = child.GetComponent<SkillConnection>();
                if (skillConnection != null) {
                    skillTreeConnections.Add(skillConnection);
                    continue;
                }
            }

            // Initialize relevant SkillNodes
            foreach (SkillNode skillNode in skillTreeNodes) {
                    skillNode.Initialize();

                    // Add to SkillNodes for Refreshing
                    SkillNodes.Add(skillNode);

                    // Add to Tiers for left/right navigation
                    if (!Tiers.ContainsKey(skillNode.Tier)) Tiers[skillNode.Tier] = new List<SkillNode>();
                    Tiers[skillNode.Tier].Add(skillNode);
            }

            // Initialize relevant SkillConnections
            foreach (SkillConnection skillConnection in skillTreeConnections) {
                skillConnection.Initialize(skillTreeNodes);

                // Add to SkillConnections for Refreshing
                SkillConnections.Add(skillConnection);

                // Assign SkillConnections to SkillNodes
                skillConnection.Input.OutputConnections.Add(skillConnection);
                skillConnection.Output.InputConnections.Add(skillConnection);
            }

        }
    }

    public override void HandleMoveLeft()
    {
        SkillNode leftNode = DetermineAppropriateHorizontalNode(true);
        if (leftNode != null && leftNode != Cursor) {
            SetCursor(leftNode);
        }
    }

    public override void HandleMoveRight()
    {
        SkillNode rightNode = DetermineAppropriateHorizontalNode(false);
        if (rightNode != null && rightNode != Cursor) {
            SetCursor(rightNode);
        }
    }

    public override void HandleMoveUp()
    {
        SkillNode upNode = FetchUpSkillNode();
        if (upNode != null && upNode != Cursor) {
            SetCursor(upNode);
        }
    }

    public override void HandleMoveDown()
    {
        SkillNode downNode = FetchDownSkillNode();
        if (downNode != null && downNode != Cursor) {
            SetCursor(downNode);
        }
    }

    public override void HandleSelect()
    {
        if (Cursor == null) {
            Debug.LogError("Cursor is null");
            return;
        };

        Cursor.AttemptSkillActivationOrUpgrade();
    }

    public override void HandleExit()
    {
        UIManager.Inst.DisableSkillTreeUI();
        UIManager.Inst.EnableInterStageUI();
    }

    public override void HandleBack() {}

    // Methods
    public void SetCursor(SkillNode skillNode)
    {
        Cursor?.Deselect();
        Cursor = skillNode;
        Cursor.Select();
    }

    private SkillNode FetchUpSkillNode()
    {
        // No output connections
        if (Cursor.OutputConnections == null || Cursor.OutputConnections.Count == 0) return null; 

        // Combine all potential nodes
        List<SkillNode> potentialNodes = new List<SkillNode>();
        foreach (SkillConnection connection in Cursor.OutputConnections)
        {
            potentialNodes.Add(connection.Output);
        }

        return DetermineAppropriateVerticalNode(potentialNodes);
    }

    private SkillNode FetchDownSkillNode()
    {
        // No input connections
        if (Cursor.InputConnections == null || Cursor.InputConnections.Count == 0) return null;

        // Combine all potential nodes
        List<SkillNode> potentialNodes = new List<SkillNode>();
        foreach (SkillConnection connection in Cursor.InputConnections)
        {
            potentialNodes.Add(connection.Input);
        }
        return DetermineAppropriateVerticalNode(potentialNodes);
    }

    private SkillNode DetermineAppropriateHorizontalNode(bool left) {
        SkillNode appropriateNode = null;
        float minDistance = float.MaxValue;

        foreach (SkillNode potentialNode in Tiers[Cursor.Tier])
        {
            if ((left && potentialNode.XPos < Cursor.XPos) || (!left && potentialNode.XPos > Cursor.XPos))
            {
                float distance = left ? Cursor.XPos - potentialNode.XPos : potentialNode.XPos - Cursor.XPos;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    appropriateNode = potentialNode;
                }
            }
        }
        return appropriateNode;
    }

    private SkillNode DetermineAppropriateVerticalNode(List<SkillNode> potentialNodes) {
        SkillNode appropriateNode = null;
        foreach(SkillNode potentialNode in potentialNodes) {
            if (appropriateNode == null) appropriateNode = potentialNode;

            // If the node is vertically aligned, return it
            if (potentialNode.XPos == Cursor.XPos) {
                appropriateNode = potentialNode;
                break;
            }

            // Choose the leftmost node
            if (potentialNode.XPos < appropriateNode.XPos) {
                appropriateNode = potentialNode;
            }
        }
        return appropriateNode;
    }
}