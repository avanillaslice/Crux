using System;
using UnityEngine;

namespace Project.Core
{
    public static class StageManager
    {
        public static bool StageActive = false;
        public static event Action OnStageStart;
        public static event Action OnStageCompleted;
        public static void StartStage(int stageIndex)
        {
            Debug.Log($"Initializing Stage {stageIndex}");
            StageData stageData = GameConfig.GameData.Stages[stageIndex];
            if (!ValidateStage(stageData))
            {
                Debug.LogError("Stage validation failed. Stopping stage initialization.");
                EndStage();
            }
            LevelManager.StartLevels(stageData);
            StageActive = true;
            OnStageStart?.Invoke();
        }

        private static bool ValidateStage(StageData stage)
        {
            if (stage.Levels == null || stage.Levels.Length == 0) {
                Debug.LogError("No Levels found in Stage!");
                return false;
            }
            return true;
        }

        public static void HandleAllLevelsCompleted()
        {
            // Stage Complete Events
            EndStage();
        }

        private static void EndStage()
        {
            Debug.Log("Stage Completed!");
            StageActive = false;
            OnStageCompleted?.Invoke();
            GameManager.HandleStageCompleted();
        }
    }
}