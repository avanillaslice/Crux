using System;

public abstract class ToggleFireWeaponBase : WeaponBase
{
    public override void AttemptFire(bool isEnemy)
    {
        Fire(isEnemy);
    }

    public override void AttemptCeaseFire()
    {
        ShipBase ship = GetComponentInParent<ShipBase>();
        CeaseFire(OnCeaseFireCompleted);
    }

    protected abstract void Fire(bool isEnemy);
    protected abstract void CeaseFire(Action onCompleted);
    protected virtual void OnCeaseFireCompleted()
    {
        ShipBase ship = GetComponentInParent<ShipBase>();
        if (ship != null) ship.HandleSpecialFireCeased();
    }
}
