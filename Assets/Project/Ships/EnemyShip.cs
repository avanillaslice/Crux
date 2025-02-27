using UnityEngine;

public class EnemyShip : ShipBase
{
    [SerializeField] protected int pointsOnKill;
    public int damageOnCollision = 10;

    public bool TargetedByTurret;
    
    public delegate void EnemyShipEvent(EnemyShip ship);
    public event EnemyShipEvent OnDestroyed;

    private EffectData AssignedEffectData;

    void Start()
    {
        IsEnemy = true;
    }

    public void AssignItemDrop(EffectData effectData)
    {
        if (effectData != null)
        {
            AssignedEffectData = effectData;
        }
    }

    public override void Die()
    {
        GameManager.IncrementScore(pointsOnKill);
        Explode();
        if (AssignedEffectData != null)
        {
            ItemDropManager.Inst.CreateItemDrop(transform.position, AssignedEffectData);
        }
    }

    void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }

    protected override void SubtractHealth(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            isDestroyed = true;
            Die();
        }
    }

    public override void AddShield(float amt)
    {
        Shield += amt;
        ShieldIsActive = true;
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
        else return 0;
    }

    private void DeactivateShield()
    {
        ShieldIsActive = false;
        Shield = 0;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = DefaultMaterial;
    }

    public override void AddHealth(float amt)
    {
        Health += amt;
    }
}
