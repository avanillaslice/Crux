using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    // Protected variables to be set by inheriting classes
    [SerializeField]
    protected float BaseDamage;
    [SerializeField]
    protected float BaseSpeed;
    [SerializeField]
    protected GameObject ExplosionPrefab;

    protected bool FiredByEnemy;
    protected float SpeedModifier;
    protected float DamageModifier;
    protected float CriticalHitChanceModifier;
    protected int PiercingModifier;
    protected int PierceCount;

    // Initialize method to set the projectile's properties
    public void Initialize(bool isEnemy, float speedModifier, float damageModifier, int piercingModifier, float criticalHitChanceModifier, Vector2 initialVelocity, RelativeSide side, Vector2? direction = null)
    {
        FiredByEnemy = isEnemy;
        SpeedModifier = speedModifier;
        DamageModifier = damageModifier;
        PiercingModifier = piercingModifier;
        CriticalHitChanceModifier = criticalHitChanceModifier;
        // Call the specific behavior initialization
        InitializeBehaviour(initialVelocity, side, direction);
    }

    // Abstract method to be implemented by specific projectiles
    protected abstract void InitializeBehaviour(Vector2 initialVelocity, RelativeSide side, Vector2? direction);

    // Method to handle collision with other objects
    void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;

        // Code to execute when an object enters the trigger
        if ((tag == "Enemy" && !FiredByEnemy) || (tag == "Player" && FiredByEnemy))
        {
            ShipBase ship = other.GetComponent<ShipBase>();
            if (ship != null)
            {
                ship.TakeDamage(BaseDamage * DamageModifier, CriticalHitChanceModifier);
                PierceCount++;
            }
            Explode(PierceCount > PiercingModifier);
        }

        if (tag == "Shield")
        {
            ShieldEffectBase shield = other.GetComponent<ShieldEffectBase>();
            if (shield != null)
            {
                if (shield.IsEnemyShield != FiredByEnemy)
                {
                    shield.HandleHit(BaseDamage * DamageModifier);
                    Explode();
                }
            }
        }
    }

    public void Explode(bool destroyOnExplosion = true)
    {
        // Instantiate the explosion prefab at the projectile's position
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        if (destroyOnExplosion) Destroy(gameObject);
    }
}
