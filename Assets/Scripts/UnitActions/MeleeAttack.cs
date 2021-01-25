using UnityEngine;
using System.Collections;

public class MeleeAttack : UnitAction
{
    public MeleeAttack(UnitBehavior unitBehavior) : base(unitBehavior)
    {
        this.actionRange = 1;
    }

    public override bool CanActOn(GameObject targetObject)
    {
        GameObject selfGameObject = this.unitBehavior.gameObject;
        return selfGameObject.CompareTag("Friendly") && targetObject.CompareTag("Enemy")
            || selfGameObject.CompareTag("Enemy") && targetObject.CompareTag("Friendly");
    }

    public override void ActOn(GameObject gameObject)
    {
        this.unitBehavior.MoveTo(gameObject.transform.position);
    }
}
