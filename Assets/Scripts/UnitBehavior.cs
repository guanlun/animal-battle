using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitBehavior : MonoBehaviour
{
    protected Transform catModelTransform;
    protected NavMeshAgent navMeshAgent;
    public Animator animator;
    protected GameObject selectedStateIndicator;
    protected GameObject attackTargetGameObject;
    protected HashSet<UnitBehavior> enemiesSpotted = new HashSet<UnitBehavior>();

    public int health = 100;
    public bool isDead = false;
    public bool isEnemyUnit;

    protected float detectionDistance = 10;
    protected float attackDistance = 2;

    protected List<UnitAction> actions = new List<UnitAction>();
    protected UnitAction currentAction;

    // Start is called before the first frame update
    void Awake()
    {
        this.catModelTransform = this.transform.Find("Cat");
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
        this.animator = this.catModelTransform.gameObject.GetComponent<Animator>();
        this.selectedStateIndicator = this.transform.Find("SelectedStateIndicator").gameObject;

        this.selectedStateIndicator.SetActive(false);

        this.isEnemyUnit = this.CompareTag("Enemy");

        this.actions.Add(new GatherResource(this));
        this.actions.Add(new MeleeAttack(this));
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isDead)
        {
            return;
        }

        if (this.currentAction != null && this.currentAction.IsTargetInRange())
        {
            this.navMeshAgent.isStopped = true;
            this.currentAction.Act();
        }

        //if (this.attackTargetGameObject)
        //{
        //    UnitBehavior targetBehavior = this.attackTargetGameObject.GetComponent<UnitBehavior>();

        //    if (targetBehavior.isDead)
        //    {
        //        //this.attackTargetGameObject = null;
        //        //this.animator.SetBool("isAttacking", false);
        //    }
        //    else
        //    {
        //        float distanceToTarget = Vector3.Distance(this.transform.position, this.attackTargetGameObject.transform.position);

        //        if (distanceToTarget < this.attackDistance)
        //        {
        //            this.navMeshAgent.isStopped = true;
        //            this.animator.SetBool("isAttacking", true);
        //        }
        //        else
        //        {
        //            this.navMeshAgent.isStopped = false;
        //            this.animator.SetBool("isAttacking", false);
        //            this.navMeshAgent.SetDestination(this.attackTargetGameObject.transform.position);
        //        }
        //    }
        //}

        if (this.animator.GetBool("isWalking") && !this.navMeshAgent.pathPending)
        {
            if (this.navMeshAgent.remainingDistance <= this.navMeshAgent.stoppingDistance)
            {
                if (!this.navMeshAgent.hasPath || this.navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    this.animator.SetBool("isWalking", false);
                }
            }
        }
    }

    public void SetSelectedState(bool isSelected)
    {
        this.selectedStateIndicator.SetActive(isSelected);
    }

    public void MoveTo(Vector3 destination)
    {
        this.navMeshAgent.isStopped = false;
        this.navMeshAgent.SetDestination(destination);

        this.navMeshAgent.isStopped = false;
        this.animator.SetBool("isWalking", true);
    }

    public void SetTarget(GameObject target)
    {
        foreach (UnitAction action in this.actions)
        {
            if (action.CanActOn(target))
            {
                action.SetTarget(target);

                this.currentAction = action;
            }
        }
    }

    public void SetAttackTarget(GameObject target)
    {
        this.attackTargetGameObject = target;
        this.navMeshAgent.isStopped = false;

        this.animator.SetBool("isWalking", true);
    }

    public void HandleAnimationEvent(string animationName)
    {
        if (this.currentAction != null)
        {
            this.currentAction.HandleAnimationEvent(animationName);
        }
    }

    public void HandleAttackHitAnimationEvent()
    {
        if (!this.attackTargetGameObject)
        {
            this.ChooseAttackTarget();
            return;
        }

        UnitBehavior targetBehavior = this.attackTargetGameObject.GetComponent<UnitBehavior>();

        if (targetBehavior)
        {
            targetBehavior.TakeHit(30);
        }
    }

    private void ChooseAttackTarget()
    {
        float minDistance = float.MaxValue;
        UnitBehavior closestEnemy = null;
        foreach (UnitBehavior enemy in this.enemiesSpotted)
        {
            if (enemy.isDead)
            {
                continue;
            }

            float distance = Vector3.Distance(enemy.transform.position, this.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy)
        {
            this.SetAttackTarget(closestEnemy.gameObject);
        }
    }

    public void HandleAttackFinishAnimationEvent()
    {
        if (!this.attackTargetGameObject || this.attackTargetGameObject.GetComponent<UnitBehavior>().isDead)
        {
            animator.SetBool("isAttacking", false);

            this.ChooseAttackTarget();
        }
    }

    public void TakeHit(int damage)
    {
        this.health -= damage;

        if (this.health <= 0)
        {
            this.isDead = true;
            this.gameObject.SetActive(false);
        }
    }

    public void OnDetectionRangeTriggerEnter(Collider other)
    {
        UnitBehavior otherUnitBehavior = other.gameObject.GetComponentInParent<UnitBehavior>();

        if (otherUnitBehavior && otherUnitBehavior.isEnemyUnit != this.isEnemyUnit)
        {
            this.enemiesSpotted.Add(otherUnitBehavior);
        }

        this.ChooseAttackTarget();
    }

    public void OnDetectionRangeTriggerExit(Collider other)
    {
        UnitBehavior otherUnitBehavior = other.gameObject.GetComponent<UnitBehavior>();

        if (this.enemiesSpotted.Contains(otherUnitBehavior))
        {
            this.enemiesSpotted.Remove(otherUnitBehavior);
        }
    }
}
