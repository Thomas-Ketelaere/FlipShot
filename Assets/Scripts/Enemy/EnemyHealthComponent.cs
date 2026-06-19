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
    private const string ANIMATION_DAMAGE_NAME = "Damage02";

    protected override void Start()
    {
        base.Start();
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

    }

    public void EnableRagdollWithForce(Vector3 force, Vector3 hitPoint)
    {
        EnableRagdoll();

        Rigidbody closestBone = _ragdollRigidbodies
            .OrderBy(rb => Vector3.Distance(rb.position, hitPoint))
            .First();

        closestBone.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    protected override void Die(Vector3 direction, Vector3 hitPoint, Vector3 hitPointNormal)
    {
        base.Die(direction, hitPoint, hitPointNormal);
        EnableRagdollWithForce(direction * RAGDOLL_HIT_STRENGTH, hitPoint);
    }

    public override void GetHit(Vector3 direction, Vector3 hitPoint, Vector3 hitPointNormal)
    {
        _animator.SetTrigger(ANIMATION_DAMAGE_NAME);
        base.GetHit(direction, hitPoint, hitPointNormal);
    }
}
