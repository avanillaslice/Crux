using UnityEngine;

public enum EffectType
{
    Passive,
    Weapon
}

public enum EffectSubType
{
    // Points,
    FireRate,
    // Lives,
    Damage,
    Health,
    MissileLauncher
}

public enum ExpiryType
{
    Never,
    Time,
    Death,
}

public abstract class EffectBase : MonoBehaviour
{
    public EffectType Type;
    public EffectSubType SubType;
    public ExpiryType Expiry;
    public float Duration;
    public float Amt;
    protected GameObject TargetShip;

    public abstract void Activate(GameObject target);
    public abstract void Deactivate();
}
