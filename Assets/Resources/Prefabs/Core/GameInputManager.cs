using UnityEngine;

public class GameInputHandler : MonoBehaviour
{
    public static GameInputHandler Inst { get; private set; }
    private GameControls controls;
    public bool WASDEnabled;

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

        controls.MenuNavigation.MoveUp.performed += ctx => MoveUp();
        controls.MenuNavigation.MoveDown.performed += ctx => MoveDown();
        controls.MenuNavigation.MoveLeft.performed += ctx => MoveLeft();
        controls.MenuNavigation.MoveRight.performed += ctx => MoveRight();
        controls.MenuNavigation.Select.performed += ctx => Select();
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

    private void MoveUp()
    {
        UIManager.Inst.HandleMoveUp();
    }

    private void MoveDown()
    {
        UIManager.Inst.HandleMoveDown();
    }

    private void MoveLeft()
    {
        UIManager.Inst.HandleMoveLeft();
    }

    private void MoveRight()
    {
        UIManager.Inst.HandleMoveRight();
    }

    private void Select()
    {
        UIManager.Inst.HandleSelect();
    }
}
