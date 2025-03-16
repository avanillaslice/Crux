using Project.Ships;
using UnityEngine;

namespace Project.Core
{
	public class GameInputHandler : MonoBehaviour
	{
		public static GameInputHandler Inst { get; private set; }
		private GameControls controls;
		public bool WASDEnabled;

		// Add events for menu navigation
		public event System.Action OnMoveUp;
		public event System.Action OnMoveDown;
		public event System.Action OnMoveLeft;
		public event System.Action OnMoveRight;
		public event System.Action OnSelect;
		public event System.Action OnBack;

		private void Awake()
		{
			if (Inst != null && Inst != this)
			{
				Debug.Log("GameInputHandler already exists");
				Destroy(gameObject);
				return;  // Ensure no further code execution in this instance
			}
			Inst = this;

			controls = new GameControls();
			controls.Gameplay.Pause.performed += ctx => TogglePause();
			controls.Gameplay.PrimaryAttack.performed += ctx => OnPrimaryAttackPerformed();
			controls.Gameplay.PrimaryAttack.canceled += ctx => OnPrimaryAttackCanceled();
			controls.Gameplay.SpecialAttack.performed += ctx => OnSpecialAttackPerformed();
			controls.Gameplay.SpecialAttack.canceled += ctx => OnSpecialAttackCanceled();

			// Modify menu navigation to invoke events
			controls.MenuNavigation.MoveUp.performed += ctx => OnMoveUp?.Invoke();
			controls.MenuNavigation.MoveDown.performed += ctx => OnMoveDown?.Invoke();
			controls.MenuNavigation.MoveLeft.performed += ctx => OnMoveLeft?.Invoke();
			controls.MenuNavigation.MoveRight.performed += ctx => OnMoveRight?.Invoke();
			controls.MenuNavigation.Select.performed += ctx => OnSelect?.Invoke();
			controls.MenuNavigation.Back.performed += ctx => OnBack?.Invoke();
		}

		public void EnableGameplayControls()
		{
			Debug.Log("EnablingGameplayControls");
			WASDEnabled = true;
			controls.Gameplay.Enable();
		}

		public void DisableGameplayControls()
		{
			Debug.Log("DisablingGameplayControls");
			WASDEnabled = false;
			controls.Gameplay.Disable();
		}
		public void EnableMenuNavigationControls()
		{
			controls.MenuNavigation.Enable();
		}

		public void DisableMenuNavigationControls()
		{
			controls.MenuNavigation.Disable();
		}

		private void TogglePause()
		{
			GameManager.TogglePause();
		}

		private void OnPrimaryAttackPerformed()
		{
			PlayerShip playerShip = PlayerManager.Inst.ActivePlayerShip;
			if (playerShip != null) playerShip.EnablePrimaryFire();
		}

		private void OnPrimaryAttackCanceled()
		{
			PlayerShip playerShip = PlayerManager.Inst.ActivePlayerShip;
			if (playerShip != null) playerShip.DisablePrimaryFire();
		}

		private void OnSpecialAttackPerformed()
		{
			GameManager.DisableFirstSpecialWeaponUI();
			PlayerShip playerShip = PlayerManager.Inst.ActivePlayerShip;
			if (playerShip != null) playerShip.EnableSpecialFire();
		}

		private void OnSpecialAttackCanceled()
		{
			PlayerShip playerShip = PlayerManager.Inst.ActivePlayerShip;
			if (playerShip != null) playerShip.DisableSpecialFire();
		}
	}
}
