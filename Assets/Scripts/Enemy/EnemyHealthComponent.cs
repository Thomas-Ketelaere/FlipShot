using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealthComponent : HealthComponent
{
    [Header("Ragdoll")]
    [SerializeField] private Rigidbody[] _ragdollRigidbodies;

    private Animator _animator;
    private NavMeshAgent _agent;

    private const float RAGDOLL_HIT_STRENGTH = 50f;

    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        SetRagdollActive(false);
    }

    private void Update()
    {
        //_animator.SetTrigger("Damage02");
    }


    [ContextMenu("Populate Ragdoll Components")] //for not doing it manually
    private void PopulateRagdollComponents()
    {
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>()
            .Where(rb => rb.gameObject != gameObject)
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
        base.Die(direction, hitPoint);
        EnableRagdollWithForce(direction * RAGDOLL_HIT_STRENGTH, hitPoint);
    }

    public override void GetHit(Vector3 direction, Vector3 hitPoint)
    {
        Debug.Log("Enemy hit animation");
        _animator.SetTrigger("Damage02"); //TODO no magic value and should play random damage animation
        base.GetHit(direction, hitPoint);
    }
}
