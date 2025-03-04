using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Project.Utilities
{
    public class VideoSceneController : MonoBehaviour
    {
        public VideoPlayer videoPlayer;
        public string nextSceneName;

        void Start()
        {
            videoPlayer.loopPointReached += OnVideoFinished; // Subscribe to the event
        }

        void OnVideoFinished(VideoPlayer vp)
        {
            SceneManager.LoadScene(nextSceneName); // Load the next scene
        }
    }
}
