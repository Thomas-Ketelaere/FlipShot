using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealthComponent : HealthComponent
{
    [Header("Ragdoll")]
    [SerializeField] private Rigidbody[] _ragdollRigidbodies;
    [SerializeField] private Collider[] _ragdollColliders;

    private Animator _animator;
    private NavMeshAgent _agent;

    private const float RAGDOLL_HIT_STRENGTH = 50f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        SetRagdollActive(false);
    }


    [ContextMenu("Populate Ragdoll Components")] //for not doing it manually
    private void PopulateRagdollComponents()
    {
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>()
            .Where(rb => rb.gameObject != gameObject)
            .ToArray();

        _ragdollColliders = GetComponentsInChildren<Collider>()
            .Where(col => col.gameObject != gameObject)
            .ToArray();
    }

    public void EnableRagdoll()
    {
        _animator.enabled = false;
        _agent.enabled = false;

        SetRagdollActive(true);
    }

    private void SetRagdollActive(bool active)
    {
        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = !active;
        }

        //going to use colliders for injury system
        //foreach (var col in _ragdollColliders)
        //{
        //    col.enabled = active;
        //}
    }

    public void EnableRagdollWithForce(Vector3 force, Vector3 hitPoint)
    {
        EnableRagdoll();

        Rigidbody closestBone = _ragdollRigidbodies
            .OrderBy(rb => Vector3.Distance(rb.position, hitPoint))
            .First();

        closestBone.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    protected override void Die(Vector3 direction, Vector3 hitPoint)
    {
        EnableRagdollWithForce(direction * RAGDOLL_HIT_STRENGTH, hitPoint);
    }
}
