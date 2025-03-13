using Project.UI.Loadout.Events;
using UnityEngine;

namespace Project.UI.Loadout.Controllers
{
	/// <summary>
	/// Controller for handling all input in the Loadout UI system.
	/// </summary>
	public class LoadoutInputController : MonoBehaviour
	{
		// Input settings
		[Header("Input Settings")]
		[SerializeField] private float inputCooldown = 0.2f;

		// State tracking
		private bool isInitialized = false;
		private bool isInputEnabled = true;
		private float lastInputTime = 0f;

		void OnEnable()
		{
			// Subscribe to events
			LoadoutEvents.OnAllAnimationsComplete += HandleAnimationsComplete;
			LoadoutEvents.OnLoadoutClosed += HandleLoadoutClosed;
		}

		void OnDisable()
		{
			// Unsubscribe from events
			LoadoutEvents.OnAllAnimationsComplete -= HandleAnimationsComplete;
			LoadoutEvents.OnLoadoutClosed -= HandleLoadoutClosed;
		}

		void Update()
		{
			if (!isInitialized || !isInputEnabled) return;

			// Check for input cooldown
			if (Time.time - lastInputTime < inputCooldown) return;

			// Handle directional input
			Vector2 direction = Vector2.zero;

			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
			{
				direction = Vector2.up;
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
			{
				direction = Vector2.down;
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
			{
				direction = Vector2.left;
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
			{
				direction = Vector2.right;
			}

			if (direction != Vector2.zero)
			{
				LoadoutEvents.TriggerNavigationInput(direction);
				lastInputTime = Time.time;
				return;
			}

			// Handle select input
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				LoadoutEvents.TriggerSelectInput();
				lastInputTime = Time.time;
				return;
			}

			// Handle back input
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
			{
				LoadoutEvents.TriggerBackInput();
				lastInputTime = Time.time;
				return;
			}
		}

		/// <summary>
		/// Initializes the input controller.
		/// </summary>
		public void Initialize()
		{
			isInitialized = false;
			isInputEnabled = false;
			lastInputTime = 0f;
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