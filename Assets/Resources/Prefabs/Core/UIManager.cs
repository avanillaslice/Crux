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
    private GameObject SpecialWeaponUnlockedUI;

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

    public void EnableSpecialWeaponUnlockedUI()
    {
        SpecialWeaponUnlockedUI = Instantiate(AssetManager.SpecialWeaponUnlockedPrefab, GameplayOverlayCanvas.transform);

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
    }

    public void EnableInterStageUI()
    {
        if (InterStageUI == null) InterStageUI = Instantiate(AssetManager.InterStageUIPrefab, InterStageUICanvas.transform);
        else InterStageUI.SetActive(true);
    }

    public void DisableInterStageUI()
    {
        if (InterStageUI == null) return;
        else InterStageUI.SetActive(false);
    }

    // Menu Controls...
    public void HandleMoveUp()
    {

    }

    public void HandleMoveDown()
    {

    }

    public void HandleMoveLeft()
    {
        // if (SceneManager.GetActiveScene().name == "MainMenu")
        if (ShipSelectionUI.activeSelf)
        {
            ShipSelectionUI.GetComponent<ShipSelection>().MoveCursorLeft();
        }
    }

    public void HandleMoveRight()
    {
        if (ShipSelectionUI.activeSelf)
        {
            ShipSelectionUI.GetComponent<ShipSelection>().MoveCursorRight();
        }
    }

    public void HandleSelect()
    {
        if (ShipSelectionUI.activeSelf)
        {
            ShipSelectionUI.GetComponent<ShipSelection>().SelectShip();
        }
    }
}
