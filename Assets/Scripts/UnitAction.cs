using System;
using UnityEngine;

public abstract class UnitAction
{
    protected CatBehavior catBehavior;
    protected GameObject target;

    public int actionRange;

    public UnitAction(CatBehavior catBehavior)
    {
        this.catBehavior = catBehavior;
    }

    public abstract bool CanActOn(GameObject gameObject);

    public abstract void ActOn(GameObject gameObject);

    public bool IsTargetInRange()
    {
        return Vector3.Distance(this.catBehavior.gameObject.transform.position, this.target.transform.position) <= this.actionRange;
    }
}
