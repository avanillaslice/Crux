using UnityEngine;
public abstract class SkillBase
{
    public int Level;
    public string SkillName;
    protected virtual int MaxLevel { get; set; }
    public ShipBase TargetShip;
    public bool IsActive;


    public SkillBase(int level)
    {
        Level = level;
    }

    public void AttemptActivation(ShipBase targetShip)
    {
        if (Level > MaxLevel)
        {
            Debug.LogError("Cannot activate skill: " + SkillName + ",  level too high");
            return;
        }
        if (Level == 0)
        {
            Debug.LogError("Cannot activate skill: " + SkillName + " at level zero");
            return;
        }

        // Override TargetShip if provided
        if (targetShip != null) TargetShip = targetShip;

        if (TargetShip != null) 
        {
            Activate();
            Debug.Log(SkillName + " ACTIVATED AT LEVEL: " + Level);
            return;
        }

        Debug.LogError("Cannot activate skill, TargetShip is null");
    }

    public void Upgrade()
    {
        if (Level == MaxLevel)
        {
            Debug.LogError("Cannot upgrade skill, level is maxed");
            return;
        }
        Level += 1;
    }

    public abstract void Activate();
    public abstract void Deactivate();
}