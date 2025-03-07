using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.SkillTree
{
    public class SkillConnection : MonoBehaviour {

        // Inspector
        public int PrerequisiteLevel;
        public Color EnabledColor;
        public Color DefaultColor = Color.white;
        public GameObject PortA;
        public GameObject PortB;

        // State
        [HideInInspector] // Can probably just be private
        public bool IsEnabled;
        [HideInInspector]
        public SkillNode Input;
        [HideInInspector]
        public SkillNode Output;
        private Image ImageComponent;

        public void Initialize(List<SkillNode> skillNodes)
        {
            ImageComponent = GetComponent<Image>();
            SetInputAndOutputNodes(skillNodes);
            if (Input.SkillNodeActive && Output.SkillNodeActive) Enable();
        }

        private void SetInputAndOutputNodes(List<SkillNode> skillNodes) {
            SkillNode nodeA = FindClosestSkillNode(skillNodes, PortA.transform.position);
            SkillNode nodeB = FindClosestSkillNode(skillNodes, PortB.transform.position);

            if (nodeA == null || nodeB == null)
            {
                Debug.LogError($"Unable to find SkillNodes for connection: {gameObject.name}");
                return;
            }

            // Determine input and output nodes based on YPos
            if (nodeA.transform.position.y < nodeB.transform.position.y)
            {
                Input = nodeA;
                Output = nodeB;
            }
            else
            {
                Input = nodeB;
                Output = nodeA;
            }
        }

        private SkillNode FindClosestSkillNode(List<SkillNode> skillNodes, Vector3 portGlobalPosition)
        {
            SkillNode closestNode = null;
            float minDistance = 1f; // Limit for the search

            foreach (SkillNode skillNode in skillNodes)
            {
                float distance = Vector3.Distance(portGlobalPosition, skillNode.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNode = skillNode;
                }
            }

            return closestNode;
        }

        public void Enable() {
            if (IsEnabled) return;
            ImageComponent.color = EnabledColor;
            IsEnabled = true;
        }

        public void Disable() {
            if (!IsEnabled) return;
            ImageComponent.color = DefaultColor;
            IsEnabled = false;
        }
    }
}