using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float _farBlastRadius = 4f; //only gets hit
    [SerializeField] private float _closeBlastRadius = 2f; //gets insta killed
    [SerializeField] private float _blastForce = 100f;
    [SerializeField] private ParticleSystem _explosionParticle;
    [SerializeField] private MeshRenderer _grenadeMesh;

    private const float FUSE_TIME = 5f;

    private int _layerMask;
    private const float EXPLOSION_TIME = 1f;

    private void Start()
    {
        _layerMask = LayerMask.GetMask("Damageable"); //pref here also env damage (stretch goal)
        //should grenade trigger/move other grenade
        Invoke("Explode", FUSE_TIME); 

        //todo animation pin
        //todo lever going away
    }

    public void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _farBlastRadius, _layerMask);

        Dictionary<HealthComponent, Vector3> uniqueCharacters = new(); //only save one health per object part

        foreach (Collider collider in colliders)
        {
            InjuryPart part = collider.GetComponent<InjuryPart>();
            if (part == null)
            {
                Debug.LogError("[Grenade] Hittable collider missing InjuryPart: " + collider.gameObject.name);
                continue;
            }

            HealthComponent health = part.Health;

            if (!uniqueCharacters.ContainsKey(health))
            {
                uniqueCharacters.Add(health, collider.ClosestPoint(transform.position));
            }
                
        }

        foreach (var (health, hitPoint) in uniqueCharacters)
        {
            float distance = Vector3.Distance(health.transform.position, transform.position);
            bool isClose = distance <= _closeBlastRadius;

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

        _explosionParticle.Play();
        _grenadeMesh.enabled = false;
        Invoke("DestroyGrenade", EXPLOSION_TIME);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _farBlastRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _closeBlastRadius);
    }

    private void DestroyGrenade()
    {
        Destroy(gameObject);
    }

}
