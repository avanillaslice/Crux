using System;
using Project.Combat.Weapons;
using Project.UI.Loadout.Model;
using UnityEngine;

namespace Project.UI.Loadout.Events
{
	/// <summary>
	/// Central event system for the Loadout UI. Handles all event-based communication between components.
	/// </summary>
	public static class LoadoutEvents
	{
		// Node events
		public static event Action<WeaponNode> OnNodeHoverEnter;
		public static event Action<WeaponNode> OnNodeHoverExit;
		public static event Action<WeaponNode> OnNodeSelected;
		public static event Action<WeaponNode> OnNodeDeselected;

		// Selector events
		public static event Action<WeaponNodeSelector> OnSelectorHoverEnter;
		public static event Action<WeaponNodeSelector> OnSelectorHoverExit;
		public static event Action<WeaponNodeSelector> OnSelectorSelected;
		public static event Action<WeaponNodeSelector> OnSelectorDeselected;

		// List events
		public static event Action<WeaponNodeSelectorList> OnListActivated;
		public static event Action<WeaponNodeSelectorList> OnListDeactivated;
		public static event Action<WeaponNodeSelectorList, int> OnListScrolled;

		// Weapon events
		public static event Action<WeaponBase> OnWeaponSelected;
		public static event Action<WeaponBase, string> OnWeaponEquipped;

		// Animation events
		public static event Action<WeaponNode> OnNodeAnimationComplete;
		public static event Action<WeaponNode> OnConnectionLineDrawComplete;
		public static event Action OnAllAnimationsComplete;

		// Navigation events
		public static event Action<Vector2> OnNavigationInput;
		public static event Action OnSelectInput;
		public static event Action OnBackInput;

		// System events
		public static event Action OnLoadoutInitialized;
		public static event Action OnLoadoutClosed;

		// Event trigger methods
		public static void TriggerNodeHoverEnter(WeaponNode node)
		{
			OnNodeHoverEnter?.Invoke(node);
		}

		public static void TriggerNodeHoverExit(WeaponNode node)
		{
			OnNodeHoverExit?.Invoke(node);
		}

		public static void TriggerNodeSelected(WeaponNode node)
		{
			OnNodeSelected?.Invoke(node);
		}

		public static void TriggerNodeDeselected(WeaponNode node)
		{
			OnNodeDeselected?.Invoke(node);
		}

		public static void TriggerSelectorHoverEnter(WeaponNodeSelector selector)
		{
			OnSelectorHoverEnter?.Invoke(selector);
		}

		public static void TriggerSelectorHoverExit(WeaponNodeSelector selector)
		{
			OnSelectorHoverExit?.Invoke(selector);
		}

		public static void TriggerSelectorSelected(WeaponNodeSelector selector)
		{
			OnSelectorSelected?.Invoke(selector);
		}

		public static void TriggerSelectorDeselected(WeaponNodeSelector selector)
		{
			OnSelectorDeselected?.Invoke(selector);
		}

		public static void TriggerListActivated(WeaponNodeSelectorList list)
		{
			OnListActivated?.Invoke(list);
		}

		public static void TriggerListDeactivated(WeaponNodeSelectorList list)
		{
			OnListDeactivated?.Invoke(list);
		}

		public static void TriggerListScrolled(WeaponNodeSelectorList list, int direction)
		{
			OnListScrolled?.Invoke(list, direction);
		}

		public static void TriggerWeaponSelected(WeaponBase weapon)
		{
			OnWeaponSelected?.Invoke(weapon);
		}

		public static void TriggerWeaponEquipped(WeaponBase weapon, string slotId)
		{
			OnWeaponEquipped?.Invoke(weapon, slotId);
		}

		public static void TriggerNodeAnimationComplete(WeaponNode node)
		{
			OnNodeAnimationComplete?.Invoke(node);
		}

		public static void TriggerConnectionLineDrawComplete(WeaponNode node)
		{
			OnConnectionLineDrawComplete?.Invoke(node);
		}

		public static void TriggerAllAnimationsComplete()
		{
			OnAllAnimationsComplete?.Invoke();
		}

		public static void TriggerNavigationInput(Vector2 direction)
		{
			OnNavigationInput?.Invoke(direction);
		}

		public static void TriggerSelectInput()
		{
			OnSelectInput?.Invoke();
		}

		public static void TriggerBackInput()
		{
			OnBackInput?.Invoke();
		}

		public static void TriggerLoadoutInitialized()
		{
			OnLoadoutInitialized?.Invoke();
		}

		public static void TriggerLoadoutClosed()
		{
			OnLoadoutClosed?.Invoke();
		}

		// Clear all events (useful when closing the loadout UI)
		public static void ClearAllEvents()
		{
			OnNodeHoverEnter = null;
			OnNodeHoverExit = null;
			OnNodeSelected = null;
			OnNodeDeselected = null;
			OnSelectorHoverEnter = null;
			OnSelectorHoverExit = null;
			OnSelectorSelected = null;
			OnSelectorDeselected = null;
			OnListActivated = null;
			OnListDeactivated = null;
			OnListScrolled = null;
			OnWeaponSelected = null;
			OnWeaponEquipped = null;
			OnNodeAnimationComplete = null;
			OnConnectionLineDrawComplete = null;
			OnAllAnimationsComplete = null;
			OnNavigationInput = null;
			OnSelectInput = null;
			OnBackInput = null;
			OnLoadoutInitialized = null;
			OnLoadoutClosed = null;
		}
	}
}