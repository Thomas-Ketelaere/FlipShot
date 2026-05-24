using UnityEngine;

public class BloodVFXComponent : MonoBehaviour
{
    [SerializeField] private float LifeTime = 20f;
    private ParticleSystem _bloodEffect;

    private void Start()
    {
        _bloodEffect = GetComponent<ParticleSystem>();
    }

    public void SetActive()
    {
        Invoke("SetInactive", LifeTime);
        _bloodEffect.Play();
    }
    public void SetInactive()
    {
        CancelInvoke("SetInactive"); //when pool full, this one gets set inactive, so need to cancel for lifetime since it would be shorter
        gameObject.SetActive(false);
    }
}
