using Project.UI.Loadout.Events;
using Project.Core;
using UnityEngine;

namespace Project.UI.Loadout.Controllers
{
	/// <summary>
	/// Controller for handling all input in the Loadout UI system.
	/// </summary>
	public class LoadoutInputController : MonoBehaviour
	{
		// State tracking
		private bool isInitialized = false;
		private bool isInputEnabled = true;

		void OnEnable()
		{
			// Subscribe to Loadout events
			LoadoutEvents.OnAllAnimationsComplete += HandleAnimationsComplete;
			LoadoutEvents.OnLoadoutClosed += HandleLoadoutClosed;

			// Subscribe to input events
			if (GameInputHandler.Inst != null)
			{
				SubscribeToInputEvents();
				GameInputHandler.Inst.EnableMenuNavigationControls();
			}
		}

		void OnDisable()
		{
			// Unsubscribe from Loadout events
			LoadoutEvents.OnAllAnimationsComplete -= HandleAnimationsComplete;
			LoadoutEvents.OnLoadoutClosed -= HandleLoadoutClosed;

			// Unsubscribe from input events
			if (GameInputHandler.Inst != null)
			{
				UnsubscribeFromInputEvents();
				GameInputHandler.Inst.DisableMenuNavigationControls();
			}
		}

		private void SubscribeToInputEvents()
		{
			GameInputHandler.Inst.OnMoveUp += HandleMoveUp;
			GameInputHandler.Inst.OnMoveDown += HandleMoveDown;
			GameInputHandler.Inst.OnMoveLeft += HandleMoveLeft;
			GameInputHandler.Inst.OnMoveRight += HandleMoveRight;
			GameInputHandler.Inst.OnSelect += HandleSelect;
			GameInputHandler.Inst.OnBack += HandleBack;
		}

		private void UnsubscribeFromInputEvents()
		{
			GameInputHandler.Inst.OnMoveUp -= HandleMoveUp;
			GameInputHandler.Inst.OnMoveDown -= HandleMoveDown;
			GameInputHandler.Inst.OnMoveLeft -= HandleMoveLeft;
			GameInputHandler.Inst.OnMoveRight -= HandleMoveRight;
			GameInputHandler.Inst.OnSelect -= HandleSelect;
			GameInputHandler.Inst.OnBack -= HandleBack;
		}

		private void HandleMoveUp()
		{
			if (!isInitialized || !isInputEnabled) return;
			LoadoutEvents.TriggerNavigationInput(Vector2.up);
		}

		private void HandleMoveDown()
		{
			if (!isInitialized || !isInputEnabled) return;
			LoadoutEvents.TriggerNavigationInput(Vector2.down);
		}

		private void HandleMoveLeft()
		{
			if (!isInitialized || !isInputEnabled) return;
			LoadoutEvents.TriggerNavigationInput(Vector2.left);
		}

		private void HandleMoveRight()
		{
			if (!isInitialized || !isInputEnabled) return;
			LoadoutEvents.TriggerNavigationInput(Vector2.right);
		}

		private void HandleSelect()
		{
			if (!isInitialized || !isInputEnabled) return;
			LoadoutEvents.TriggerSelectInput();
		}

		private void HandleBack()
		{
			if (!isInitialized || !isInputEnabled) return;
			LoadoutEvents.TriggerBackInput();
		}

		/// <summary>
		/// Initializes the input controller.
		/// </summary>
		public void Initialize()
		{
			isInitialized = false;
			isInputEnabled = false;
		}

		/// <summary>
		/// Handles the completion of all animations.
		/// </summary>
		private void HandleAnimationsComplete()
		{
			isInitialized = true;
			isInputEnabled = true;
		}

		/// <summary>
		/// Handles the closing of the loadout UI.
		/// </summary>
		private void HandleLoadoutClosed()
		{
			isInputEnabled = false;
		}

		/// <summary>
		/// Enables input handling.
		/// </summary>
		public void EnableInput()
		{
			isInputEnabled = true;
		}

		/// <summary>
		/// Disables input handling.
		/// </summary>
		public void DisableInput()
		{
			isInputEnabled = false;
		}
	}
}