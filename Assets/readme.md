# Crux

### Wrap Up

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

#### InterScene
There are a few ways to accomplish this
  - Fresh scene
    - Encourages music to be swapped out upon new stage
  - Persistent scene
    - Preserves Music
    - Gameplay might feel more fluent
  - Hybrid
    - Upon stage end, show score tally, equips, skills
    - Once finished, fly off and transition into new scene

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
        - 

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