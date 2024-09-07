using UnityEngine;

public enum WeaponType
{
    Primary,
    Secondary,
    Special
}

public abstract class WeaponBase : MonoBehaviour
{
    // Public variables to be set from the inspector in derived classes
    public GameObject ProjectilePrefab;
    public Transform FirePoint;

    // Protected variables
    protected bool hasAnimation;
    [SerializeField]
    protected float BaseFireRate;
    protected float CurrentFireRate;
    protected float fireRateTimer;

    public SlotType SlotType;
    public WeaponType WeaponType;
    public string WeaponName;

    // Reference to the Rigidbody2D component of the weapon
    protected Rigidbody2D rb;

    [SerializeField]
    public Sprite WeaponIcon;

    // Add the Side field
    public AttachPoint.RelativeSide Side { get; set; }

    protected ShipBase ParentShip;

    void Start()
    {
        ParentShip = GetComponentInParent<ShipBase>();
        if (ParentShip == null)
        {
            Debug.LogError("SingleFireWeaponBase: No ShipBase component found on parent GameObject.");
        }
    }

    // Abstract methods to be implemented by derived classes
    public abstract void AttemptFire(bool isEnemy);
    public abstract void AttemptCeaseFire();

    protected virtual void StartAnimation()
    {
        if (hasAnimation)
        {
            // Animation logic here
        }
    }

    protected virtual void Update()
    {
        // Update the shooting timer
        fireRateTimer += Time.deltaTime;
    }
}
