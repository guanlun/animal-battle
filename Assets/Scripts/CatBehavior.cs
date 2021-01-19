using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatBehavior : MonoBehaviour
{
    protected Transform catModelTransform;
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    protected GameObject selectedStateIndicator;
    protected GameObject attackTargetGameObject;
    protected HashSet<CatBehavior> enemiesSpotted = new HashSet<CatBehavior>();
    
    protected int health = 100;
    protected bool isDead = false;
    protected bool isEnemyUnit;

    protected float detectionDistance = 10;
    protected float attackDistance = 2;

    // Start is called before the first frame update
    void Awake()
    {
        this.catModelTransform = this.transform.Find("Cat");
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
        this.animator = this.catModelTransform.gameObject.GetComponent<Animator>();
        this.selectedStateIndicator = this.transform.Find("SelectedStateIndicator").gameObject;

        this.selectedStateIndicator.SetActive(false);

        this.isEnemyUnit = this.CompareTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isDead)
        {
            return;
        }
        //if (isMoving)
        //{
        //    this.catModelTransform.rotation = Quaternion.LookRotation(facingDirection);
        //    this.characterController.Move(facingDirection.normalized * (isRunning ? RUN_SPEED : WALK_SPEED));
        //}

        if (this.attackTargetGameObject)
        {
            CatBehavior targetBehavior = this.attackTargetGameObject.GetComponent<CatBehavior>();

            if (targetBehavior.isDead)
            {
                //this.attackTargetGameObject = null;
                //this.animator.SetBool("isAttacking", false);
            }
            else
            {
                float distanceToTarget = Vector3.Distance(this.transform.position, this.attackTargetGameObject.transform.position);

                if (distanceToTarget < this.attackDistance)
                {
                    this.navMeshAgent.isStopped = true;
                    this.animator.SetBool("isAttacking", true);
                }
                else
                {
                    this.navMeshAgent.isStopped = false;
                    this.animator.SetBool("isAttacking", false);
                    this.navMeshAgent.SetDestination(this.attackTargetGameObject.transform.position);
                }
            }
        }

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

    public void SetAttackTarget(GameObject target)
    {
        this.attackTargetGameObject = target;
        this.navMeshAgent.isStopped = false;

        this.animator.SetBool("isWalking", true);
    }

    public void HandleAttackHitAnimationEvent()
    {
        if (!this.attackTargetGameObject)
        {
            this.ChooseAttackTarget();
            return;
        }

        CatBehavior targetBehavior = this.attackTargetGameObject.GetComponent<CatBehavior>();

        if (targetBehavior)
        {
            targetBehavior.TakeHit(30);
        }
    }

    private void ChooseAttackTarget()
    {
        float minDistance = float.MaxValue;
        CatBehavior closestEnemy = null;
        foreach (CatBehavior enemy in this.enemiesSpotted)
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
        if (!this.attackTargetGameObject || this.attackTargetGameObject.GetComponent<CatBehavior>().isDead)
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
        CatBehavior otherCatBehavior = other.gameObject.GetComponentInParent<CatBehavior>();

        if (otherCatBehavior && otherCatBehavior.isEnemyUnit != this.isEnemyUnit)
        {
            this.enemiesSpotted.Add(otherCatBehavior);
        }

        this.ChooseAttackTarget();
    }

    public void OnDetectionRangeTriggerExit(Collider other)
    {
        CatBehavior otherCatBehavior = other.gameObject.GetComponent<CatBehavior>();

        if (this.enemiesSpotted.Contains(otherCatBehavior))
        {
            this.enemiesSpotted.Remove(otherCatBehavior);
        }
    }
}
