using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShieldRegen : SkillBase
{
    public override string SkillName => "Shield Regen";
    public override string Description => "Increases the shield regeneration rate.";
    public override int MaxLevel => 3;

    public static readonly Dictionary<int, float> levelEffects = new Dictionary<int, float>
    {
        { 1, 0.025f }, // 2.5% per second
        { 2, 0.05f },  // 5% per second
        { 3, 0.1f }    // 10% per second
    };

    private bool isRegenerating = false;
    private Coroutine regenCoroutine;

    public ShieldRegen(int level) : base(level) { }

    public override void Activate()
    {
        TargetShip.OnHit += OnHit;
        TargetShip.OnUpdate += OnUpdate;
    }

    private float DetermineRegenRate()
    {
        return GetAmountAffected(Level);
    }

    private void OnHit()
    {
        isRegenerating = false;
        if (regenCoroutine != null)
        {
            TargetShip.StopCoroutine(regenCoroutine);
        }
        regenCoroutine = TargetShip.StartCoroutine(StartRegenCountdown());
    }

    private IEnumerator StartRegenCountdown()
    {
        yield return new WaitForSeconds(5f);
        isRegenerating = true;
    }

    private void OnUpdate()
    {
        if (isRegenerating && TargetShip.Shield < TargetShip.MaxShield)
        {
            float regenAmount = TargetShip.MaxShield * DetermineRegenRate() * Time.deltaTime;
            TargetShip.AddShield(regenAmount);
        }
    }

    public override void Deactivate()
    {
        // TargetShip.OnHit -= OnHit;
        // TargetShip.OnUpdate -= OnUpdate;
        // if (regenCoroutine != null)
        // {
        //     TargetShip.StopCoroutine(regenCoroutine);
        // }
    }

    public static float GetAmountAffected(int level)
    {
        if (levelEffects.TryGetValue(level, out float effect))
        {
            return effect;
        }
        else
        {
            Debug.LogError("Shield Regen level is invalid");
            return 0.025f; // Default value or error handling
        }
    }
}