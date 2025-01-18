using UnityEngine;

public abstract class UIWindowBase : MonoBehaviour
{
    // Abstract methods to be implemented by derived classes
    public abstract void HandleMoveLeft();
    public abstract void HandleMoveRight();
    public abstract void HandleMoveUp();
    public abstract void HandleMoveDown();
    public abstract void HandleSelect();
    public abstract void HandleBack();
    public abstract void HandleExit();
}