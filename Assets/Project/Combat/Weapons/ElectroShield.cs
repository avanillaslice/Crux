using UnityEngine;
using System;

public class ElectroShield : ToggleFireWeaponBase
{
    // Initialize default values
    private GameObject Shield;
    private GameObject ElectroShieldEffectPrefab;
    private GameObject ElectricExplosionPrefab;
    public float InitialCharge = 10;
    public float InitialHealth = 300;

    protected void Awake()
    {
        hasAnimation = false;
        CurrentFireRate = BaseFireRate;

        // Fetch the ElectroShieldEffect prefab from the AssetManager
        ElectroShieldEffectPrefab = AssetManager.GetWeaponPrefab("ElectroShieldEffect");
        if (ElectroShieldEffectPrefab == null)
        {
            Debug.LogError("Failed to load ElectroShieldEffect prefab!");
        }

        // Fetch the ElectricExplosion prefab from the AssetManager
        ElectricExplosionPrefab = AssetManager.GetProjectilePrefab("ElectricExplosion");
        if (ElectricExplosionPrefab == null)
        {
            Debug.LogError("Failed to load ElectricExplosion prefab!");
        }
    }

    protected override void Fire(bool isEnemy)
    {
        if (ElectroShieldEffectPrefab != null)
        {
            // Instantiate the ElectroShieldEffect at the center of the parent ship
            ShipBase parentShip = GetComponentInParent<ShipBase>();
            if (parentShip != null)
            {
                Shield = Instantiate(ElectroShieldEffectPrefab, parentShip.transform.position, Quaternion.identity, parentShip.transform);
                ElectroShieldEffect ElectroShieldEffectComponent = Shield.GetComponent<ElectroShieldEffect>();
                ElectroShieldEffectComponent.Initialise(isEnemy, InitialCharge, InitialHealth, this);
                
            }
            else
            {
                Debug.LogError("Parent ship not found!");
            }
        }
    }

    protected override void CeaseFire(Action onCompleted)
    {
        if (ElectricExplosionPrefab != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.time = 0.5f;
                audioSource.Play();
            }
            else
            {
                Debug.LogError("AudioSource component not found!");
            }
            // Instantiate the ElectricExplosion at the center of the parent ship
            ShipBase parentShip = GetComponentInParent<ShipBase>();
            if (parentShip != null)
            {
                GameObject electricExplosion = Instantiate(ElectricExplosionPrefab, parentShip.transform.position, Quaternion.identity, parentShip.transform);
                ElectricExplosion explosionScript = electricExplosion.GetComponent<ElectricExplosion>();
                explosionScript.Initialise(Shield.GetComponent<ElectroShieldEffect>().CurrentCharge);
                if (explosionScript != null)
                {
                    explosionScript.OnExplosionFinished += () =>
                    {
                        onCompleted?.Invoke();
                    };
                    Destroy(Shield);
                }
            }
            else
            {
                Debug.LogError("Parent ship not found!");
                onCompleted?.Invoke();
            }
        }
        else
        {
            Debug.Log("ElectricExplosionPrefab was null");
            Destroy(Shield);
            onCompleted?.Invoke();
        }
    }
}
