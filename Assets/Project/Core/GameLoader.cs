using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Core
{
  public class GameLoader : MonoBehaviour
  {
    private bool WasNotLoaded { get; set; }
    void Awake()
    {
      if (!GameConfig.HasBeenLoaded)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        UnityEngine.Debug.Log("Starting game configuration loading...");
        WasNotLoaded = true;
        GameConfig.Initialise();
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Game configuration loaded in {stopwatch.ElapsedMilliseconds}ms");
      }
    }

    void Start()
    {
      if (WasNotLoaded)
      {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == "Game")
        {
          GameManager.InitiateGameplay(true);
        }
      }
    }
  }
}
