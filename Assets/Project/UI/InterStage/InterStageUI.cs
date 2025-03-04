using Project.Core;
using UnityEngine;

namespace Project.UI.InterStage
{
    public class InterStageUI : MonoBehaviour
    {
        public void Continue()
        {
            GameManager.HandleInterStageCompleted();
        }

        public void SkillTree()
        {
            UIManager.Inst.TransitionToSkillTree();
        }

        public void Loadout()
        {
            UIManager.Inst.TransitionToLoadout();
        }

        public void OldLoadout()
        {
            UIManager.Inst.TransitionToOldLoadout();
        }

        public void Skills()
        {
        }
    }
}
