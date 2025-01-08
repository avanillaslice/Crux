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
  private List<WeaponSlotNode> WeaponSlotNodes;
  // private List<WeaponSlotSelector> WeaponSlotSelectors;

  private WeaponSlotNode WeaponSlotNodeCursor;
  // private WeaponSlotNode WeaponSlotSelectorCursor;

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
    SetWeaponSlotNodes();
    // SetWeaponSlotSelectors();
    UpdateAvailableWeapons();
  }

  private void SetWeaponSlotNodes() {
    // Find all Nodes, set to WeaponSlotNodes
  }

  // private void SetWeaponSlotSelectors() {
    // Find all Selectors, set to WeaponSlotSelectors
  // }

  private void SetInitialCursor()
  {
    WeaponSlotNodeCursor = InitialWeaponSlotNodeCursor.GetComponent("WeaponSlotNode");
    
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
    if (!WeaponSlotCursor.Selected) WeaponSlotCursor.Select();
    else WeaponSlotCursor.WeaponSlotSelector.HandleSelect();
  }

  HandleBack() {
    if (WeaponSlotCursor.Selected) 
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
GameObject
  - White square
      - Highlighted color
      - Selected color

Animations
  - Init
    - Some kind of pop in or lock-on
  - Terminate
    - Fade out or unlock-on

Functions
```c#
HandleSelect() {
  if (IsSelected) {
    // Pass select on if already selected
    WeaponSlotSelector.HandleSelect()
    return;
  }

  WeaponSlotSelector.Activate()
  IsSelected = true;
}


```
  - HandleSelect
    - WeaponSlotSelector.Activate()
    - HandleUp/Down/Left/Right is now sent to WeaponSlotSelector
      - This will make more sense being directed by Loadout.cs

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
        - 
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