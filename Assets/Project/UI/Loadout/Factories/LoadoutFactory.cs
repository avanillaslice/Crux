using System.Collections.Generic;
using Project.Core;
using Project.Ships;
using Project.UI.Loadout.Model;
using UnityEngine;

namespace Project.UI.Loadout.Factories
{
	/// <summary>
	/// Factory for creating UI elements in the Loadout UI system.
	/// </summary>
	public class LoadoutFactory : MonoBehaviour
	{
		// References
		private Transform uiContainer;

		// Cached lists
		private List<WeaponNode> createdNodes = new List<WeaponNode>();
		private List<WeaponNodeSelector> createdSelectors = new List<WeaponNodeSelector>();

		/// <summary>
		/// Sets the UI container reference.
		/// </summary>
		/// <param name="container">The UI container transform</param>
		public void SetUIContainer(Transform container)
		{
			if (container == null)
			{
				Debug.LogError("Attempting to set null UI container in LoadoutFactory");
				return;
			}

			uiContainer = container;
			Debug.Log($"UI Container set: {uiContainer.name} (Path: {GetFullPath(uiContainer)})");
		}

		/// <summary>
		/// Gets the full path of a transform in the hierarchy.
		/// </summary>
		/// <param name="transform">The transform to get the path for</param>
		/// <returns>The full path of the transform</returns>
		private string GetFullPath(Transform transform)
		{
			string path = transform.name;
			Transform parent = transform.parent;

			while (parent != null)
			{
				path = parent.name + "/" + path;
				parent = parent.parent;
			}

			return path;
		}

		/// <summary>
		/// Creates weapon nodes based on the data in the model.
		/// </summary>
		/// <returns>The list of created weapon nodes</returns>
		public List<WeaponNode> CreateWeaponNodes()
		{
			// Check if UI container is set
			if (uiContainer == null)
			{
				Debug.LogError("UI Container is not set in LoadoutFactory");
				return new List<WeaponNode>();
			}

			// Clear existing nodes
			createdNodes.Clear();

			// Get node data from the model
			List<NodeData> nodeDataList = LoadoutModel.Instance.WeaponNodeDataList;
			List<List<NodeData>> nodeGroups = LoadoutModel.Instance.WeaponNodeGroups;

			// Create nodes for each data entry
			foreach (NodeData nodeData in nodeDataList)
			{
				// Create the node
				WeaponNode weaponNode = CreateWeaponNode(nodeData);
				if (weaponNode != null)
				{
					createdNodes.Add(weaponNode);
				}
			}

			// Set related nodes for each group
			foreach (List<NodeData> group in nodeGroups)
			{
				List<WeaponNode> relatedNodes = new List<WeaponNode>();

				// Find the nodes for this group
				foreach (NodeData nodeData in group)
				{
					foreach (WeaponNode node in createdNodes)
					{
						if (node.NodeId == nodeData.NodeId)
						{
							relatedNodes.Add(node);
							break;
						}
					}
				}

				// Set related nodes for each node in the group
				foreach (WeaponNode node in relatedNodes)
				{
					node.SetRelatedNodes(relatedNodes);
				}
			}

			return createdNodes;
		}

		/// <summary>
		/// Creates a single weapon node.
		/// </summary>
		/// <param name="nodeData">The data for the node</param>
		/// <returns>The created weapon node</returns>
		private WeaponNode CreateWeaponNode(NodeData nodeData)
		{
			if (uiContainer == null)
			{
				Debug.LogError("UI Container is null in CreateWeaponNode");
				return null;
			}

			if (AssetManager.WeaponNodePrefab == null)
			{
				Debug.LogError("WeaponNodePrefab is null in AssetManager");
				return null;
			}

			// Set the Z position
			Vector3 position = nodeData.Position;
			position.z = uiContainer.position.z;

			// Instantiate the node
			GameObject nodeObject = Instantiate(AssetManager.WeaponNodePrefab, position, Quaternion.identity, uiContainer);
			if (nodeObject == null)
			{
				Debug.LogError("Failed to instantiate WeaponNode");
				return null;
			}

			WeaponNode weaponNode = nodeObject.GetComponent<WeaponNode>();
			if (weaponNode == null)
			{
				Debug.LogError("WeaponNode component not found on instantiated prefab");
				return null;
			}

			// Initialize the node
			weaponNode.Init(nodeData.AttachPoint, nodeData.WeaponSlot, nodeData.NodeId);

			return weaponNode;
		}

		/// <summary>
		/// Creates weapon node selectors based on the calculated positions.
		/// </summary>
		/// <param name="selectorDistance">The distance from the center for selectors</param>
		/// <returns>The list of created weapon node selectors</returns>
		public List<WeaponNodeSelector> CreateWeaponNodeSelectors(float selectorDistance)
		{
			// Check if UI container is set
			if (uiContainer == null)
			{
				Debug.LogError("UI Container is not set in LoadoutFactory");
				return new List<WeaponNodeSelector>();
			}

			// Clear existing selectors
			createdSelectors.Clear();

			// Calculate positions for selectors
			List<Vector3> positions = LoadoutModel.Instance.CalculateSelectorPositions(selectorDistance);

			// Create selectors at each position
			foreach (Vector3 position in positions)
			{
				// Set the Z position
				Vector3 selectorPosition = position;
				selectorPosition.z = uiContainer.position.z;

				// Instantiate the selector
				GameObject selectorObject = Instantiate(AssetManager.WeaponNodeSelectorPrefab, selectorPosition, Quaternion.identity, uiContainer);
				if (selectorObject == null)
				{
					Debug.LogError("Failed to instantiate WeaponNodeSelector");
					continue;
				}

				WeaponNodeSelector selector = selectorObject.GetComponent<WeaponNodeSelector>();
				if (selector == null)
				{
					Debug.LogError("WeaponNodeSelector component not found on instantiated prefab");
					continue;
				}

				createdSelectors.Add(selector);
			}

			return createdSelectors;
		}

		/// <summary>
		/// Links weapon nodes to their corresponding selectors.
		/// </summary>
		public void LinkNodesToSelectors()
		{
			if (createdNodes.Count == 0 || createdSelectors.Count == 0)
			{
				Debug.LogError("Cannot link nodes to selectors: no nodes or selectors created");
				return;
			}

			// Determine if the first node is centered
			bool firstNodeIsCentered = createdNodes[0].XPos == 0;

			// Track asymmetrical nodes
			WeaponNode rearAsymmetricalNode = null;

			// Link nodes to selectors
			int nodeCounter = 0;
			int nodeGroupCounter = 0;
			List<List<NodeData>> nodeGroups = LoadoutModel.Instance.WeaponNodeGroups;

			while (nodeCounter < createdNodes.Count)
			{
				foreach (NodeData nodeData in nodeGroups[nodeGroupCounter])
				{
					// Find the corresponding node
					WeaponNode weaponNode = null;
					foreach (WeaponNode node in createdNodes)
					{
						if (node.NodeId == nodeData.NodeId)
						{
							weaponNode = node;
							break;
						}
					}

					if (weaponNode == null)
					{
						Debug.LogError($"Could not find node with ID {nodeData.NodeId}");
						continue;
					}

					// Link based on side
					switch (weaponNode.Side)
					{
						case RelativeSide.Right:
							{
								int index = nodeGroupCounter - (rearAsymmetricalNode == null ? 0 : 1);
								if (index >= 0 && index < createdSelectors.Count)
								{
									weaponNode.AssignSelector(createdSelectors[index]);
								}
								break;
							}
						case RelativeSide.Left:
							{
								int index = createdNodes.Count - nodeGroupCounter + (rearAsymmetricalNode == null ? 0 : 1) - (firstNodeIsCentered ? 0 : 1);
								if (index >= 0 && index < createdSelectors.Count)
								{
									weaponNode.AssignSelector(createdSelectors[index]);
								}
								break;
							}
						case RelativeSide.Center:
							{
								if (nodeGroupCounter == 0)
								{
									if (createdSelectors.Count > 0)
									{
										weaponNode.AssignSelector(createdSelectors[0]);
									}
								}
								else
								{
									if (rearAsymmetricalNode != null)
									{
										int rearIndex = createdNodes.Count - nodeGroupCounter;
										if (rearIndex >= 0 && rearIndex < createdSelectors.Count)
										{
											rearAsymmetricalNode.AssignSelector(createdSelectors[rearIndex]);
										}

										if (nodeGroupCounter < createdSelectors.Count)
										{
											weaponNode.AssignSelector(createdSelectors[nodeGroupCounter]);
										}

										rearAsymmetricalNode = null;
									}
									else
									{
										rearAsymmetricalNode = weaponNode;
									}
								}
								break;
							}
					}

					nodeCounter++;
				}

				nodeGroupCounter++;
			}

			// Handle any remaining asymmetrical node
			if (rearAsymmetricalNode != null)
			{
				int remainingIndex = FindRemainingSelectorIndex();
				if (remainingIndex != -1)
				{
					rearAsymmetricalNode.AssignSelector(createdSelectors[remainingIndex]);
				}
				else
				{
					Debug.LogError("Could not find remaining selector for asymmetrical node");
				}
			}
		}

		/// <summary>
		/// Finds the index of a selector that hasn't been assigned to a node.
		/// </summary>
		/// <returns>The index of the remaining selector, or -1 if none found</returns>
		private int FindRemainingSelectorIndex()
		{
			foreach (WeaponNodeSelector selector in createdSelectors)
			{
				if (selector.ID.text == "abc")
				{
					return createdSelectors.IndexOf(selector);
				}
			}

			return -1;
		}

		/// <summary>
		/// Cleans up all created UI elements.
		/// </summary>
		public void CleanUp()
		{
			foreach (WeaponNode node in createdNodes)
			{
				if (node != null)
				{
					Destroy(node.gameObject);
				}
			}

			foreach (WeaponNodeSelector selector in createdSelectors)
			{
				if (selector != null)
				{
					Destroy(selector.gameObject);
				}
			}

			createdNodes.Clear();
			createdSelectors.Clear();
		}
	}
}