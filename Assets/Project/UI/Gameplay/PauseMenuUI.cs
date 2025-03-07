using Project.Core;
using UnityEngine;

namespace Project.UI.Gameplay
{
    public class PauseMenuUI : MonoBehaviour
    {
        public static PauseMenuUI Inst { get; private set; }

        void Awake()
        {
            if (Inst != null && Inst != this)
            {
                Debug.Log("PauseMenu already exists");
                Destroy(gameObject);
                return;  // Ensure no further code execution in this instance
            }
            Inst = this;
            Resume();
        }

        public void Resume()
        {
            GameManager.TogglePause();
        }

        public void OpenOptions()
        {
            // Assuming you have an options menu scene or a way to show options
            Debug.Log("Open options here.");
            // GameManager.NavigateToOptions();
            // Implement your options menu functionality here, e.g., GameManager.LoadScene("OptionsMenu");
        }

        public async void GoToMainMenu()
        {
            await GameManager.LoadSceneAsync("MainMenu");
        }

        // Add other methods for buttons like GoToMainMenu(), OpenOptions(), etc.
    }
}