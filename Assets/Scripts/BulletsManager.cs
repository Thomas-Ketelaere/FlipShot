using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BulletsManager : MonoBehaviour
{
    public static BulletsManager Instance { get; private set; }

    [SerializeField] private int _maxBullets;
    [SerializeField] private int _maxBulletHoles;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _bulletHolePrefab;
    [SerializeField] private GameObject _bulletShellPrefab;
    [SerializeField] private int _maxBloodWalls;
    [SerializeField] private List<GameObject> _bloodWallPrefabs = new List<GameObject>();
    private GameObject[] _bullets;
    private float[] _bulletsActiveTime;
    private GameObject[] _bulletHolesObject;
    private float[] _bulletHolesActiveTime;
    private GameObject[] _bulletShellsObject;
    private float[] _bulletShellsActiveTime;
    private GameObject[] _bloodWalls;
    private float[] _bloodWallsActiveTime;

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

        //bulletShells
        _bulletShellsObject = new GameObject[_maxBullets]; 
        _bulletShellsActiveTime = new float[_maxBullets];
        for (int i = 0; i < _maxBullets; i++)
        {
            _bulletShellsObject[i] = Instantiate(_bulletShellPrefab);
            _bulletShellsObject[i].SetActive(false);
        }

        // blood on walls
        _bloodWalls = new GameObject[_maxBloodWalls];
        _bloodWallsActiveTime = new float[_maxBloodWalls];

        for (int i = 0; i < _maxBloodWalls; i++)
        {
            GameObject randomPrefab = _bloodWallPrefabs[Random.Range(0, _bloodWallPrefabs.Count)];
            _bloodWalls[i] = Instantiate(randomPrefab);
            _bloodWalls[i].SetActive(false);
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
                _bulletHolesObject[i].GetComponent<BulletParticleSysComponent>().SetActive();
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

        BulletParticleSysComponent bulletHoleComponent = _bulletHolesObject[oldestIndex].GetComponent<BulletParticleSysComponent>();
        bulletHoleComponent.SetInactive();
        _bulletHolesObject[oldestIndex].gameObject.SetActive(true);
        bulletHoleComponent.SetActive();
        _bulletHolesActiveTime[oldestIndex] = Time.time;
        return _bulletHolesObject[oldestIndex];
    }

    public GameObject RequestBulletShellObject()
    {
        int oldestIndex = -1;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < _bulletShellsObject.Length; i++)
        {
            if (!_bulletShellsObject[i].activeSelf)
            {
                _bulletShellsObject[i].gameObject.SetActive(true);
                _bulletShellsObject[i].GetComponent<TempBulletComponent>().SetActive();
                _bulletShellsActiveTime[i] = Time.time;
                return _bulletShellsObject[i];
            }
            else //object is active (for when memory pool is all active)
            {
                if (_bulletShellsActiveTime[i] < oldestTime)
                {
                    oldestIndex = i;
                    oldestTime = _bulletShellsActiveTime[i];
                }
            }
        }

        Debug.Log("No BulletShells left in Memory Pool");
        Debug.Log("Getting Oldest One");

        TempBulletComponent BulletShellComponent = _bulletShellsObject[oldestIndex].GetComponent<TempBulletComponent>();
        BulletShellComponent.SetInactive();
        _bulletShellsObject[oldestIndex].gameObject.SetActive(true);
        BulletShellComponent.SetActive();
        _bulletShellsActiveTime[oldestIndex] = Time.time;
        return _bulletShellsObject[oldestIndex];
    }

    public GameObject RequestBloodWallObject()
    {
        int oldestIndex = -1;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < _bloodWalls.Length; i++)
        {
            if (!_bloodWalls[i].activeSelf)
            {
                _bloodWalls[i].gameObject.SetActive(true);
                _bloodWalls[i].GetComponent<TempBulletComponent>().SetActive();
                _bloodWallsActiveTime[i] = Time.time;
                return _bloodWalls[i];
            }
            else //object is active (for when memory pool is all active)
            {
                if (_bloodWallsActiveTime[i] < oldestTime)
                {
                    oldestIndex = i;
                    oldestTime = _bloodWallsActiveTime[i];
                }
            }
        }

        Debug.Log("No BulletShells left in Memory Pool");
        Debug.Log("Getting Oldest One");

        TempBulletComponent BloodWallComponent = _bloodWalls[oldestIndex].GetComponent<TempBulletComponent>();
        BloodWallComponent.SetInactive();
        _bloodWalls[oldestIndex].gameObject.SetActive(true);
        BloodWallComponent.SetActive();
        _bloodWallsActiveTime[oldestIndex] = Time.time;
        return _bloodWalls[oldestIndex];
    }



}
