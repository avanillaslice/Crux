using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Inst { get; private set; }

    // CANVAS
    public GameObject GameplayOverlayCanvas;
    public GameObject InterStageUICanvas;

    // UI PREFABS
    public GameObject MainMenuUI;
    private GameObject ShipSelectionUI;
    private GameObject PauseMenuUI;
    private GameObject InterStageUI;
    private GameObject LoadoutUI;
    private GameObject SpecialWeaponUnlockedUI;

    // UTILITY
    private string ActiveWindow;

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Debug.Log("UIManager already exists");
            Destroy(gameObject);
            return;  // Ensure no further code execution in this instance
        }
        Inst = this;
    }

    void Start()
    {
        ShipSelectionUI = Instantiate(AssetManager.ShipSelectionUIPrefab);
        ShipSelectionUI.SetActive(false);
    }

    public void TransitionToShipSelection()
    {
        if (MainMenuUI == null) 
        {
            Debug.LogError("MainMenuUI is null");
            return;
        }
        MainMenuUI.SetActive(false);
        ShipSelectionUI.SetActive(true);
        ActiveWindow = "ShipSelection";
    }

    public void TransitionToLoadout()
    {
        DisableInterStageUI();
        EnableLoadoutUI();
    }

    public void EnableSpecialWeaponUnlockedUI()
    {
        SpecialWeaponUnlockedUI = Instantiate(AssetManager.SpecialWeaponUnlockedPrefab, GameplayOverlayCanvas.transform);
        ActiveWindow = "SpecialWeaponUnlocked";

        // Find the Animator component in the child GameObject
        Animator animator = SpecialWeaponUnlockedUI.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        else
        {
            Debug.LogWarning("Animator component not found in the SpecialWeaponUnlockedUI prefab.");
        }
    }

    public void DisableSpecialWeaponUnlockedUI()
    {
        Destroy(SpecialWeaponUnlockedUI);
        ActiveWindow = null;
    }

    public void EnableInterStageUI()
    {
        if (InterStageUI == null) InterStageUI = Instantiate(AssetManager.InterStageUIPrefab, InterStageUICanvas.transform);
        else InterStageUI.SetActive(true);
        ActiveWindow = "InterStage";
    }

    public void DisableInterStageUI()
    {
        if (InterStageUI == null) return;
        else InterStageUI.SetActive(false);
        ActiveWindow = null;
    }

    private void EnableLoadoutUI()
    {
        if (LoadoutUI == null) LoadoutUI = Instantiate(AssetManager.LoadoutUIPrefab, InterStageUICanvas.transform);
        else LoadoutUI.SetActive(true);
        ActiveWindow = "Loadout";
    }

    public void DisableLoadoutUI()
    {
        if (LoadoutUI == null) return;
        else LoadoutUI.SetActive(false);
        ActiveWindow = null;
    }

    // Menu Controls...
    /*
    Could potentially have it attempt to send HandleMove to all menus
    Only the active one will receive. Although could cause annoying issues..
    */
    public void HandleMoveUp()
    {
        switch (ActiveWindow)
        {
            case "Loadout":
                LoadoutUI.GetComponent<Loadout>().HandleMoveUp();
                break;
        }
    }

    public void HandleMoveDown()
    {
        switch (ActiveWindow)
        {
            case "Loadout":
                LoadoutUI.GetComponent<Loadout>().HandleMoveDown();
                break;
        }
    }

    public void HandleMoveLeft()
    {
        switch (ActiveWindow)
        {
            case "Loadout":
                // LoadoutUI.GetComponent<Loadout>().HandleMoveLeft();
                break;
            case "ShipSelection":
                ShipSelectionUI.GetComponent<ShipSelection>().MoveCursorLeft();
                break;
        }
        // if (ShipSelectionUI.activeSelf)
        // {
        //     ShipSelectionUI.GetComponent<ShipSelection>().MoveCursorLeft();
        // }
    }

    public void HandleMoveRight()
    {
        switch (ActiveWindow)
        {
            case "Loadout":
                // LoadoutUI.GetComponent<Loadout>().HandleMoveRight();
                break;
            case "ShipSelection":
                ShipSelectionUI.GetComponent<ShipSelection>().MoveCursorRight();
                break;
        }
    }

    public void HandleSelect()
    {
        switch (ActiveWindow)
        {
            case "Loadout":
                LoadoutUI.GetComponent<Loadout>().HandleSelect();
                break;
            case "ShipSelection":
                ShipSelectionUI.GetComponent<ShipSelection>().SelectShip();
                break;
        }
    }

    public void EnablePauseMenu()
    {
        if (PauseMenuUI == null) PauseMenuUI = Instantiate(AssetManager.PauseMenuPrefab, GameplayOverlayCanvas.transform);
        else PauseMenuUI.SetActive(true);
    }

    public void DisablePauseMenu()
    {
        if (PauseMenuUI == null) return;
        else PauseMenuUI.SetActive(false);
    }
}
