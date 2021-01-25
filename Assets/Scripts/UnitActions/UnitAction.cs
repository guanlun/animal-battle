﻿using System;
using UnityEngine;

public abstract class UnitAction
{
    protected UnitBehavior unitBehavior;
    protected GameObject target;

    public int actionRange;

    public UnitAction(UnitBehavior unitBehavior)
    {
        this.unitBehavior = unitBehavior;
    }

    public abstract bool CanActOn(GameObject targetObject);

    public abstract void ActOn(GameObject targetObject);

    public bool IsTargetInRange()
    {
        return Vector3.Distance(this.unitBehavior.gameObject.transform.position, this.target.transform.position) <= this.actionRange;
    }
}