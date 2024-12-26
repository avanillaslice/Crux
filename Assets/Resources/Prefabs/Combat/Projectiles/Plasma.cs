using UnityEngine;

public class Plasma : ProjectileBase
{
    protected override void InitializeBehaviour(Vector2 initialVelocity, AttachPoint.RelativeSide side, Vector2? direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
    
        // Use the provided direction or default to transform.up
        Vector2 finalDirection = direction ?? (Vector2)transform.up;
    
        // Set the velocity of the projectile
        rb.linearVelocity = initialVelocity + (finalDirection * BaseSpeed * SpeedModifier);
    
        // Calculate the angle to rotate the projectile
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
    
        // Rotate the projectile to face its movement direction
        transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
    }
}
