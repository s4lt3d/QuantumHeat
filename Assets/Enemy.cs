using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform playerTarget;
    public float stopDistance = 5f;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();    
        SetupRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerTarget.position);
        float distance = Vector3.Distance(playerTarget.position, transform.position);
        if (distance < stopDistance) { 
            agent.isStopped = true;
            animator.SetBool("Shoot", true);
        }
    }

    public void SetupRagdoll()
    {
        foreach(var item in GetComponentsInChildren<Rigidbody>())
        {
            item.isKinematic = true;
        }

    }

    public void Dead(Vector3 hitPosition)
    {
        foreach (var item in GetComponentsInChildren<Rigidbody>())
        {
            item.isKinematic = false;
        }

        foreach (var item in Physics.OverlapSphere(hitPosition, 0.3f))
        {
            Rigidbody rb = item.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(1000, hitPosition, 10f);
            }
        }


    }

}
