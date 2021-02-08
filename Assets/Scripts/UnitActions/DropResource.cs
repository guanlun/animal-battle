using System;
using UnityEngine;

public class DropResource : UnitAction
{
    public DropResource(UnitBehavior unitBehavior) : base(unitBehavior)
    {
        this.actionName = "DropResource";
        this.actionRange = 2;
    }

    public override void Act()
    {
        throw new NotImplementedException();
    }

    public override bool CanActOn(GameObject targetObject)
    {
        return targetObject.name == "Base";
    }

    public override void SetTarget(GameObject gameObject)
    {
        this.target = gameObject;
        this.unitBehavior.MoveTo(gameObject.transform.position);
    }
}
