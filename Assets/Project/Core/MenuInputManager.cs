using UnityEngine;

public class MainMenuInputHandler : MonoBehaviour
{
    private GameControls controls;

    private void Awake()
    {
        controls = new GameControls();
        controls.MenuNavigation.MoveUp.performed += ctx => MoveUp();
        controls.MenuNavigation.MoveDown.performed += ctx => MoveDown();
        controls.MenuNavigation.MoveLeft.performed += ctx => MoveLeft();
        controls.MenuNavigation.MoveRight.performed += ctx => MoveRight();
        controls.MenuNavigation.Select.performed += ctx => Select();
    }

    private void OnEnable()
    {
        controls.MenuNavigation.Enable();
    }

    private void OnDisable()
    {
        controls.MenuNavigation.Disable();
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
