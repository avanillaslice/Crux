using System.Collections;
using Project.Ships;
using UnityEngine;

namespace Project.Combat.Projectiles
{
    public class HomingMissile : ProjectileBase
    {
        public float lockOnDistance = 3f;
        public float lockOnYThreshold = 4.5f;
        public float initialRotationSpeed = 360f; // Degrees per second
        public float curveSpeed = 2f; // Speed at which the missile curves towards the target
        public float selfDestructTime = 10f; // Time after which the missile will self-destruct
        public float initialAngle = 45f; // Configurable initial angle in degrees

        private Transform target;

        protected override void InitializeBehaviour(Vector2 initialVelocity, RelativeSide side, Vector2? direction)
        {
            StartCoroutine(MoveMissile(initialVelocity, side));
        }

        private IEnumerator MoveMissile(Vector2 initialVelocity, RelativeSide side)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            // Determine the initial direction based on the side and initial angle
            float angle = (side == RelativeSide.Left) ? initialAngle : -initialAngle;
            Vector2 initialDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;
            rb.linearVelocity = initialVelocity + initialDirection * BaseSpeed * SpeedModifier;

            // Initial rotation
            float distanceTraveled = 0f;
            while (distanceTraveled < lockOnDistance)
            {
                distanceTraveled += rb.linearVelocity.magnitude * Time.deltaTime;
                transform.Rotate(0, 0, initialRotationSpeed * Time.deltaTime);
                yield return null;
            }

            // Lock onto the nearest enemy below the y-coordinate threshold
            target = FindTarget();
            float elapsedTime = 0f;
            while (elapsedTime < selfDestructTime)
            {
                if (target == null || !target.gameObject.activeInHierarchy)
                {
                    target = FindTarget();
                }

                if (target != null)
                {
                    // Move towards the target in a curved path
                    Vector2 directionToTarget = (target.position - transform.position).normalized;
                    Vector2 newVelocity = Vector2.Lerp(rb.linearVelocity, directionToTarget * BaseSpeed * SpeedModifier, curveSpeed * Time.deltaTime);
                    rb.linearVelocity = newVelocity;
                }

                // Rotate to face the direction of movement
                float currentAngle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle - 90)); // Adjust angle to match the missile's orientation

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Self-destruct after the timer expires
            Explode();
        }

        private Transform FindTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                if (enemy.transform.position.y < lockOnYThreshold)
                {
                    return enemy.transform;
                }
            }
            return null;
        }
    }
}
