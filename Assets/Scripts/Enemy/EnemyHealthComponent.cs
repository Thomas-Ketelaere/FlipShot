using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealthComponent : HealthComponent
{
    [Header("Ragdoll")]
    [SerializeField] private Rigidbody[] _ragdollRigidbodies;

    private Animator _animator;
    private NavMeshAgent _agent;

    private const float RAGDOLL_BULLET_HIT_STRENGTH = 50f;
    private const float RAGDOLL_GRENADE_HIT_STRENGTH = 50f;
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

    public void EnableRagdollWithExplosionForce(Vector3 force, Vector3 explosionPoint)
    {
        EnableRagdoll();

        foreach (Rigidbody rb in _ragdollRigidbodies)
        {
            //rb.AddExplosionForce(force, explosionPoint, explosionRadius, 0.5f, ForceMode.Impulse);
            rb.AddForceAtPosition(force, explosionPoint, ForceMode.Impulse);
        }
    }

    protected override void Die(Vector3 direction, Vector3 hitPoint, DamageSource damageSource)
    {
        base.Die(direction, hitPoint, damageSource);
        switch (damageSource)
        {
            case DamageSource.Weapon:
                EnableRagdollWithForce(direction * RAGDOLL_BULLET_HIT_STRENGTH, hitPoint);
                break;
            case DamageSource.Grenade:
                EnableRagdollWithExplosionForce(direction * RAGDOLL_GRENADE_HIT_STRENGTH, hitPoint);
                break;
        }

        
    }

    public override void GetHit(Vector3 direction, Vector3 hitPoint, DamageSource damageSource, bool playBloodVFX = false, Vector3 hitPointNormal = default)
    {
        _animator.SetTrigger(ANIMATION_DAMAGE_NAME);
        base.GetHit(direction, hitPoint, damageSource);
    }
}
