using UnityEngine;

public class AttackDrone : DroneShip
{
    [SerializeField] private float chargeDrainRate = 20f; // Charge drained per second in Aggressive mode
    private GameObject currentTarget;

    protected override void ActivateEffect()
    {
        if (CurrentBehavior == DroneBehavior.Aggressive)
        {
            EnablePrimaryFire();
        }
        else
        {
            DisablePrimaryFire();
            ResetRotation();
        }
    }

    protected override void Update()
    {
        base.Update(); // Call the base class Update method
        
        if (CurrentBehavior == DroneBehavior.Aggressive)
        {
            DrainCharge();
            if (AdvancedTargetting)
            {
                RotateTowardsClosestEnemy();
            }
        }
    }

    private void DrainCharge()
    {
        SubtractCharge(chargeDrainRate * Time.deltaTime);
    }

    protected override void MoveDrone()
    {
        if (curveProgress >= 1f)
        {
            currentLocalTarget = nextLocalTarget;
            nextLocalTarget = PickNewRelativeLocation();
            curveProgress = 0f;
        }

        curveProgress += Time.deltaTime / curveDuration;
        Vector3 newPosition = CalculateBezierPoint(curveProgress, currentLocalTarget, Vector3.zero, nextLocalTarget);

        if (CurrentBehavior == DroneBehavior.Aggressive)
        {
            newPosition = ParentShip.transform.TransformPoint(newPosition);
        }
        else
        {
            newPosition = ParentDroneAnchor.transform.TransformPoint(newPosition);
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 5f * MovementSpeedModifier);
    }

    private void RotateTowardsClosestEnemy()
    {
        if (currentTarget == null || !IsWithinBounds(currentTarget.transform.position) || !currentTarget.activeInHierarchy)
        {
            currentTarget = FindClosestEnemy();
            if (currentTarget != null)
            {
                // Debug.Log($"New target acquired: {currentTarget.name}");
            }
        }
    
        if (currentTarget != null)
        {
            Rigidbody2D targetRigidbody = currentTarget.GetComponent<Rigidbody2D>();
            if (targetRigidbody != null)
            {
                Vector3 targetVelocity = targetRigidbody.velocity;
                float timeToTarget = Vector3.Distance(transform.position, currentTarget.transform.position) / MovementSpeedModifier;
                Vector3 futurePosition = currentTarget.transform.position + targetVelocity * timeToTarget;
    
                Vector3 direction = futurePosition - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 270f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), Time.deltaTime * 5f);
            }
            else
            {
                // Fallback to the current position if no Rigidbody2D is found
                Vector3 direction = currentTarget.transform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 270f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), Time.deltaTime * 5f);
            }
        }
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (IsWithinBounds(enemy.transform.position))
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    private bool IsWithinBounds(Vector3 position)
    {
        return position.x >= -6.7f && position.x <= 6.7f && position.y >= -5f && position.y <= 5f;
    }

    private void ResetRotation()
    {
        // Reset the rotation to point straight up
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
