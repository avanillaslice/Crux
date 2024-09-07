using UnityEngine;

public class InterStage : MonoBehaviour
{
    public void Continue()
    {
        GameManager.HandleInterStageCompleted();
    }

    public void Loadout()
    {
        UIManager.Inst.TransitionToLoadout();
    }

    public void Skills()
    {
    }
}
