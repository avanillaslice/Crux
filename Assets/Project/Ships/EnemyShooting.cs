using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public float minBatchDelay = 3f; // min wait time before shooting again
    public float maxBatchDelay = 7f; // max wait time before shooting again
    public float minShootDuration = 1f; // min duration for shooting
    public float maxShootDuration = 3f; // max duration for shooting
    public float yShootThreshold = 0f; // The Y coordinate below which the enemy can shoot

    private float batchDelay = 0f;
    private bool isShooting = false;
    private float shootDuration = 0f;
    private float shootTimer = 0f;
    private EnemyShip enemyShip;

    void Start()
    {
        enemyShip = GetComponent<EnemyShip>();
        CalculateNextBatch();
    }

    void Update()
    {
        // Check if the enemy is below the specified Y coordinate
        if (transform.position.y >= yShootThreshold) return;

        if (isShooting)
        {
            // Shooting logic
            shootTimer += Time.deltaTime;
            if (shootTimer < shootDuration)
            {
                enemyShip.EnablePrimaryFire();
            }
            else
            {
                // Once shooting duration is over, start waiting
                enemyShip.DisablePrimaryFire();
                CalculateNextBatch();
            }
            return;
        }

        // Waiting logic
        if (batchDelay > 0) batchDelay -= Time.deltaTime;
        else
        {
            isShooting = true;
            shootTimer = 0f;
        }
    }

    public void CalculateNextBatch()
    {
        isShooting = false;
        shootDuration = Random.Range(minShootDuration, maxShootDuration);
        batchDelay = Random.Range(minBatchDelay, maxBatchDelay);
    }
}
