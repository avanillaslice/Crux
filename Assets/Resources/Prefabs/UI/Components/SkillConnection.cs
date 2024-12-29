using UnityEngine;
using UnityEngine.UI;

public class SkillConnection : MonoBehaviour {

    // Inspector
    public SkillNode SkillNodeInput;
    public SkillNode SkillNodeOutput;
    public int PrerequisiteLevel;
    public Color EnabledColor;
    public Color DefaultColor = Color.white;

    // State
    [HideInInspector] // Can probably just be private
    public bool IsEnabled;
    private Image imageComponent;

    void Awake() {
        imageComponent = GetComponent<Image>();
        if (imageComponent == null) {
            Debug.LogError("Image component not found on the GameObject.");
        }
    }

    public void Enable() {
        imageComponent.color = EnabledColor;
        IsEnabled = true;
    }

    public void Disable() {
        imageComponent.color = DefaultColor;
        IsEnabled = false;
    }
}