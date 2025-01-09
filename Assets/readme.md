# Crux
While this will end up being the developer guide to the codebase, this is currently serving as a concept document for planning and tracking progress.

## Wrap Up

Enemy Behaviour
  - ~~Simplify the flight patterns~~
  - ~~Have each ship have an array of valid flight patterns and decide at random~~
  - ~~Slow down the fire rate~~

UI
  - ~~Upon item pickup have a bit of text appear above the player~~
  - Damage indicator?
  - Drop has icon of weapon/item

Progression
  - Decide how to increase difficulty
    - Boost modifiers
    - Increase quantity
    - Increase wave/level/stage duration
    - Add more enemies
  - RNG based drops

Bosses
  - Requires Movement Logic

Core
  - Energy for player ship
  - Setup player ship prefabs

Misc
  - ~~Ship entry and exit~~
  - Ships flash upon hit
  - ~~Accellerate out of stage~~
    - ~~Increase scroll rate of BGManager~~
  - Explosions on enemy deaths
  - Cash drops!? omg get to pick up gold coins with a satisfying souind
  - If the last wave used the same path, choose another
  - ElectroShield gets a sprite that sits on the player, activates animation on fire

Scenes
  - Inter-Stage
    - Weapon swap
    - Skills
  - Hi-Score
    - Persistent data?
  - Game Over

Potential Bugs
  - Stage ends before player spawns
    - There wont be an activeplayership to move

## InterScene
  - 

### Initial UI
  - Arrows pointing up/left/right
    - Selection animation
      - On directional hold, starts
      - On directional release, reverts
      - On animation complete, triggers relevant UI transition

### SkillTree
  - Three sets of trees
    - Update directional logic to handle being sideways
      - Setup as if they are upright, swap the up/down and left/right inputs if on side
    - Initial animation (OnEnable)
      - Each tree goes from a scale of 0 to 100, expanding outwards from ship (maintain ratio)
    - Terminate animation
      - Each tree goes from a scale of 100 to 0, contracting towards the ship (maintain ratio)
  - (Esc) Back button
  - Set initial cursor

### Loadout
```c#
  // Inspector
  public GameObject WeaponSlotUIContainer;
  public GameObject InitialWeaponSlotNodeCursor;

  // Data
  private float WeaponSlotSelectorDistance = 4f;
  private List<WeaponSlotNode> WeaponSlotNodes;
  private WeaponSlotNode WeaponSlotNodeCursor;
  public List<WeaponBase> LightWeapons;
  public List<WeaponBase> MediumWeapons;
  public List<WeaponBase> HeavyWeapons;

  void Awake() {
    InitialiseLoadoutUI();
  }

  void Enable() {
    UpdateAvailableWeapons();
    SetInitialCursor();
  }

  private void InitialiseLoadoutUI() {
    InstantiateWeaponSlotNodes();
    if (WeaponSlotNodes.Count == 0) {
      Debug.LogError("NO NODES FOUND")
      return;
    }
    InitialiseWeaponSlotSelectors()
    UpdateAvailableWeapons();
  }

  // Fetch AttachPoints, Sort by YPOS, Sort into Left/Right/Middle
  private void InstantiateWeaponSlotNodes() {
    // Find all AttachPoints from PlayerShip
    List<GameObject> attachPoints = PlayerManager.Inst.ActivePlayerShip.Find("AttachPoint");
    // Sort attachPoints by YPOS (what kind of list do I use for static order?)
    foreach (GameObject attachPoint in attachPoints) {
      WeaponSlotNode weaponSlotNode = Instantiate(AssetManager.WeaponSlotNode, attachPoint.transform.position, ROTATION STUFF);
      WeaponSlotNodes.Add(weaponSlotNode); // Dunno why, just in case
      if (weaponSlotNode.transform.position.x > 0) LeftWeaponSlotNodes.Add(weaponSlotNode);
      else if (weaponSlotNode.transform.position.x < 0) RightWeaponSlotNodes.Add(weaponSlotNode);
      else CentralWeaponSlotNodes.Add(weaponSlotNode); // Maybe? Just felt weird it not being in a list
    }
  }

  private void InitialiseWeaponSlotSelectors() {
    List<Vector3> WeaponSlotSelectorPositions = DetermineWeaponSlotSelectorPositions();
    InstantiateWeaponSlotSelectors(WeaponSlotSelectorPositions);
  }
  
  private List<Vector3> DetermineWeaponSlotSelectorPositions()
  {
    List<Vector3> WeaponSlotSelectorPositions = new List<Vector3>();

    // Angle between each WeaponSlotSelector
    float anglePerSelector = 360 / WeaponSlotNodes.Count;

    // Determine first WeaponSlotSelector position
    Vector3 FirstWeaponSlotSelectorPosition;
    if (WeaponSlotNodes[0].YPos == 0) {
      FirstWeaponSlotSelectorPosition = CalculateLocalPosition(0)
    } else {
      FirstWeaponSlotSelectorPosition = CalculateLocalPosition(anglePerSelector / 2);
    }

    // Calculate each position and add to list
    int i = 0;
    while (i < WeaponSlotNodes.Count)
    {
      if (i == 0) WeaponSlotSelectorPositions.Add(FirstWeaponSlotSelectorPostition);
      else {
        Vector3 newPosition = CalculateLocalPosition(anglePerSelector * i);
        WeaponSlotSelectorPositions.Add(newPosition);
      }
    }

    return WeaponSlotSelectorPositions;
  }

  private void InstantiateWeaponSlotSelectors(List<Vector3> weaponSlotSelectorPositions) {
    if (ListIsEmpty) {
      Debug.LogError("No weaponSlotSelectorPositions");
      return;
    }

    foreach(Vector3 weaponslotSelectorPosition in weaponSlotSelectorPositions)
    {
      GameObject weaponSlotSelector = Instantiate(AssetManager.WeaponSlotSelector, weaponSlotSelectorPostion, ROTATIONSTUFF);
      WeaponSlotSelectors.Add(weaponSlotSelector);
    }
  }

  Vector3 CalculateLocalPosition(float angleDegrees)
  {
      // Convert angle to radians
      float angleRadians = angleDegrees * Mathf.Deg2Rad;

      // Calculate offsets
      float offsetX = Mathf.Cos(angleRadians) * WeaponSlotSelectorDistance;
      float offsetY = Mathf.Sin(angleRadians) * WeaponSlotSelectorDistance;

      // Create the new position vector
      Vector3 offset = new Vector3(offsetX, offsetY, 0);

      // Transform the offset to local coordinates
      Vector3 localPosition = PlayerManager.Inst.ActivePlayerShip.transform.localPosition + offset;

      return localPosition;
  }

  // private void SetWeaponSlotSelectors() {
    // Find all Selectors, set to WeaponSlotSelectors
  // }

  private void SetInitialCursor()
  {
    weaponSlotNode = InitialWeaponSlotNodeCursor.GetComponent("WeaponSlotNode");
    SetCursor(weaponSlotNode);
  }

  private void SetCursor(WeaponSlotNode weaponSlotNode) {
    if (WeaponSlotNodeCursor != null) WeaponSlotNodeCursor.Deselect();
    WeaponSlotNodeCursor = weaponSlotNode;
    WeaponSlotCursor.Highlight();
  }

  private void UpdateAvailableWeapons() {
    LightWeapons = LoadoutManager.FetchWeapons(WeaponSlotType.Light);
    MediumWeapons = LoadoutManager.FetchWeapons(WeaponSlotType.Medium);
    HeavyWeapons = LoadoutManager.FetchWeapons(WeaponSlotType.Heavy);
  }

  HandleSelect() {
    if (WeaponSlotCursor != null) WeaponSlotCursor.HandleSelect();
  }

  HandleBack() {
    if (WeaponSlotCursor.Selected) WeaponSlotCursor.Deselect();
    else // Trigger termination animations and return to InterScene
  }

  HandleMoveLeft() {
    if (!WeaponSlotCursor.Selected) {
      WeaponSlot leftWeaponNode = DetermineAppropriateHoizontalNode(true);
      if (leftWeaponNode == null) return;
      SetCursor(leftWeaponNode);
    }
  }

  HandleMoveRight() {
    if (!WeaponSlotCursor.Selected) {
      WeaponSlot rightWeaponNode = DetermineAppropriateHoizontalNode(false);
      if (rightWeaponNode == null) return;
      SetCursor(rightWeaponNode);
    }
  }

  HandleMoveUp() {
    if (!WeaponSlotCursor.Selected) {
      WeaponSlot aboveWeaponNode = DetermineAppropriateVerticalNode(true);
      if (aboveWeaponNode == null) return;
      SetCursor(aboveWeaponNode);      
    } else {
      WeaponSlotCursor.WeaponSlotSelector.HandleScrollUp();
    }
  }

  HandleMoveDown() {
    if (!WeaponSlotCursor.Selected) {
      WeaponSlot belowWeaponNode = DetermineAppropriateVerticalNode(false);
      if (belowWeaponNode == null) return;
      SetCursor(belowWeaponNode);      
    } else {
      WeaponSlotCursor.WeaponSlotSelector.HandleScrollDown();
    }
  }
```

#### WeaponSlotNode
A polyganal border, designed to center on top of an AttachPoint and link to a WeaponSlotSelector.

```c#
  // Inspector
  public GameObject WeaponSlotSelector;
  public Color HighlightedColor;
  public Color SelectedColor;

  // Data
  public bool IsSelected;
  public bool IsHighlighted;
  private Color DefaultColor;
  private Image ColorComponent;

  void Awake() {
    ColorComponent = GetComponent<Image>();
    // Fetch current Color from ColorComponent and set as DefaultColor
  }

  void Highlight() {
    if (IsHighlighted || IsSelected) return;
    // Set Color of GameObject to HighlightedColor
    IsHighlighted = true;
  }

  void Deselect() {
    if (!IsSelected || !IsHighlighted) return;
    WeaponSlotSelector.HandleDeselect();
    IsSelected = false;
  }

  void HandleSelect() {
    if (IsSelected) {
      // Pass select on if already selected
      WeaponSlotSelector.HandleSelect()
      return;
    }

    WeaponSlotSelector.Activate()
    IsSelected = true;
  }
```

GameObject
  - White square
      - Highlighted color
      - Selected color

Animations
  - Init
    - Some kind of pop in or lock-on
  - Terminate
    - Fade out or unlock-on

#### WeaponSlotSelector
GameObject
  - IconBlock
    - WeaponSlotType Sprite
  - DescriptionBlock
    - Weapon Sprite
    - Weapon Description
  - WeaponSlotTypeBlock
    - WeaponSlotType

Animations
  - Init
    - asd

Functions
  - Initialise(availableWeapons)
    - AvailableWeapons = availableWeapons
  - SetState(WeaponSlotSelector.State state)
    - if (state == Default)
      - IconBlock and DescriptionBlock
      - if (CurrentState == Selected) TerminateDropDown()
    - if (state == Hover)
      - IconBlock, DescriptionBlock, and WeaponSlotTypeBlock
        - if (CurrentState == Selected) TerminateDropDown()
    - if (state == Selected)
      - if (CurrentState == Selected) return
      - else InitDropDown()
  - InitDropDown()
    - 

  - AvailableWeaponList
    - GameObject
    - Animations
    - Functions

  - Place WeaponSlotNodes on each WeaponSlot
    - Initial animation
      - Extend lines from WeaponSlotNodes to WeaponSlotSelectors
        - Maybe add ports on both to have the line draw itself
        - Maybe manually add the lines and just setup an animation
      - WeaponSlotSelectors expand away from port side
        - 
    - Selected variant
    - Highighted variant


#### 360 Concept
- Modify the movement logic to 8 directions
- Have the camera pan with the ship as the center, releasing the lock-on when the edges of the camera touch the edges.
  - Camera Component
    - Inspector Variables
      - LeftBorder, RightBorder, TopBorder, BottomBorder
    - Functions
      - public UpdateCameraPos()
        - If PlayerManager.ActivePlayerShip != null, center on ship
      - private UpdateBorderContacts

##### Skill Concepts
- EMP Blast: Temporarily disables enemy ships' systems, rendering them unable to fire or move for a short duration.
- Cloaking Device: Makes the player's ship invisible to enemies for a limited time, allowing for strategic repositioning or escape.
- Gravity Well: Creates a field that slows down enemy ships and projectiles, giving the player more time to react.
- Reflective Shield: Reflects incoming projectiles back at enemies, potentially causing them damage.
- Overdrive: Temporarily increases the ship's speed and fire rate, allowing for rapid movement and attack.
- Nanobot Repair: Gradually repairs the ship's hull over time, providing sustained survivability.
- Missile Barrage: Launches a series of homing missiles that target multiple enemies.
- Energy Absorption: Converts a percentage of incoming damage into energy that can be used to power other skills.
- Time Dilation: Slows down time for a brief period, giving the player an advantage in dodging attacks and targeting enemies.
- Teleportation: Instantly moves the player's ship to a different location on the screen, useful for evading attacks or closing in on enemies.
- Decoy Drones: Deploys drones that mimic the player's ship, confusing enemies and drawing their fire.
- Plasma Wave: Emits a wave of plasma that damages all enemies in a radius around the player's ship.
- Magnetic Field: Attracts nearby power-ups and resources to the player's ship, making collection easier.
- Ion Cannon: Fires a powerful, concentrated beam that pierces through multiple enemies.
- Holographic Projections: Creates holographic duplicates of the player's ship to distract and confuse enemies.