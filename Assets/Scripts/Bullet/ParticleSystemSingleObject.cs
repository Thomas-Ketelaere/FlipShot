using UnityEngine;

public class ParticleSystemSingleObject : SingleObject
{
    [SerializeField] private ParticleSystem _particlySystem;

    public override void SetActive()
    {
        base.SetActive();
        _particlySystem.Play();
    }
}
