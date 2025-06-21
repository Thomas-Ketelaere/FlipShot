using UnityEngine;

public class BulletsManager : MonoBehaviour
{
    public static BulletsManager Instance { get; private set; }

    [SerializeField] private int _maxBullets;
    [SerializeField] private GameObject _bulletPrefab;
    private GameObject[] _bullets;

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

        _bullets = new GameObject[_maxBullets]; //memory pool

        for (int i = 0; i < _maxBullets; i++)
        {
            _bullets[i] = Instantiate(_bulletPrefab);
            _bullets[i].SetActive(false);
        }
    }

    public GameObject RequestBullet()
    {
        for (int i = 0; i < _bullets.Length; i++)
        {
            if (!_bullets[i].activeSelf)
            {
                return _bullets[i];
            }
        }

        Debug.LogWarning("No Bullets left in Memory Pool");
        return null; //this should disable the oldest one
    }
}
