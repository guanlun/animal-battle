using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    protected Transform catModelTransform;
    protected Transform targetCatTransform;
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    protected int health = 100;

    protected string currentState = "Idle";

    // Start is called before the first frame update
    void Start()
    {
        this.targetCatTransform = GameObject.Find("MainCat").transform;
        this.navMeshAgent = GetComponent<NavMeshAgent>();

        this.catModelTransform = this.transform.Find("Cat");
        this.animator = this.catModelTransform.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (this.health <= 0)
        //{
        //    this.navMeshAgent.isStopped = true;
        //    return;
        //}

        //float distanceToTarget = Vector3.Distance(this.transform.position, this.targetCatTransform.position);

        //RaycastHit hit;
        //Physics.Raycast(this.transform.position, this.targetCatTransform.position - this.transform.position, out hit);

        //if (hit.collider != null && hit.collider.gameObject.name != "MainCat")
        //{
        //    return;
        //}

        //if (distanceToTarget < 2)
        //{
        //    this.navMeshAgent.isStopped = true;

        //    if (this.animator.GetBool("isWalking"))
        //    {
        //        this.animator.SetBool("isWalking", false);
        //    }
        //}
        //else if (distanceToTarget < 10)
        //{
        //    this.navMeshAgent.isStopped = false;

        //    this.transform.rotation = Quaternion.LookRotation(this.targetCatTransform.position - this.transform.position);
        //    this.navMeshAgent.SetDestination(this.targetCatTransform.position);

        //    if (!this.animator.GetBool("isWalking"))
        //    {
        //        this.animator.SetBool("isWalking", true);
        //    }
        //}
    }

    public void TakeHit(int damage)
    {
        this.health -= damage;
    }
}
