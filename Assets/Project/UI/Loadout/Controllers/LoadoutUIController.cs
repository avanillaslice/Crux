using System.Collections;
using System.Collections.Generic;
using Project.Core;
using Project.Ships;
using Project.UI.Loadout.Events;
using Project.UI.Loadout.Factories;
using Project.UI.Loadout.Model;
using Project.UI.Loadout.Services;
using UnityEngine;

namespace Project.UI.Loadout.Controllers
{
	/// <summary>
	/// Main controller for the Loadout UI system.
	/// </summary>
	public class LoadoutUIController : UIWindowBase
	{
		// Singleton instance
		public static LoadoutUIController Inst { get; private set; }

		// Inspector
		[Header("UI References")]
		[SerializeField] private GameObject weaponUIContainer;

		[Header("Settings")]
		[SerializeField] private float weaponNodeSelectorDistance = 3f;

		// Controllers
		private LoadoutAnimationController animationController;
		private LoadoutInputController inputController;
		private LoadoutNavigationController navigationController;

		// Factory
		private LoadoutFactory factory;

		// State tracking
		private bool isInitializationComplete = false;
		private WeaponNode currentNode;

		// References
		private List<WeaponNode> weaponNodes = new List<WeaponNode>();
		private List<WeaponNodeSelector> weaponNodeSelectors = new List<WeaponNodeSelector>();

		void Awake()
		{
			// Singleton setup
			if (Inst != null && Inst != this)
			{
				Debug.Log("Loadout already exists");
				Destroy(gameObject);
				return;
			}
			Inst = this;

			// Initialize the model
			LoadoutServices.Register(LoadoutModel.Instance);

			// Create controllers
			CreateControllers();
		}

		void OnEnable()
		{
			// Subscribe to events
			LoadoutEvents.OnNodeHoverEnter += HandleNodeHoverEnter;
			LoadoutEvents.OnNodeHoverExit += HandleNodeHoverExit;
			LoadoutEvents.OnNodeSelected += HandleNodeSelected;
			LoadoutEvents.OnNodeDeselected += HandleNodeDeselected;
			LoadoutEvents.OnSelectInput += HandleSelectInput;
			LoadoutEvents.OnBackInput += HandleBackInput;
			LoadoutEvents.OnAllAnimationsComplete += HandleAllAnimationsComplete;

			// Setup player ship
			PlayerManager.Inst.ActivePlayerShip.transform.localScale += new Vector3(1f, 1f, 1f);
			PlayerManager.Inst.ActivePlayerShip.DeactivateShield();

			// Initialize the UI
			InitializeLoadoutUI();
		}

		void OnDisable()
		{
			// Unsubscribe from events
			LoadoutEvents.OnNodeHoverEnter -= HandleNodeHoverEnter;
			LoadoutEvents.OnNodeHoverExit -= HandleNodeHoverExit;
			LoadoutEvents.OnNodeSelected -= HandleNodeSelected;
			LoadoutEvents.OnNodeDeselected -= HandleNodeDeselected;
			LoadoutEvents.OnSelectInput -= HandleSelectInput;
			LoadoutEvents.OnBackInput -= HandleBackInput;
			LoadoutEvents.OnAllAnimationsComplete -= HandleAllAnimationsComplete;

			// Reset player ship
			PlayerManager.Inst.ActivePlayerShip.transform.localScale -= new Vector3(1f, 1f, 1f);
			PlayerManager.Inst.ActivePlayerShip.ActivateShield();

			// Reset cursor state
			if (currentNode != null)
			{
				currentNode.SetState(WeaponNode.NodeState.Default);
				currentNode = null;
			}

			// Clean up UI elements
			CleanUp();

			// Clear events
			LoadoutEvents.ClearAllEvents();
		}

		/// <summary>
		/// Creates the controllers for the Loadout UI.
		/// </summary>
		private void CreateControllers()
		{
			// Create animation controller
			GameObject animControllerObj = new GameObject("LoadoutAnimationController");
			animControllerObj.transform.SetParent(transform);
			animationController = animControllerObj.AddComponent<LoadoutAnimationController>();

			// Create input controller
			GameObject inputControllerObj = new GameObject("LoadoutInputController");
			inputControllerObj.transform.SetParent(transform);
			inputController = inputControllerObj.AddComponent<LoadoutInputController>();

			// Create navigation controller
			GameObject navControllerObj = new GameObject("LoadoutNavigationController");
			navControllerObj.transform.SetParent(transform);
			navigationController = navControllerObj.AddComponent<LoadoutNavigationController>();

			// Create factory
			GameObject factoryObj = new GameObject("LoadoutFactory");
			factoryObj.transform.SetParent(transform);
			factory = factoryObj.AddComponent<LoadoutFactory>();

			// Register controllers with service locator
			LoadoutServices.Register(animationController);
			LoadoutServices.Register(inputController);
			LoadoutServices.Register(navigationController);
			LoadoutServices.Register(factory);
		}

		/// <summary>
		/// Initializes the Loadout UI.
		/// </summary>
		private void InitializeLoadoutUI()
		{
			// Update available weapons
			LoadoutModel.Instance.UpdateAvailableWeapons();

			// Load weapon slots
			LoadoutModel.Instance.LoadActiveWeaponSlots();

			// Process weapon slots to create node data
			LoadoutModel.Instance.ProcessWeaponSlots();

			// Create weapon nodes
			weaponNodes = factory.CreateWeaponNodes();

			if (weaponNodes.Count == 0)
			{
				Debug.LogError("No weapon nodes created");
				return;
			}

			// Create weapon node selectors
			weaponNodeSelectors = factory.CreateWeaponNodeSelectors(weaponNodeSelectorDistance);

			// Link nodes to selectors
			factory.LinkNodesToSelectors();

			// Initialize controllers
			animationController.Initialize(weaponNodes);
			navigationController.Initialize(weaponNodes);
			inputController.Initialize();

			// Start animations
			animationController.PlayStartupAnimations();
		}

		/// <summary>
		/// Handles the completion of all animations.
		/// </summary>
		private void HandleAllAnimationsComplete()
		{
			isInitializationComplete = true;
			LoadoutEvents.TriggerLoadoutInitialized();
			Debug.Log("Loadout UI initialization complete");
		}

		/// <summary>
		/// Cleans up the Loadout UI.
		/// </summary>
		private void CleanUp()
		{
			// Clean up UI elements
			factory.CleanUp();

			// Clear lists
			weaponNodes.Clear();
			weaponNodeSelectors.Clear();

			// Reset state
			isInitializationComplete = false;
			currentNode = null;
		}

		/// <summary>
		/// Sets the current node.
		/// </summary>
		/// <param name="node">The node to set as current</param>
		public void SetCurrentNode(WeaponNode node)
		{
			if (currentNode != null && currentNode != node)
			{
				currentNode.DisableHover();
			}

			currentNode = node;
			currentNode.EnableHover();
		}

		/// <summary>
		/// Handles a node hover enter event.
		/// </summary>
		/// <param name="node">The node being hovered</param>
		private void HandleNodeHoverEnter(WeaponNode node)
		{
			if (!isInitializationComplete) return;
			SetCurrentNode(node);
		}

		/// <summary>
		/// Handles a node hover exit event.
		/// </summary>
		/// <param name="node">The node being exited</param>
		private void HandleNodeHoverExit(WeaponNode node)
		{
			if (!isInitializationComplete) return;

			// Only disable hover if this is the current node
			if (currentNode == node)
			{
				node.DisableHover();
			}
		}

		/// <summary>
		/// Handles a node selected event.
		/// </summary>
		/// <param name="node">The selected node</param>
		private void HandleNodeSelected(WeaponNode node)
		{
			if (!isInitializationComplete) return;
			currentNode = node;
		}

		/// <summary>
		/// Handles a node deselected event.
		/// </summary>
		/// <param name="node">The deselected node</param>
		private void HandleNodeDeselected(WeaponNode node)
		{
			if (!isInitializationComplete) return;

			// Keep the current node as is, but ensure it's in hover state
			if (currentNode == node)
			{
				node.EnableHover();
			}
		}

		/// <summary>
		/// Handles a select input event.
		/// </summary>
		private void HandleSelectInput()
		{
			if (!isInitializationComplete) return;

			// Forward the select action to the current node
			currentNode?.HandleSelect();
		}

		/// <summary>
		/// Handles a back input event.
		/// </summary>
		private void HandleBackInput()
		{
			// If a node is selected, deselect it, otherwise handle normal back behavior
			if (currentNode != null && currentNode.State == WeaponNode.NodeState.Selected)
			{
				currentNode.HandleDeselect();
			}
			else
			{
				// Handle normal back behavior
				HandleExit();
			}
		}

		/// <summary>
		/// Handles the select action.
		/// </summary>
		public override void HandleSelect()
		{
			if (!isInitializationComplete) return;

			// Forward the select action to the current node
			currentNode?.HandleSelect();
		}

		/// <summary>
		/// Handles the back action.
		/// </summary>
		public override void HandleBack()
		{
			HandleBackInput();
		}

		/// <summary>
		/// Handles the exit action.
		/// </summary>
		public override void HandleExit()
		{
			// Trigger the loadout closed event
			LoadoutEvents.TriggerLoadoutClosed();

			// Close the loadout UI
			UIManager.Inst.DisableLoadoutUI();
			UIManager.Inst.EnableInterStageUI();
		}

		/// <summary>
		/// Handles the move left action.
		/// </summary>
		public override void HandleMoveLeft()
		{
			if (!isInitializationComplete) return;

			// Trigger navigation input
			LoadoutEvents.TriggerNavigationInput(Vector2.left);
		}

		/// <summary>
		/// Handles the move right action.
		/// </summary>
		public override void HandleMoveRight()
		{
			if (!isInitializationComplete) return;

			// Trigger navigation input
			LoadoutEvents.TriggerNavigationInput(Vector2.right);
		}

		/// <summary>
		/// Handles the move up action.
		/// </summary>
		public override void HandleMoveUp()
		{
			if (!isInitializationComplete) return;

			// Trigger navigation input
			LoadoutEvents.TriggerNavigationInput(Vector2.up);
		}

		/// <summary>
		/// Handles the move down action.
		/// </summary>
		public override void HandleMoveDown()
		{
			if (!isInitializationComplete) return;

			// Trigger navigation input
			LoadoutEvents.TriggerNavigationInput(Vector2.down);
		}
	}
}