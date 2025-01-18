using UnityEngine;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Inst { get; private set; }
    private List<AudioSource> AudioSources = new List<AudioSource>();
    public AudioSource BGMAudioSource;

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Debug.Log("MusicManager already exists");
            Destroy(gameObject);
            return;  // Ensure no further code execution in this instance
        }
        Inst = this;
        InitialiseAudioSources();
    }

    private void InitialiseAudioSources() {
        AudioSources.AddRange(GetComponents<AudioSource>());
        foreach(AudioSource audioSource in AudioSources) audioSource.volume = GameConfig.BaseVolume;

        // Set randomised queue on GameManager
        if (GameManager.BackgroundMusicQueue.Count < 1) CreateBGMQueue();

        BGMAudioSource.volume = GameConfig.BaseVolume;
    }

    private void CreateBGMQueue()
    {
        // Get all the keys from the BackgroundMusic dictionary
        List<string> musicKeys = new List<string>(AssetManager.BackgroundMusic.Keys);

        // Shuffle the list of keys
        for (int i = musicKeys.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string temp = musicKeys[i];
            musicKeys[i] = musicKeys[randomIndex];
            musicKeys[randomIndex] = temp;
        }

        // Assign the shuffled list to BackgroundMusicQueue
        GameManager.SetBackgroundMusicQueue(musicKeys);
    }

    public void PlaySoundEffect(string clipName, float pitch = 1.0f)
    {
        // Find an available audio source
        AudioSource availableSource = null;
        foreach (AudioSource source in AudioSources)
        {
            if (!source.isPlaying)
            {
                availableSource = source;
                break;
            }
        }
    
        if (availableSource == null)
        {
            Debug.LogError("No available audio sources to play the clip.");
            return;
        }
    
        // Get the audio clip from the AssetManager
        if (AssetManager.SoundEffects.TryGetValue(clipName, out AudioClip clip))
        {
            availableSource.clip = clip;
            availableSource.pitch = pitch; // Set the pitch of the audio source
            availableSource.Play();
        }
        else
        {
            Debug.LogError($"Audio clip '{clipName}' not found in the AssetManager.");
        }
    }

    public void PlayBackgroundMusic()
    {
        string clipName = GameManager.BackgroundMusicQueue.Dequeue();
        if (clipName == null)
        {
            Debug.LogError("[MusicManager] BackgroundMusicQueue is empty");
            return;
        }

        // Get the audio clip from the AssetManager
        if (AssetManager.BackgroundMusic.TryGetValue(clipName, out AudioClip clip))
        {
            Debug.Log("[MusicManager] Playing: " + clipName);
            BGMAudioSource.clip = clip;
            BGMAudioSource.Play();
        }
        else
        {
            Debug.LogError($"Audio clip '{clipName}' not found");
        }
    }

}
