using UnityEngine;
using System.Collections;

public class MeleeAttack : UnitAction
{
    public MeleeAttack(UnitBehavior unitBehavior) : base(unitBehavior)
    {
        this.actionName = "MeleeAttack";
        this.actionRange = 2;
    }

    public override bool CanActOn(GameObject targetObject)
    {
        GameObject selfGameObject = this.unitBehavior.gameObject;
        return selfGameObject.CompareTag("Friendly") && targetObject.CompareTag("Enemy")
            || selfGameObject.CompareTag("Enemy") && targetObject.CompareTag("Friendly");
    }

    public override void SetTarget(GameObject gameObject)
    {
        this.target = gameObject;
        
        this.unitBehavior.MoveTo(gameObject.transform.position);
    }

    public override void Act()
    {
        if (this.target == null || this.target.GetComponent<UnitBehavior>().isDead)
        {
            return;
        }

        this.unitBehavior.animator.SetBool("isAttacking", true);
    }

    public override void HandleAnimationEvent(string animationName)
    {
        if (animationName == "AttackHit")
        {
            Debug.Log("hit");
        }
    }
}
