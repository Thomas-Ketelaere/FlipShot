using UnityEngine;

public class BulletParticleSysComponent : SingleObject
{
    [SerializeField] private ParticleSystem _bulletImpact;

    public override void SetActive()
    {
        base.SetActive();
        _bulletImpact.Play();
    }
}
