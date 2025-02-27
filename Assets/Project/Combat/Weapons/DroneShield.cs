using UnityEngine;
using System;

public class DroneShield : ToggleFireWeaponBase
{
    // Initialize default values
    private GameObject Shield;
    private GameObject DroneShieldEffectPrefab;
    private ShieldDrone ParentShieldDrone;

    protected void Awake()
    {
        hasAnimation = false;
        CurrentFireRate = BaseFireRate;

        // Fetch the DroneShieldEffect prefab from the AssetManager
        DroneShieldEffectPrefab = AssetManager.GetWeaponPrefab("DroneShieldEffect");
        if (DroneShieldEffectPrefab == null)
        {
            Debug.LogError("Failed to load DroneShieldEffect prefab!");
        }

        // Fetch the parent ShieldDrone component
        ShieldDrone parentShieldDrone = GetComponentInParent<ShieldDrone>();
        if (parentShieldDrone == null)
        {
            Debug.LogError("Failed to find parent ShieldDrone component!");
        }
        else
        {
            ParentShieldDrone = parentShieldDrone;
        }
    }

    protected override void Fire(bool isEnemy)
    {
        if (DroneShieldEffectPrefab != null)
        {
            ShipBase parentShip = GetComponentInParent<ShipBase>();
            if (parentShip != null)
            {
                Vector3 shieldPosition = parentShip.transform.position + new Vector3(0, 0.5f, 0);
                Shield = Instantiate(DroneShieldEffectPrefab, shieldPosition, Quaternion.identity, parentShip.transform);
                DroneShieldEffect DroneShieldEffectComponent = Shield.GetComponent<DroneShieldEffect>();
                DroneShieldEffectComponent.Initialise(isEnemy, ParentShieldDrone);
            }
            else
            {
                Debug.LogError("Parent ship not found!");
            }
        }
        else
        {
            Debug.LogError("DroneShieldEffectPrefab not found!");
        }
    }

    protected override void CeaseFire(Action onCompleted)
    {
        Destroy(Shield);
        onCompleted?.Invoke();
    }
}
