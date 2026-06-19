using UnityEngine;

public abstract class HealthComponent : MonoBehaviour
{
    [SerializeField] private uint _maxHealth = 2;
    [SerializeField] private uint _startHealth = 2;

    private uint _currentHealth;
    private bool _isDead = false;

    private const float DISTANCE_WALL_BLOOD = 3f;

    protected virtual void Start()
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

    public void InstantKill(Vector3 direction, Vector3 hitPoint, Vector3 hitPointNormal)
    {
        PlayBloodVFX(direction, hitPoint, hitPointNormal);
        if (_isDead) return;
        Die(direction, hitPoint, hitPointNormal);
    }

    public virtual void GetHit(Vector3 direction, Vector3 hitPoint, Vector3 hitPointNormal)
    {
        PlayBloodVFX(direction, hitPoint, hitPointNormal);
        if (_isDead) return;
        --_currentHealth;
        if (_currentHealth == 0)
        {
            Die(direction, hitPoint, hitPointNormal);
        }
    }

    protected virtual void Die(Vector3 direction, Vector3 hitPoint, Vector3 hitPointNormal)
    {
        _isDead = true;
    }

    private void PlayBloodVFX(Vector3 direction, Vector3 hitPoint, Vector3 hitPointNormal)
    {
        GameObject bloodVFXObject = ObjectPool.Instance.RequestBloodVFX();
        bloodVFXObject.transform.position = hitPoint;
        bloodVFXObject.transform.rotation = Quaternion.LookRotation(-hitPointNormal);
        if(_isDead) return; //not doing wall blood when dead
        RaycastHit wallHit;
        if (Physics.Raycast(hitPoint, direction, out wallHit, DISTANCE_WALL_BLOOD, LayerMask.GetMask("Hittable")))
        {
            Debug.Log("Hit wall");
            GameObject bloodObject = ObjectPool.Instance.RequestBloodWallObject();
            if (bloodObject != null)
            {
                bloodObject.transform.position = wallHit.point + wallHit.normal * 0.01f;
                Quaternion lookRot = Quaternion.LookRotation(-wallHit.normal);
                lookRot *= Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
                bloodObject.transform.rotation = lookRot;
            }
        }
    }
}
