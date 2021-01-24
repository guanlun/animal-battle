using System;
using UnityEngine;

public class GatherResource : UnitAction
{
    public GatherResource(CatBehavior catBehavior) : base(catBehavior)
    {
        this.actionRange = 1;
    }

    public override bool CanActOn(GameObject gameObject)
    {
        return gameObject.CompareTag("Resource");
    }

    public override void ActOn(GameObject gameObject)
    {
        this.catBehavior.MoveTo(gameObject.transform.position);
    }
}
