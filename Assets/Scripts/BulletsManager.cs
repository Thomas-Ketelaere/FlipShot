using UnityEngine;

public class BulletsManager : MonoBehaviour
{
    public static BulletsManager Instance { get; private set; }

    [SerializeField] private int _maxBullets;
    [SerializeField] private int _maxBulletHoles;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _bulletHolePrefab;
    private GameObject[] _bullets;
    private float[] _bulletsActiveTime;
    private GameObject[] _bulletHolesObject;
    private float[] _bulletHolesActiveTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        //Bullets
        _bullets = new GameObject[_maxBullets]; //memory pool
        _bulletsActiveTime = new float[_maxBullets];
        for (int i = 0; i < _maxBullets; i++)
        {
            _bullets[i] = Instantiate(_bulletPrefab);
            _bullets[i].SetActive(false);
        }

        //BulletHoles
        _bulletHolesObject = new GameObject[_maxBulletHoles];
        _bulletHolesActiveTime = new float[_maxBulletHoles];
        for (int i = 0; i < _maxBulletHoles; i++)
        {
            _bulletHolesObject[i] = Instantiate(_bulletHolePrefab);
            _bulletHolesObject[i].SetActive(false);
        }
    }

    public GameObject RequestBullet()
    {
        int oldestIndex = -1;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < _bullets.Length; i++)
        {
            if (!_bullets[i].activeSelf)
            {
                _bullets[i].gameObject.SetActive(true);
                _bulletsActiveTime[i] = Time.time;
                return _bullets[i];
            }

            else //object is active (for when memory pool is all active)
            {
                if (_bulletsActiveTime[i] < oldestTime)
                {
                    oldestIndex = i;
                    oldestTime = _bulletsActiveTime[i];
                }
            }
        }

        Debug.Log("No Bullets left in Memory Pool");
        Debug.Log("Getting Oldest One");

        _bullets[oldestIndex].GetComponent<BulletComponent>().SetBulletInactive();
        _bullets[oldestIndex].gameObject.SetActive(true);
        _bulletsActiveTime[oldestIndex] = Time.time;
        return _bullets[oldestIndex]; 
    }

    public GameObject RequestBulletHoleObject()
    {
        int oldestIndex = -1;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < _bulletHolesObject.Length; i++)
        {
            if (!_bulletHolesObject[i].activeSelf)
            {
                _bulletHolesObject[i].gameObject.SetActive(true);
                _bulletHolesObject[i].GetComponent<BulletHoleComponent>().SetActive();
                _bulletHolesActiveTime[i] = Time.time;
                return _bulletHolesObject[i];
            }
            else //object is active (for when memory pool is all active)
            {
                if (_bulletHolesActiveTime[i] < oldestTime)
                {
                    oldestIndex = i;
                    oldestTime = _bulletHolesActiveTime[i];
                }
            }
        }

        Debug.Log("No BulletHoles left in Memory Pool");
        Debug.Log("Getting Oldest One");

        BulletHoleComponent bulletHoleComponent = _bulletHolesObject[oldestIndex].GetComponent<BulletHoleComponent>();
        bulletHoleComponent.SetInactive();
        _bulletHolesObject[oldestIndex].gameObject.SetActive(true);
        bulletHoleComponent.SetActive();
        _bulletHolesActiveTime[oldestIndex] = Time.time;
        return _bulletHolesObject[oldestIndex];
    }
}
