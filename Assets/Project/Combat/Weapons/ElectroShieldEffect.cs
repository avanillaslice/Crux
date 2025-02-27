using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ElectroShieldEffect : ShieldEffectBase
{
    public float MaxPitch = 3.0f; // Maximum pitch value to prevent endless growth
    public float CurrentCharge;
    private float MaxCharge = 500;
    private float Health;
    private AudioSource audioSource;
    private ToggleFireWeaponBase ElectroShieldComponent; 

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on the GameObject.");
        }
    }

    public void Initialise(bool isEnemy, float charge, float initialHealth, ToggleFireWeaponBase electroShieldComponent)
    {
        IsEnemyShield = isEnemy;
        CurrentCharge = charge;
        Health = initialHealth;
        ElectroShieldComponent = electroShieldComponent;
    }

    public override void HandleHit(float damage) {
        CurrentCharge += damage;
        if (CurrentCharge > MaxCharge)
        {
            CurrentCharge = MaxCharge;
        }
        if (audioSource != null)
        {
            audioSource.pitch = 3 * (CurrentCharge / MaxCharge) + 1;
        }
    }

    private void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            // Do shield break stuff
            // Play break sound
            ElectroShieldComponent.AttemptCeaseFire();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        if (tag == "Enemy" && !IsEnemyShield)
        {
            EnemyShip enemyShip = other.GetComponent<EnemyShip>();
            if (enemyShip != null)
            {
                TakeDamage(enemyShip.damageOnCollision);
                enemyShip.Die();
            }
        }
    }
}
