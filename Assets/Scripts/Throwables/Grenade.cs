using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float _farBlastRadius = 4f; //only gets hit
    [SerializeField] private float _closeBlastRadius = 2f; //gets insta killed
    [SerializeField] private float _blastForce = 100f;

    private int _layerMask;

    private void Start()
    {
        _layerMask = LayerMask.GetMask("Damageable"); //pref here also env damage (stretch goal)

        //TEMP
        Invoke("Explode", 3f);
    }

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _farBlastRadius, _layerMask);

        // Deduplicate: multiple parts hit on the same character should only affect that character once
        // Key = the EnemyHealth root, Value = the closest hit point for force direction
        Dictionary<HealthComponent, Vector3> uniqueCharacters = new();

        foreach (Collider collider in colliders)
        {
            InjuryPart part = collider.GetComponent<InjuryPart>();
            if (part == null)
            {
                Debug.LogError("[Grenade] Hittable collider missing InjuryPart: " + collider.gameObject.name);
                continue;
            }

            HealthComponent health = part.Health;

            // Only register each character once, keep the hit point of the first/closest part
            if (!uniqueCharacters.ContainsKey(health))
                uniqueCharacters.Add(health, collider.ClosestPoint(transform.position));
        }

        foreach (var (health, hitPoint) in uniqueCharacters)
        {
            float distance = Vector3.Distance(health.transform.position, transform.position);
            bool isClose = distance <= _closeBlastRadius;

            // Direction from grenade to character, used for blast force
            Vector3 blastDirection = (health.transform.position - transform.position).normalized;

            if (isClose)
            {
                health.InstantKill(blastDirection, hitPoint, DamageSource.Grenade);
            }
            else
            {
                health.GetHit(blastDirection, hitPoint, DamageSource.Grenade);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _farBlastRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _closeBlastRadius);
    }

}
