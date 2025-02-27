using UnityEngine;

public class ShieldDrone : DroneShip
{
    protected override void ActivateEffect()
    {
        if (CurrentBehavior == DroneBehavior.Aggressive)
        {
            EnableSpecialFire();
        }
        else
        {
            DisableSpecialFire();
        }
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
}
