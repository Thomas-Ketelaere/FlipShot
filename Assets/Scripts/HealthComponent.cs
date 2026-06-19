using UnityEngine;

public abstract class HealthComponent : MonoBehaviour
{
    [SerializeField] private uint _maxHealth = 1;
    [SerializeField] private uint _startHealth = 1; 

    private uint _currentHealth;

    void Start()
    {
        if(_maxHealth == 0)
        {
            Debug.LogError("[Health Component] Max health is 0 on start, change value");
        }
        if(_startHealth > _maxHealth)
        {
            Debug.LogWarning("[Health Component] Start health is bigger than max health, setting start equal to max health");
            _startHealth = _maxHealth;
        }
        if(_startHealth == 0)
        {
            Debug.LogWarning("[Health Component] Start health is 0 on start, setting it to max health");
            _startHealth = _maxHealth;
        }

        _currentHealth = _startHealth;
    }

    public void InstantKill(Vector3 direction, Vector3 hitPoint)
    {
        Die(direction, hitPoint);
    }

    public void GetHit(Vector3 direction, Vector3 hitPoint)
    {
        --_currentHealth;
        if (_currentHealth == 0)
        {
            Die(direction, hitPoint);
        }
    }

    protected abstract void Die(Vector3 direction, Vector3 hitPoint); //todo send hit direction for ragdoll 
}
