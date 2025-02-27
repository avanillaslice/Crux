using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class ShieldEffectBase : MonoBehaviour
{
    public bool IsEnemyShield;

    public abstract void HandleHit(float damage);

    protected abstract void OnTriggerEnter2D(Collider2D other);
}
