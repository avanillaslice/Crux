using UnityEngine;

public class ShipEffect : EffectBase
{
    public override void Activate(GameObject targetShip)
    {
        TargetShip = targetShip;
        switch (SubType)
        {
            case EffectSubType.FireRate:
                ActivateFireRateEffect();
                break;
            case EffectSubType.Health:
                ActivateHealthEffect();
                break;
            case EffectSubType.Damage:
                ActivateDamageEffect();
                break;
            default:
                Debug.LogError("Unknown effect sub type");
                return;
        }

        if (Expiry == ExpiryType.Time && Duration > 0) {
            Debug.Log($"Expiry deteceted for {gameObject.name} with duration {Duration}");
            CoroutineManager.Inst.DeactivateEffectAfterDelay(this, Duration);
        }
    }

    public override void Deactivate()
    {
        switch (SubType)
        {
            case EffectSubType.FireRate:
                DeactivateFireRateEffect();
                break;
            case EffectSubType.Damage:
                DeactivateDamageEffect();
                break;
            default:
                Debug.LogError("Unknown effect sub type");
                return;
        }
    }

    private void ActivateHealthEffect()
    {
        ShipBase ship = TargetShip.GetComponent<ShipBase>();
        ship.AddHealth(Amt);
    }

    private void ActivateDamageEffect()
    {
        TargetShip.GetComponent<ShipBase>().DamageModifier += Amt;
    }

    private void DeactivateDamageEffect()
    {
        TargetShip.GetComponent<ShipBase>().DamageModifier -= Amt;
    }

    private void ActivateFireRateEffect()
    {
        TargetShip.GetComponent<ShipBase>().FireRateModifier += Amt;
    }

    private void DeactivateFireRateEffect()
    {
        TargetShip.GetComponent<ShipBase>().FireRateModifier -= Amt;
    }
}