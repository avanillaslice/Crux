using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static bool IsPaused = false;
    public static bool DisplayingFirstSpecialWeaponUI = false;
    public static bool HasDisplayedFirstSpecialWeaponUI = false;
    public static int Score { get; private set; }
    public static bool SceneIsChanging;
    public static Queue<string> BackgroundMusicQueue { get; private set; } = new Queue<string>();

    public static async void InitiateGameplay(bool skipLoad)
    {
        // If Game scene already loaded
        if (!skipLoad)
        {
            await LoadSceneAsync("Game");
        }

        Score = GameConfig.InitialScore;
        PlayerManager.Inst.Lives = GameConfig.InitialLives;
        PlayerManager.Inst.BuildInitialSkills();
        MusicManager.Inst.PlayBackgroundMusic();
        await PlayerManager.Inst.SpawnPlayerAsync(true); // Wait for the player to arrive
        GameInputHandler.Inst.EnableGameplayControls();
        StageManager.StartStage(0);
    }

    public static void TogglePause()
    {
        if (!IsPaused) PauseGame();
        else UnPauseGame();
    }

    private static void PauseGame()
    {
        HUDManager.Inst.EnablePauseMenu();
        Time.timeScale = 0f;
        IsPaused = true;
    }

    private static void UnPauseGame()
    {
        HUDManager.Inst.DisablePauseMenu();
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public static void HandleSpecialWeaponUnlock()
    {
        if (HasDisplayedFirstSpecialWeaponUI) return;
        HUDManager.Inst.EnableSpecialWeaponUnlockedUI();
        Time.timeScale = 0f;
        DisplayingFirstSpecialWeaponUI = true;
        HasDisplayedFirstSpecialWeaponUI = true;
        IsPaused = true;
    }

    public static void DisableFirstSpecialWeaponUI()
    {
        if (!DisplayingFirstSpecialWeaponUI) return;
        HUDManager.Inst.DisableSpecialWeaponUnlockedUI();
        Time.timeScale = 1f;
        DisplayingFirstSpecialWeaponUI = false;
        IsPaused = false;
    }

    public static void SetBackgroundMusicQueue(List<string> musicKeys)
    {
        BackgroundMusicQueue.Clear();
        foreach (string key in musicKeys)
        {
            BackgroundMusicQueue.Enqueue(key);
        }
    }

    public static void IncrementScore(int pointsToAdd)
    {
        Score += pointsToAdd;
        HUDManager.Inst.UpdateScoreDisplay();
    }

    public static async void HandleGameOver()
    {
        Debug.Log("GAME OVER");
        await LoadSceneAsync("MainMenu");
    }

    public static void HandleStageSelected(int stageIndex)
    {
        StageManager.StartStage(stageIndex);
    }

    public static async void HandleStageCompleted()
    {
        TransitionToInterStage();
        // await PlayerManager.Inst.FlyOutOfScene();
        // await LoadSceneAsync("MainMenu");
    }

    public static void TransitionToInterStage()
    {
        GameInputHandler.Inst.DisableGameplayControls();
        HUDManager.Inst.DisableGameplayUI();
        HUDManager.Inst.EnableInterStageUI();
        GameInputHandler.Inst.EnableMenuNavigationControls();
    }

    public static Task LoadSceneAsync(string sceneName)
    {
        SceneIsChanging = true;
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.completed += _ => {
            tcs.SetResult(true);
            Time.timeScale = 1f;
            SceneIsChanging = false;
        };
        return tcs.Task;
    }

    public static void LoadScene(string sceneName)
    {
        SceneIsChanging = true;
        SceneManager.LoadScene(sceneName);
        SceneIsChanging = false;
    }
}
