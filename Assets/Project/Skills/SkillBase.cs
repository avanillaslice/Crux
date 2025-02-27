using System;
using UnityEngine;
public abstract class SkillBase
{
    public int Level;
    public abstract string SkillName { get; }
    public abstract string Description { get; }
    public abstract int MaxLevel { get; }
    public ShipBase TargetShip;
    public bool IsActive;

    public SkillBase(int level)
    {
        Level = level;
    }

    public void AttemptActivation(ShipBase targetShip)
    {
        if (Level > MaxLevel) throw new Exception("Cannot activate skill: " + SkillName + ",  level too high");
        if (Level == 0) throw new Exception("Cannot activate skill: " + SkillName + " at level zero");

        // Override TargetShip if provided
        if (targetShip != null) TargetShip = targetShip;
        if (TargetShip == null) throw new Exception("Cannot activate skill: " + SkillName + ", target ship is null");

        Activate();
        Debug.Log(SkillName + " ACTIVATED AT LEVEL: " + Level);
    }

    public void Upgrade()
    {
        if (Level == MaxLevel) throw new Exception("Cannot upgrade skill, level is maxed");
        Level += 1;
        Debug.Log(SkillName + " UPGRADED TO LEVEL: " + Level);
    }

    public abstract void Activate();
    public abstract void Deactivate();
}