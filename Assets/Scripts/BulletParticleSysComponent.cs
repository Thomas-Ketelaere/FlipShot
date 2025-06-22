using UnityEngine;

public class BulletParticleSysComponent : MonoBehaviour
{
    [SerializeField] private float LifeTime = 20f;
    [SerializeField] private ParticleSystem _bulletImpact;

    public void SetActive()
    {
        Invoke("SetInactive", LifeTime);
        _bulletImpact.Play();
    }
    public void SetInactive()
    {
        CancelInvoke("SetInactive"); //when pool full, this one gets set inactive, so need to cancel for lifetime since it is shorter
        gameObject.SetActive(false);
    }
}
