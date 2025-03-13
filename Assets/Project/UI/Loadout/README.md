# Loadout UI System

This document provides an overview of the Loadout UI system architecture, which has been refactored to follow best practices for maintainability, extensibility, and performance.

## Architecture Overview

The Loadout UI system follows the Model-View-Controller (MVC) pattern, with additional patterns for specific concerns:

1. **Model**: Handles data management and business logic
2. **View**: Handles UI rendering and user interaction
3. **Controller**: Handles application flow and coordination
4. **Events**: Provides decoupled communication between components
5. **Factory**: Handles object creation
6. **Service Locator**: Provides access to shared services

## Directory Structure

```
Assets/Project/UI/Loadout/
├── Controllers/           # Controllers for coordinating system behavior
├── Events/                # Event system for decoupled communication
├── Factories/             # Factories for creating UI elements
├── Model/                 # Data models and business logic
├── Services/              # Service locator for shared services
├── Views/                 # UI components and rendering
└── README.md              # This file
```

## Key Components

### Model

- **LoadoutModel**: Central data store for the loadout system. Manages weapon data, node data, and calculations.

### Views

- **WeaponNode**: Represents a weapon attachment point in the UI.
- **WeaponNodeSelector**: Represents a weapon selector in the UI.
- **WeaponNodeSelectorList**: Represents a list of available weapons.
- **WeaponNodeSelectorListCell**: Represents a cell in the weapon list.

### Controllers

- **LoadoutUIController**: Main controller for the loadout system. Coordinates initialization, cleanup, and high-level flow.
- **LoadoutAnimationController**: Handles animation sequencing and timing.
- **LoadoutInputController**: Handles user input and mapping to actions.
- **LoadoutNavigationController**: Handles navigation between nodes.

### Events

- **LoadoutEvents**: Central event system for decoupled communication between components.

### Factories

- **LoadoutFactory**: Creates and initializes UI elements.

### Services

- **LoadoutServices**: Service locator for accessing shared services.

## Initialization Flow

1. **LoadoutUIController.OnEnable**:
   - Subscribes to events
   - Sets up player ship
   - Calls InitializeLoadoutUI

2. **InitializeLoadoutUI**:
   - Updates available weapons in the model
   - Loads weapon slots from the player ship
   - Processes weapon slots to create node data
   - Creates weapon nodes using the factory
   - Creates weapon node selectors using the factory
   - Links nodes to selectors
   - Initializes controllers
   - Starts animations

3. **Animation Sequence**:
   - Animates each weapon node with a delay
   - Activates connection lines
   - Triggers AllAnimationsComplete event when done

4. **Interaction Enabled**:
   - After animations complete, user interaction is enabled
   - Navigation between nodes is enabled
   - Weapon selection is enabled

## Interaction Flow

1. **User Input**:
   - Input is captured by LoadoutInputController
   - Input is mapped to events (navigation, select, back)

2. **Navigation**:
   - Navigation events are handled by LoadoutNavigationController
   - Current node is updated
   - Node hover state is updated

3. **Selection**:
   - Select events are forwarded to the current node
   - Node handles selection logic
   - Connection line is animated
   - Weapon list is activated

4. **Weapon Selection**:
   - User navigates the weapon list
   - Selected weapon is equipped to the weapon slot
   - Node is refreshed to reflect the new weapon

## Cleanup Flow

1. **LoadoutUIController.OnDisable**:
   - Unsubscribes from events
   - Resets player ship
   - Resets cursor state
   - Cleans up UI elements
   - Clears events

## Benefits of the New Architecture

1. **Separation of Concerns**: Each class has a single responsibility, making the code easier to understand and maintain.
2. **Decoupled Communication**: Components communicate through events, reducing dependencies.
3. **Centralized Data Management**: Data is managed by the model, providing a single source of truth.
4. **Improved Testability**: Components can be tested in isolation.
5. **Enhanced Extensibility**: New features can be added without modifying existing code.
6. **Better Performance**: Object pooling and optimized rendering reduce overhead.
7. **Clearer Initialization**: The initialization flow is more explicit and sequential.

## Migration Guide

To migrate from the old architecture to the new one:

1. Replace LoadoutUI with LoadoutUIController
2. Update references to LoadoutUI.Inst to LoadoutUIController.Inst
3. Use LoadoutEvents for communication between components
4. Use LoadoutServices to access shared services
5. Use LoadoutModel for data access and manipulation

## Future Improvements

1. **Object Pooling**: Implement object pooling for list cells and connection lines.
2. **Async Initialization**: Make initialization asynchronous to improve performance.
3. **State Pattern**: Implement explicit state objects for different UI states.
4. **Command Pattern**: Implement command objects for user actions.
5. **Data Binding**: Implement proper data binding between model and views. 