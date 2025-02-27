using UnityEngine;
using System;

public class ElectricExplosion : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private GameObject ElectricExplosionChainPrefab;

    public event Action OnExplosionFinished;

    public float Charge;

    [SerializeField] private Color LowColor;
    [SerializeField] private Color HighColor;

    void Awake()
    {
        // Fetch the ElectricExplosion prefab from the AssetManager
        ElectricExplosionChainPrefab = AssetManager.GetProjectilePrefab("ElectricExplosionChain");
        if (ElectricExplosionChainPrefab == null)
        {
            Debug.LogError("Failed to load ElectricExplosion prefab!");
        }
    }

    public void Initialise(float charge)
    {
        Charge = charge;
        // Fetch the ParticleSystem component
        particleSystem = GetComponent<ParticleSystem>();

        //! This has been covered by the inspector
        // Check if the ParticleSystem component is found
        if (particleSystem != null)
        {
            // Enable collision module
            var collision = particleSystem.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision2D;
            collision.sendCollisionMessages = true;

            // Set the particle color based on the charge
            SetParticleColor(charge);

            // Play the ParticleSystem
            particleSystem.Play();

            // Register to the particle system's stopped event
            var main = particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
        else
        {
            Debug.LogError("ParticleSystem component not found on the ElectricExplosion prefab.");
        }
    }

    private void SetParticleColor(float charge)
    {
        var main = particleSystem.main;

        // Calculate the color based on the charge value
        float t = Mathf.Clamp01(charge / 500f);
        Color particleColor = Color.Lerp(LowColor, HighColor, t);

        // Apply the calculated color to the particle system
        main.startColor = particleColor;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            ShipBase ship = other.GetComponent<ShipBase>();
            if (ship != null)
            {
                //! Check if charge is high enough?
                GameObject electricExplosionChain = Instantiate(ElectricExplosionChainPrefab, ship.transform.position, Quaternion.identity);
                ElectricExplosionChain explosionChainScript = electricExplosionChain.GetComponent<ElectricExplosionChain>();
                explosionChainScript.Initialise(Charge, 0, LowColor, HighColor);
                ship.TakeDamage(Charge / 5, 0);
            }
        }
    }

    void OnParticleSystemStopped()
    {
        OnExplosionFinished?.Invoke();
        Destroy(gameObject);
    }
}
