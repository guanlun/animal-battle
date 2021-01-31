using UnityEngine;

public class GatherResource : UnitAction
{
    public GatherResource(UnitBehavior unitBehavior) : base(unitBehavior)
    {
        this.actionRange = 2;
    }

    public override bool CanActOn(GameObject gameObject)
    {
        return gameObject.CompareTag("Resource");
    }

    public override void SetTarget(GameObject gameObject)
    {
        this.target = gameObject;
        this.unitBehavior.MoveTo(gameObject.transform.position);
    }

    public override void Act()
    {
        Debug.Log("gather");
        this.unitBehavior.animator.SetBool("isWalking", false);
        this.unitBehavior.animator.SetBool("isGathering", true);
    }

    public override void HandleAnimationEvent(string animationName)
    {
    }
}
