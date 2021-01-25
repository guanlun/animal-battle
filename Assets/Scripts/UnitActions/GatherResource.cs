using System;
using UnityEngine;

public class GatherResource : UnitAction
{
    public GatherResource(UnitBehavior unitBehavior) : base(unitBehavior)
    {
        this.actionRange = 1;
    }

    public override bool CanActOn(GameObject gameObject)
    {
        return gameObject.CompareTag("Resource");
    }

    public override void ActOn(GameObject gameObject)
    {
        this.unitBehavior.MoveTo(gameObject.transform.position);
    }
}
