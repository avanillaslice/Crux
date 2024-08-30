using UnityEngine;

public class InterStage : MonoBehaviour
{
    public void Continue()
    {
        GameManager.HandleInterStageCompleted();
    }

    public void OpenOptions()
    {
        // Assuming you have an options menu scene or a way to show options
        Debug.Log("Open options here.");
        // GameManager.NavigateToOptions();
        // Implement your options menu functionality here, e.g., GameManager.LoadScene("OptionsMenu");
    }

    public void ExitGame()
    {
        // Exit the application
        Application.Quit();
    }
}
