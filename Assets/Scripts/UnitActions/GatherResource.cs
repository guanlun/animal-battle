using UnityEngine;

public class GatherResource : UnitAction
{
    public bool hasGathered = false;
    private GameObject resourceObject;

    public GatherResource(UnitBehavior unitBehavior) : base(unitBehavior)
    {
        this.actionName = "GatherResource";
        this.actionRange = 2;
    }

    public override bool CanActOn(GameObject gameObject)
    {
        return gameObject.CompareTag("Resource");
    }

    public override void SetTarget(GameObject gameObject)
    {
        if (!hasGathered)
        {
            this.resourceObject = gameObject;
        }
        this.target = gameObject;
        this.unitBehavior.MoveTo(gameObject.transform.position);
    }

    public override void Act()
    {
        if (this.hasGathered)
        {
            this.hasGathered = false;
            this.SetTarget(this.resourceObject);
        }
        else
        {
            this.unitBehavior.animator.SetBool("isWalking", false);
            this.unitBehavior.animator.SetBool("isGathering", true);
        }
    }

    public override void HandleAnimationEvent(string animationName)
    {
        if (animationName == "GatherFinish")
        {
            this.unitBehavior.animator.SetBool("isGathering", false);
            
            //this.unitBehavior.DropResource();
            this.hasGathered = true;

            GameObject baseBuilding = GameObject.Find("Base");

            if (baseBuilding)
            {
                this.SetTarget(baseBuilding);
            }
        }
    }
}
