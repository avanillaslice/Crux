using Project.Ships;
using UnityEngine;

namespace Project.Combat.Weapons
{
    public class TurretSmall : SingleFireWeaponBase
    {
        private Transform target;
        public float rotationSpeed = 1.5f; // Adjust this value to change rotation speed
        public float MaxTargetRange = 6f;
        public float LeadTime = 2.5f; // Public variable for adjusting how far ahead to aim

        private Rigidbody2D targetRigidbody;

        protected override void Update()
        {
            base.Update();
            FindTarget();
            AimAtTarget();
        }

        private void FindTarget()
        {
            if (target == null || target.gameObject == null)
            {
                EnemyShip[] enemies = FindObjectsOfType<EnemyShip>();
                if (enemies.Length > 0)
                {
                    // Find the closest enemy
                    foreach (EnemyShip enemy in enemies)
                    {
                        float distance = Vector3.Distance(transform.position, enemy.transform.position);
                        if (distance < MaxTargetRange && !enemy.TargetedByTurret)
                        {
                            target = enemy.transform;
                            enemy.TargetedByTurret = true;
                            targetRigidbody = enemy.GetComponent<Rigidbody2D>();
                            break;
                        }
                    }
                }
                else
                {
                    target = null;
                    targetRigidbody = null;
                }
            }
        }

        private void AimAtTarget()
        {
            if (target != null && targetRigidbody != null)
            {
                Vector3 predictedPosition = PredictTargetPosition();
                Vector3 direction = predictedPosition - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                transform.rotation = rotation;
                // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Point forward if no target is found
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, rotationSpeed * Time.deltaTime);
            }
        }

        private Vector3 PredictTargetPosition()
        {
            Vector3 targetVelocity = targetRigidbody.linearVelocity;
            return target.position + targetVelocity * LeadTime;
        }
    }
}
