using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class DroneShieldEffect : ShieldEffectBase
{
    private ShieldDrone ParentShieldDrone;

    public void Initialise(bool isEnemy, ShieldDrone parentShieldDrone)
    {
        IsEnemyShield = isEnemy;
        ParentShieldDrone = parentShieldDrone;
    }

    public override void HandleHit(float damage)
    {
        ParentShieldDrone.SubtractCharge(damage);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        if (tag == "Enemy" && !IsEnemyShield)
        {
            // Upon collision, destroy enemy if their hp is less than the current charge
            EnemyShip enemyShip = other.GetComponent<EnemyShip>();
            if (enemyShip != null)
            {
                HandleHit(enemyShip.damageOnCollision);
                enemyShip.Die();
            }
        }
    }
}
