using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Game Data
    public static PlayerManager Inst { get; private set; }

    // Player Data
    public int Lives { get; set; }
    public PlayerShip ActivePlayerShip { get; set; }

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Debug.Log("PlayerManager already exists");
            Destroy(gameObject);
            return;  // Ensure no further code execution in this instance
        }
        Inst = this;
    }

    public void IncrementLives(int amt)
    {
        Lives += amt;
        HUDManager.Inst.UpdateLivesDisplay();
    }

    public void SpawnPlayer()
    {
        ActivePlayerShip = Instantiate(AssetManager.PlayerPrefab, new Vector3(0, -4, 10), Quaternion.identity);
    }

    public void HandlePlayerDestroyed()
    {
        Debug.Log("Player destroyed");
        Lives -= 1;
        ActivePlayerShip = null;
        if (Lives > 0)
        {
            HUDManager.Inst.UpdateLivesDisplay();
            RespawnPlayer();
        }
        else
        {
            // Game Over
            GameManager.HandleGameOver();
        }
    }

    public void SetActiveShipToNull()
    {
        ActivePlayerShip = null;
    }

    // Coroutine to delay the respawn
    private IEnumerator DelayedRespawn(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        SpawnPlayer();
    }

    // Function called when the player is destroyed
    public void RespawnPlayer()
    {
        Debug.Log("Player destroyed. Respawning in 2 seconds...");
        StartCoroutine(DelayedRespawn(GameConfig.RespawnTimer)); // 2 seconds delay
    }
}
