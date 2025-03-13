using System.Collections.Generic;
using Project.UI.Loadout.Events;
using UnityEngine;

namespace Project.UI.Loadout.Controllers
{
	/// <summary>
	/// Controller for handling navigation between nodes in the Loadout UI system.
	/// </summary>
	public class LoadoutNavigationController : MonoBehaviour
	{
		// References
		private List<WeaponNode> weaponNodes = new List<WeaponNode>();
		private WeaponNode currentNode;

		// State tracking
		private bool isInitialized = false;

		void OnEnable()
		{
			// Subscribe to events
			LoadoutEvents.OnNavigationInput += HandleNavigationInput;
			LoadoutEvents.OnAllAnimationsComplete += HandleAnimationsComplete;
			LoadoutEvents.OnNodeSelected += HandleNodeSelected;
			LoadoutEvents.OnNodeDeselected += HandleNodeDeselected;
		}

		void OnDisable()
		{
			// Unsubscribe from events
			LoadoutEvents.OnNavigationInput -= HandleNavigationInput;
			LoadoutEvents.OnAllAnimationsComplete -= HandleAnimationsComplete;
			LoadoutEvents.OnNodeSelected -= HandleNodeSelected;
			LoadoutEvents.OnNodeDeselected -= HandleNodeDeselected;
		}

		/// <summary>
		/// Initializes the navigation controller with the list of weapon nodes.
		/// </summary>
		/// <param name="nodes">The list of weapon nodes to navigate between</param>
		public void Initialize(List<WeaponNode> nodes)
		{
			weaponNodes = nodes;
			isInitialized = false;

			if (nodes.Count > 0)
			{
				currentNode = nodes[0];
			}
		}

		/// <summary>
		/// Handles the completion of all animations.
		/// </summary>
		private void HandleAnimationsComplete()
		{
			isInitialized = true;

			// Set the initial node as the current node
			if (currentNode != null)
			{
				SetCurrentNode(currentNode);
			}
		}

		/// <summary>
		/// Handles navigation input.
		/// </summary>
		/// <param name="direction">The direction of navigation</param>
		private void HandleNavigationInput(Vector2 direction)
		{
			if (!isInitialized || currentNode == null) return;

			// If a node is selected, handle scrolling in the list
			if (currentNode.State == WeaponNode.NodeState.Selected)
			{
				HandleListNavigation(direction);
				return;
			}

			// Otherwise, navigate between nodes
			WeaponNode targetNode = null;

			if (direction.y > 0)
			{
				// Up
				targetNode = FindNodeInDirection(true, true);
			}
			else if (direction.y < 0)
			{
				// Down
				targetNode = FindNodeInDirection(false, true);
			}
			else if (direction.x < 0)
			{
				// Left
				targetNode = FindNodeInDirection(false, false);
			}
			else if (direction.x > 0)
			{
				// Right
				targetNode = FindNodeInDirection(true, false);
			}

			if (targetNode != null)
			{
				SetCurrentNode(targetNode);
			}
		}

		/// <summary>
		/// Handles navigation within a weapon list.
		/// </summary>
		/// <param name="direction">The direction of navigation</param>
		private void HandleListNavigation(Vector2 direction)
		{
			if (currentNode.WeaponNodeSelector == null) return;

			if (direction.y > 0)
			{
				// Up
				currentNode.WeaponNodeSelector.ScrollUp();
			}
			else if (direction.y < 0)
			{
				// Down
				currentNode.WeaponNodeSelector.ScrollDown();
			}
		}

		/// <summary>
		/// Finds a node in the specified direction.
		/// </summary>
		/// <param name="positive">True for up/right, false for down/left</param>
		/// <param name="vertical">True for vertical, false for horizontal</param>
		/// <returns>The found node, or null if none found</returns>
		private WeaponNode FindNodeInDirection(bool positive, bool vertical)
		{
			if (vertical)
			{
				return FindNodeVertical(positive);
			}
			else
			{
				return FindNodeHorizontal(positive);
			}
		}

		/// <summary>
		/// Finds a node in the vertical direction.
		/// </summary>
		/// <param name="up">True for up, false for down</param>
		/// <returns>The found node, or null if none found</returns>
		private WeaponNode FindNodeVertical(bool up)
		{
			if (currentNode.XPos == 0)
			{
				// If on center line, find node directly above/below
				foreach (WeaponNode node in weaponNodes)
				{
					if (node.XPos == 0 && ((up && node.YPos > currentNode.YPos) || (!up && node.YPos < currentNode.YPos)))
					{
						return node;
					}
				}
			}

			// Otherwise, use index-based navigation
			int currentIndex = weaponNodes.IndexOf(currentNode);

			if (currentNode.XPos < 0 && currentIndex + 1 < weaponNodes.Count)
			{
				return weaponNodes[currentIndex + 1];
			}
			else if (currentNode.XPos > 0 && currentIndex - 1 >= 0)
			{
				return weaponNodes[currentIndex - 1];
			}

			// Fallback to first node
			return weaponNodes[0];
		}

		/// <summary>
		/// Finds a node in the horizontal direction.
		/// </summary>
		/// <param name="right">True for right, false for left</param>
		/// <returns>The found node, or null if none found</returns>
		private WeaponNode FindNodeHorizontal(bool right)
		{
			if (currentNode.YPos == 0)
			{
				// If on horizontal line, find node directly left/right
				foreach (WeaponNode node in weaponNodes)
				{
					if (node.YPos == 0 && ((right && node.XPos > currentNode.XPos) || (!right && node.XPos < currentNode.XPos)))
					{
						return node;
					}
				}
			}

			// Otherwise, use index-based navigation
			int currentIndex = weaponNodes.IndexOf(currentNode);

			if (currentNode.YPos < 0 && currentIndex + 1 < weaponNodes.Count)
			{
				return weaponNodes[currentIndex + 1];
			}
			else if (currentNode.YPos > 0 && currentIndex - 1 >= 0)
			{
				return weaponNodes[currentIndex - 1];
			}

			// Fallback to first node
			return weaponNodes[0];
		}

		/// <summary>
		/// Sets the current node and updates its state.
		/// </summary>
		/// <param name="node">The node to set as current</param>
		private void SetCurrentNode(WeaponNode node)
		{
			if (currentNode != null && currentNode != node)
			{
				// If the current node is not selected, disable its hover state
				if (currentNode.State != WeaponNode.NodeState.Selected)
				{
					currentNode.DisableHover();
				}
			}

			currentNode = node;

			// If the new current node is not selected, enable its hover state
			if (currentNode.State != WeaponNode.NodeState.Selected)
			{
				currentNode.EnableHover();
			}
		}

		/// <summary>
		/// Handles a node being selected.
		/// </summary>
		/// <param name="node">The selected node</param>
		private void HandleNodeSelected(WeaponNode node)
		{
			// Update the current node
			currentNode = node;
		}

		/// <summary>
		/// Handles a node being deselected.
		/// </summary>
		/// <param name="node">The deselected node</param>
		private void HandleNodeDeselected(WeaponNode node)
		{
			// Keep the current node as is, but ensure it's in hover state
			if (currentNode == node)
			{
				currentNode.EnableHover();
			}
		}

		/// <summary>
		/// Gets the current node.
		/// </summary>
		/// <returns>The current node</returns>
		public WeaponNode GetCurrentNode()
		{
			return currentNode;
		}
	}
}