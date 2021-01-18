using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimationHandler : MonoBehaviour
{
    protected CatBehavior catBehavior;

    void Start()
    {
        this.catBehavior = this.gameObject.GetComponentInParent<CatBehavior>();
    }

    void AttackHit()
    {
        this.catBehavior.HandleAttackHitAnimationEvent();
    }

    void AttackFinish()
    {
        this.catBehavior.HandleAttackFinishAnimationEvent();
    }
}
