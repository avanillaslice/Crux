# Color Manager System for Crux

This color management system provides a centralized way to define, access, and apply color schemes throughout your game. It helps maintain visual consistency and makes it easy to update colors across the entire project.

## Setup

1. In Unity, go to the menu: **Crux > Setup > Create Color Manager**
2. This will create a GameObject with the ColorManager component in your scene
3. The ColorManager is set up as a singleton and will persist between scenes

## Configuring Color Schemes

1. Select the ColorManager GameObject in your scene
2. In the Inspector, you'll see a list of color schemes
3. Each scheme contains a list of named colors
4. You can add, remove, or modify color schemes and colors as needed
5. Set the "Default Scheme Name" to your preferred default scheme

## Using Colors in Your Code

### Basic Usage

```csharp
// Get a color from the default scheme
Color primaryColor = ColorManager.Instance.GetColor("Primary");

// Get a color from a specific scheme
Color accentColor = ColorManager.Instance.GetColor("Accent", "Dark");

// Set a color at runtime
ColorManager.Instance.SetColor("CustomColor", new Color(1, 0, 1), "Default");
```

### Extension Methods

The system includes extension methods for common Unity components:

```csharp
// Set color on UI Image
myImage.SetNamedColor("Primary");

// Set color on TextMeshPro text
myText.SetNamedColor("Text");

// Set color on SpriteRenderer
mySpriteRenderer.SetNamedColor("Secondary");

// Set color on Material
myMaterial.SetNamedColor("Accent");

// Get a color from any MonoBehaviour
Color warningColor = this.GetNamedColor("Warning");
```

## Example

See the `ColorManagerExample.cs` script for a complete example of how to apply colors to UI elements.

## Best Practices

1. **Consistent Naming**: Use consistent color names across schemes (e.g., "Primary", "Secondary", "Background")
2. **Semantic Names**: Name colors based on their purpose, not their appearance (e.g., "Warning" instead of "Yellow")
3. **Scheme Organization**: Create separate schemes for different game states or UI themes
4. **Default Fallbacks**: Always provide a complete set of colors in the default scheme

## Color Scheme Structure

The default setup includes two schemes:

### Default Scheme

- Primary: Blue (#009FFF)
- Secondary: Orange (#FF9900)
- Accent: Pink (#FF3366)
- Background: Dark Blue (#191933)
- Text: White (#FFFFFF)
- TextDisabled: Gray (#808080)
- Warning: Yellow (#FFFF00)
- Error: Red (#FF0000)
- Success: Green (#00FF00)

### Dark Scheme

- Primary: Darker Blue (#0066CC)
- Secondary: Darker Orange (#CC6600)
- Accent: Darker Pink (#CC194D)
- Background: Very Dark Blue (#0D0D1A)
- Text: Light Gray (#E6E6E6)
- TextDisabled: Dark Gray (#666666)
- Warning: Darker Yellow (#E6E633)
- Error: Darker Red (#E63333)
- Success: Darker Green (#33E633)

Feel free to modify these schemes or add new ones to match your game's visual style.
