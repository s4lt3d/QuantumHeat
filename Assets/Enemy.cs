using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform playerTarget;
    public Transform playerHead;
    public float stopDistance = 5f;
    private Animator animator;
    public FireBulletOnActivate gun;

    Quaternion localGunRotation;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();    
        SetupRagdoll();
        localGunRotation = gun.spawnPoint.transform.localRotation;
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

        TossGun();
        animator.enabled = false;
        agent.enabled = false;
        this.enabled = false;

    }


    public void ShootEnemy()
    {
        Vector3 playerHeadPos = playerHead.position + Random.RandomRange(-0.4f, 0.0f) * Vector3.up;

        gun.spawnPoint.forward = (playerHeadPos - gun.spawnPoint.position).normalized;

        gun.FireBullet();
    }

    public void TossGun()
    {
        gun.transform.parent = null;
        gun.spawnPoint.localRotation = localGunRotation;
        Rigidbody rb = gun.GetComponent<Rigidbody>();

        rb.velocity = BallisticVector(gun.transform.position, playerHead.position, 45);
        rb.angularVelocity = Vector3.zero;
    }


    Vector3 BallisticVector(Vector3 source, Vector3 target, float angle)
    {

        Vector3 dir = target - source;
        float h = dir.y;
        dir.y = 0;
        float distance = dir.magnitude;
        angle *= Mathf.Deg2Rad;
        dir.y = distance * Mathf.Tan(angle);
        distance += h / Mathf.Tan(angle);

        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * angle));
        return velocity * dir.normalized;
    }

}
