using UnityEngine;

public class PlasmaLight : ProjectileBase
{
    protected override void InitializeBehaviour(Vector2 initialVelocity, AttachPoint.RelativeSide side, Vector2? direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Set the velocity of the projectile
        rb.linearVelocity = initialVelocity + (Vector2)(transform.up * BaseSpeed * SpeedModifier);
    }
}
