using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimationHandler : MonoBehaviour
{
    protected UnitBehavior unitBehavior;

    void Start()
    {
        this.unitBehavior = this.gameObject.GetComponentInParent<UnitBehavior>();
    }

    void AttackHit()
    {
        //this.unitBehavior.HandleAttackHitAnimationEvent();
        this.unitBehavior.HandleAnimationEvent("AttackHit");
    }

    void AttackFinish()
    {
        //this.unitBehavior.HandleAttackFinishAnimationEvent();
    }
}
