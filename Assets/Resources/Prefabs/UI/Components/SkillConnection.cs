public class SkillConnection {

    // Inspector
    public SkillNode SkillNodeIn;
    public SkillNode SkillNodeOut;

    // State
    public bool IsEnabled;

    public void Enable() {
        // Set Sprite to BRIGHT
        IsEnabled = true;
    }

    public void Disable() {
        // Set Sprite to DIM
        IsEnabled = false;
    }
}