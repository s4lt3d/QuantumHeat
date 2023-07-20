using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    public Transform playerTarget;
    public Transform playerHead;
    public float stopDistance = 5;
    public FireBulletOnActivate gun;
    private Quaternion localRotationGun;

    void Start()
    {
        InitializeComponents();
        localRotationGun = gun.spawnPoint.localRotation;
    }

    void Update()
    {
        UpdateAgentDestination();
        StopAndShootWhenNearPlayer();
    }

    // Initialize NavMeshAgent and Animator components
    private void InitializeComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        SetupRagdoll();
    }

    // Set destination of NavMeshAgent to the player's position
    private void UpdateAgentDestination()
    {
        agent.SetDestination(playerTarget.position);
    }

    // Stop NavMeshAgent and trigger shooting when close to the player
    private void StopAndShootWhenNearPlayer()
    {
        float distance = Vector3.Distance(playerTarget.position, transform.position);
        if (distance < stopDistance)
        {
            agent.isStopped = true;
            animator.SetBool("Shoot", true);
        }
    }

    public void ThrowGun()
    {
        SetGunOrientation();
        DetachGunFromParent();
        PropelGun();
    }

    // Set the gun's orientation
    private void SetGunOrientation()
    {
        gun.spawnPoint.localRotation = localRotationGun;
    }

    // Detach the gun from its parent object
    private void DetachGunFromParent()
    {
        gun.transform.parent = null;
    }

    // Propel the gun towards the player
    private void PropelGun()
    {
        Rigidbody rb = gun.GetComponent<Rigidbody>();
        rb.velocity = BallisticVelocityVector(gun.transform.position, playerHead.position, 45);
        rb.angularVelocity = Vector3.zero;
    }

    Vector3 BallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = CalculateDirection(source, target, angle);
        return CalculateVelocity(direction, angle);
    }

    // Calculate the direction of the shot
    private Vector3 CalculateDirection(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);
        return direction;
    }

    // Calculate the velocity of the shot
    private Vector3 CalculateVelocity(Vector3 direction, float angle)
    {
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }

    public void ShootEnemy()
    {
        Vector3 playerHeadPosition = playerHead.position - Random.Range(0, 0.4f) * Vector3.up;
        OrientGunTowardsPlayer(playerHeadPosition);
        gun.FireBullet();
    }

    // Orient the gun towards the player's position
    private void OrientGunTowardsPlayer(Vector3 playerHeadPosition)
    {
        gun.spawnPoint.forward = (playerHeadPosition - gun.spawnPoint.position).normalized;
    }

    public void SetupRagdoll()
    {
        foreach (var item in GetComponentsInChildren<Rigidbody>())
        {
            item.isKinematic = true;
        }
    }

    public void Dead(Vector3 hitPosition)
    {
        EnableRagdoll();
        ApplyExplosionForce(hitPosition);
        ThrowGun();
        DisableAnimationAndNavigation();
    }

    // Enable ragdoll effect by setting isKinematic to false
    private void EnableRagdoll()
    {
        foreach (var item in GetComponentsInChildren<Rigidbody>())
        {
            item.isKinematic = false;
        }
    }

    // Apply explosion force to surrounding objects
    private void ApplyExplosionForce(Vector3 hitPosition)
    {
        foreach (var item in Physics.OverlapSphere(hitPosition, 0.3f))
        {
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddExplosionForce(1000, hitPosition, 0.3f);
            }
        }
    }

    // Disable animation, navigation and the script after enemy's death
    private void DisableAnimationAndNavigation()
    {
        animator.enabled = false;
        agent.enabled = false;
        this.enabled = false;
    }
}
