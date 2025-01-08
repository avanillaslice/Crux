using UnityEngine;

public class PlayerShip : ShipBase
{
    public Canvas UICanvas;
    public GameObject CameraAnchor;

    protected override void Awake() {
        base.Awake(); // Call the base class Awake method
        if (GameConfig.CameraMovementEnabled) InitialiseCameraAnchor();
    }

    protected override void Update() {
        base.Update(); // Call the base class Update method
        UpdateCameraAnchor();
    }

    private void InitialiseCameraAnchor() {
        if (CameraAnchor == null) return;
        CameraAnchor = new GameObject("CameraAnchor");
        CameraAnchor.transform.position = transform.position;
    }

    private void UpdateCameraAnchor() {
        if (CameraAnchor == null) return;
        Vector3 targetPosition = transform.position;
        CameraAnchor.transform.position = Vector3.Lerp(CameraAnchor.transform.position, targetPosition, Time.deltaTime * 5f);
    }

    void Start()
    {
        ShieldIsActive = true;
        EmitOnSpawn();
        HUDManager.Inst.UpdateShieldBar();
        HUDManager.Inst.UpdateHealthBar();
    }

    public override void Die()
    {
        PlayerManager.Inst.HandlePlayerDestroyed();
        Explode();
    }

    protected override float SubtractShield(float damage)
    {
        Shield -= damage;

        if (Shield == 0) {
            DeactivateShield();
            return 0;
        }
        else if (Shield < 0) {
            float excessDamage = damage + Shield;
            DeactivateShield();
            return excessDamage;
        }
        else
        {
            HUDManager.Inst.UpdateShieldBar();
            return 0;
        }
    }

    protected override void SubtractHealth(float damage)
    {
        Health -= damage;
        if (Health <= 0) {
            Health = 0;
            isDestroyed = true;
            HUDManager.Inst.UpdateHealthBar();
            Die();
            return;
        }
        HUDManager.Inst.UpdateHealthBar();
    }

    public override void AddShield(float amt)
    {
        Shield += amt;
        ActivateShield();
        HUDManager.Inst.UpdateShieldBar();
    }

    public override void AddHealth(float amt)
    {
        Health += amt;
        HUDManager.Inst.UpdateHealthBar();
    }

    private void DeactivateShield()
    {
        ShieldIsActive = false;
        Shield = 0;
        HUDManager.Inst.UpdateShieldBar();
        MusicManager.Inst.PlaySoundEffect("ShieldPowerDown2", 1f);
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = DefaultMaterial;
    }

    private void ActivateShield()
    {
        ShieldIsActive = true;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = ShieldGlowMaterial;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;

        // Code to execute when an object enters the trigger
        if (tag == "Enemy")
        {
            EnemyShip enemyShip = other.GetComponent<EnemyShip>();
            if (enemyShip != null)
            {
                TakeDamage(enemyShip.damageOnCollision, 0);
                enemyShip.Die();
            }
            // Instantiate the explosion prefab at the projectile's position
            // Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        }
    }
}
