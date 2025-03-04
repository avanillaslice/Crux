using System.Collections;
using Project.Core;
using Project.Ships;
using UnityEngine;

namespace Project.Combat.Projectiles
{
    public class PlasmaHeavy : ProjectileBase
    {
        public float SplitTime = 1f; // Adjustable time before splitting
        public int PlasmaCount = 3; // Adjustable number of Plasma projectiles to spawn
        public GameObject PlasmaPrefab; // Assign this in the inspector

        void Awake()
        {
            PlasmaPrefab = AssetManager.GetProjectilePrefab("Plasma");
            if (PlasmaPrefab == null) {
                Debug.LogError("PlasmaHeavy: PlasmaPrefab is not assigned!");
            }
        }

        protected override void InitializeBehaviour(Vector2 initialVelocity, RelativeSide side, Vector2? direction)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            // Set the velocity of the projectile
            rb.linearVelocity = initialVelocity + (Vector2)(transform.up * BaseSpeed * SpeedModifier);

            // Start the coroutine for splitting
            StartCoroutine(SplitAfterDelay());
        }

        private IEnumerator SplitAfterDelay()
        {
            yield return new WaitForSeconds(SplitTime);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 currentDirection = rb.linearVelocity.normalized;
            float angleStep = 90f / (PlasmaCount - 1); // Maximum 45 degrees on each side

            for (int i = 0; i < PlasmaCount; i++)
            {
                float angle = -45f + (angleStep * i);
                Vector2 newDirection = Quaternion.Euler(0, 0, angle) * currentDirection;

                GameObject plasmaObj = Instantiate(PlasmaPrefab, transform.position, Quaternion.identity);
                Plasma plasmaScript = plasmaObj.GetComponent<Plasma>();
            
                if (plasmaScript != null)
                {
                    Vector2 initialVelocity = rb != null ? rb.linearVelocity : Vector2.zero;
                    // ! Replace with shrapnel projectile to prevent speed modifier conflicts
                    plasmaScript.Initialize(FiredByEnemy, 0.75f, DamageModifier, PiercingModifier, CriticalHitChanceModifier, initialVelocity, RelativeSide.Center, newDirection);
                }
            }

            Destroy(gameObject);
        }
    }
}
