using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [Header("Bullets")]
    [SerializeField] private int _maxBullets;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private GameObject _bulletShellPrefab;
    [SerializeField] private int _maxBulletHoles;
    [SerializeField] private GameObject _bulletHolePrefab;

    [Header("Blood")]
    [SerializeField] private List<GameObject> _bloodWallPrefabs = new List<GameObject>();
    [SerializeField] private int _maxBloodWalls;
    [SerializeField] private GameObject _bloodVFXPrefab;
    [SerializeField] private int _maxBloodVFX;


    private SingleObject[] _bullets;
    private float[] _bulletsActiveTime;
    private SingleObject[] _bulletHolesObject;
    private float[] _bulletHolesActiveTime;
    private SingleObject[] _bulletShellsObject;
    private float[] _bulletShellsActiveTime;
    private SingleObject[] _bloodWalls;
    private float[] _bloodWallsActiveTime;
    private SingleObject[] _bloodVFXs;
    private float[] _bloodVFXsActiveTime;

    private const string BULLETS_OBJECT_NAME = "Bullets";
    private const string BULLET_SHELLS_OBJECT_NAME = "Bullet Shells";
    private const string BULLET_HOLES_OBJECT_NAME = "Bullet Holes";
    private const string BLOOD_WALL_OBJECT_NAME = "Wall Blood Objects";
    private const string BLOOD_VFX_OBJECT_NAME = "Blood VFXs";

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
        //GameObject bulletsParent = new GameObject(BULLETS_OBJECT_NAME);
        //bulletsParent.transform.parent = this.transform;
        //_bullets = new SingleObject[_maxBullets];
        //_bulletsActiveTime = new float[_maxBullets];
        //for (int i = 0; i < _maxBullets; i++)
        //{
        //    _bullets[i] = Instantiate(_bulletPrefab, bulletsParent.transform).GetComponent<SingleObject>();
        //    _bullets[i].gameObject.SetActive(false);
        //}

        //BulletHoles
        GameObject bulletHolesParent = new GameObject(BULLET_HOLES_OBJECT_NAME);
        bulletHolesParent.transform.parent = this.transform;
        _bulletHolesObject = new SingleObject[_maxBulletHoles];
        _bulletHolesActiveTime = new float[_maxBulletHoles];
        for (int i = 0; i < _maxBulletHoles; i++)
        {
            _bulletHolesObject[i] = Instantiate(_bulletHolePrefab, bulletHolesParent.transform).GetComponent<SingleObject>();
            _bulletHolesObject[i].gameObject.SetActive(false);
        }

        //bulletShells
        GameObject bulletShellsParent = new GameObject(BULLET_SHELLS_OBJECT_NAME);
        bulletShellsParent.transform.parent = this.transform;
        _bulletShellsObject = new SingleObject[_maxBullets];
        _bulletShellsActiveTime = new float[_maxBullets];
        for (int i = 0; i < _maxBullets; i++)
        {
            _bulletShellsObject[i] = Instantiate(_bulletShellPrefab, bulletShellsParent.transform).GetComponent<SingleObject>();
            _bulletShellsObject[i].gameObject.SetActive(false);
        }

        // blood on walls
        GameObject bloodWallsParent = new GameObject(BLOOD_WALL_OBJECT_NAME);
        bloodWallsParent.transform.parent = this.transform;
        _bloodWalls = new SingleObject[_maxBloodWalls];
        _bloodWallsActiveTime = new float[_maxBloodWalls];
        for (int i = 0; i < _maxBloodWalls; i++)
        {
            GameObject randomPrefab = _bloodWallPrefabs[Random.Range(0, _bloodWallPrefabs.Count)];
            _bloodWalls[i] = Instantiate(randomPrefab, bloodWallsParent.transform).GetComponent<SingleObject>();
            _bloodWalls[i].gameObject.SetActive(false);
        }

        // blood VFX
        GameObject bloodVFXParent = new GameObject(BLOOD_VFX_OBJECT_NAME);
        bloodVFXParent.transform.parent = this.transform;
        _bloodVFXs = new SingleObject[_maxBloodVFX];
        _bloodVFXsActiveTime = new float[_maxBloodVFX];
        for (int i = 0; i < _maxBloodVFX; i++)
        {
            _bloodVFXs[i] = Instantiate(_bloodVFXPrefab, bloodVFXParent.transform).GetComponent<SingleObject>();
            _bloodVFXs[i].gameObject.SetActive(false);
        }
    }

    public GameObject RequestBullet()
    {
        int oldestIndex = -1;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < _bullets.Length; i++)
        {
            if (!_bullets[i].gameObject.activeSelf)
            {
                _bullets[i].gameObject.SetActive(true);
                _bullets[i].SetActive();
                _bulletsActiveTime[i] = Time.time;
                return _bullets[i].gameObject;
            }
            else
            {
                if (_bulletsActiveTime[i] < oldestTime)
                {
                    oldestIndex = i;
                    oldestTime = _bulletsActiveTime[i];
                }
            }
        }

        Debug.Log("No Bullets left in Memory Pool. Getting Oldest One.");

        _bullets[oldestIndex].SetInactive();
        _bullets[oldestIndex].gameObject.SetActive(true);
        _bullets[oldestIndex].SetActive();
        _bulletsActiveTime[oldestIndex] = Time.time;
        return _bullets[oldestIndex].gameObject;
    }

    public GameObject RequestBulletHoleObject()
    {
        int oldestIndex = -1;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < _bulletHolesObject.Length; i++)
        {
            if (!_bulletHolesObject[i].gameObject.activeSelf)
            {
                _bulletHolesObject[i].gameObject.SetActive(true);
                _bulletHolesObject[i].SetActive();
                _bulletHolesActiveTime[i] = Time.time;
                return _bulletHolesObject[i].gameObject;
            }
            else
            {
                if (_bulletHolesActiveTime[i] < oldestTime)
                {
                    oldestIndex = i;
                    oldestTime = _bulletHolesActiveTime[i];
                }
            }
        }

        Debug.Log("No BulletHoles left in Memory Pool. Getting Oldest One.");

        _bulletHolesObject[oldestIndex].SetInactive();
        _bulletHolesObject[oldestIndex].gameObject.SetActive(true);
        _bulletHolesObject[oldestIndex].SetActive();
        _bulletHolesActiveTime[oldestIndex] = Time.time;
        return _bulletHolesObject[oldestIndex].gameObject;
    }

    public GameObject RequestBulletShellObject()
    {
        int oldestIndex = -1;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < _bulletShellsObject.Length; i++)
        {
            if (!_bulletShellsObject[i].gameObject.activeSelf)
            {
                _bulletShellsObject[i].gameObject.SetActive(true);
                _bulletShellsObject[i].SetActive();
                _bulletShellsActiveTime[i] = Time.time;
                return _bulletShellsObject[i].gameObject;
            }
            else
            {
                if (_bulletShellsActiveTime[i] < oldestTime)
                {
                    oldestIndex = i;
                    oldestTime = _bulletShellsActiveTime[i];
                }
            }
        }

        Debug.Log("No BulletShells left in Memory Pool. Getting Oldest One.");

        _bulletShellsObject[oldestIndex].SetInactive();
        _bulletShellsObject[oldestIndex].gameObject.SetActive(true);
        _bulletShellsObject[oldestIndex].SetActive();
        _bulletShellsActiveTime[oldestIndex] = Time.time;
        return _bulletShellsObject[oldestIndex].gameObject;
    }

    public GameObject RequestBloodWallObject()
    {
        int oldestIndex = -1;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < _bloodWalls.Length; i++)
        {
            if (!_bloodWalls[i].gameObject.activeSelf)
            {
                _bloodWalls[i].gameObject.SetActive(true);
                _bloodWalls[i].SetActive();
                _bloodWallsActiveTime[i] = Time.time;
                return _bloodWalls[i].gameObject;
            }
            else
            {
                if (_bloodWallsActiveTime[i] < oldestTime)
                {
                    oldestIndex = i;
                    oldestTime = _bloodWallsActiveTime[i];
                }
            }
        }

        Debug.Log("No Blood Walls left in Memory Pool. Getting Oldest One.");

        _bloodWalls[oldestIndex].SetInactive();
        _bloodWalls[oldestIndex].gameObject.SetActive(true);
        _bloodWalls[oldestIndex].SetActive();
        _bloodWallsActiveTime[oldestIndex] = Time.time;
        return _bloodWalls[oldestIndex].gameObject;
    }

    public GameObject RequestBloodVFX()
    {
        int oldestIndex = -1;
        float oldestTime = float.MaxValue;

        for (int i = 0; i < _bloodVFXs.Length; i++)
        {
            if (!_bloodVFXs[i].gameObject.activeSelf)
            {
                _bloodVFXs[i].gameObject.SetActive(true);
                _bloodVFXs[i].SetActive();
                _bloodVFXsActiveTime[i] = Time.time;
                return _bloodVFXs[i].gameObject;
            }
            else
            {
                if (_bloodVFXsActiveTime[i] < oldestTime)
                {
                    oldestIndex = i;
                    oldestTime = _bloodVFXsActiveTime[i];
                }
            }
        }

        Debug.Log("No Blood VFXs left in Memory Pool. Getting Oldest One.");

        _bloodVFXs[oldestIndex].SetInactive();
        _bloodVFXs[oldestIndex].gameObject.SetActive(true);
        _bloodVFXs[oldestIndex].SetActive();
        _bloodVFXsActiveTime[oldestIndex] = Time.time;
        return _bloodVFXs[oldestIndex].gameObject;
    }
}