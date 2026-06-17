using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField] Vector3 _zoomWeaponPos;
    [SerializeField] Transform _barrelPos;
    [SerializeField] Transform _ejectionPoint;
    [SerializeField] TMP_Text _amountBulletsText;
    [SerializeField] GameObject _magazine;
    [SerializeField] ParticleSystem _muzzleFlash;
    [SerializeField] Vector3 _magazineCheckPos;
    [SerializeField] Vector3 _magazineCheckRot;
    [SerializeField] private float _zoomFOV = 50f;
    [SerializeField] private int _damageWeapon = 30;
    [SerializeField] private float _speedBullet = 1.0f;
    [SerializeField] private float _fireRate = 0.2f;
    [SerializeField] private float _checkAmountBulletsTime = 2f;
    [SerializeField] private int _maxBullets = 30;
    [SerializeField] private float _bulletExtraDirUp = 0.1f;
    [SerializeField] private GameObject _bloodVFXObj;

    Vector3 _normalWeaponPos;
    Vector3 _normalWeaponRot;
    Vector3 _normalMagPos;
    Vector3 _normalBoltPos;
    private const float TIME_TO_ZOOM = 0.04f;
    private const float DISTANCE_WALL_BLOOD = 3f;
    private float _accumulatedTime;
    private bool _isReloading;
    private bool _shouldShoot;
    private int _currentAmountBullets;

    private Animator _gunAnimator;

    //recoil
    private RecoilComponent _recoilComponent;

    private void Awake()
    {
        _normalWeaponPos = transform.localPosition;
        _normalMagPos = _magazine.transform.localPosition;
        _normalWeaponRot = transform.eulerAngles;
        _currentAmountBullets = _maxBullets;
        _amountBulletsText.enabled = false;
        SetAmountBulletsText();
    }

    private void Start()
    {
        _recoilComponent = GetComponent<RecoilComponent>();
        _gunAnimator = GetComponent<Animator>();
    }

    public void ZoomIn()
    {
        StartCoroutine(MoveToPos(transform, _zoomWeaponPos, TIME_TO_ZOOM));
    }

    public void ZoomOut()
    {
        StartCoroutine(MoveToPos(transform, _normalWeaponPos, TIME_TO_ZOOM));
    }

    private IEnumerator MoveToPos(Transform objectToMove, Vector3 targetPos, float inTime)
    {
        Vector3 start = objectToMove.localPosition;
        float t = 0f;

        while (t < inTime)
        {
            t += Time.deltaTime;
            objectToMove.localPosition = Vector3.Lerp(start, targetPos, t / inTime);
            yield return null;
        }

        objectToMove.localPosition = targetPos;
    }

    private IEnumerator RotateTo(Transform objectToRotate, Quaternion targetRot, float inTime)
    {
        Quaternion start = objectToRotate.localRotation;
        float t = 0f;

        while (t < inTime)
        {
            t += Time.deltaTime;
            objectToRotate.localRotation = Quaternion.Slerp(start, targetRot, t / inTime);
            yield return null;
        }

        objectToRotate.localRotation = targetRot;
    }

    public float GetZoomFOV()
    {
        return _zoomFOV;
    }

    private void Update()
    {
        if(_accumulatedTime <= _fireRate) //not necessary to keep increasing if it is already bigger than it needs to be
        {
            _accumulatedTime += Time.deltaTime;
        }
        else
        {
            if (_shouldShoot && _currentAmountBullets > 0)
            {
                Shoot();
                _accumulatedTime -= _fireRate;
            }
        }
    }

    private void Shoot()
    {
        --_currentAmountBullets;
        if (_currentAmountBullets == 0)
        {
            _gunAnimator.Play("FireWeaponEmpty", 0, 0f);
        }
        else
        {
            _gunAnimator.Play("FireWeapon", 0, 0f);
        }


        //_muzzleFlash.Play();

        //JUST "EASY" RAYCAST
        //SpawnBulletShell();

        //StartCoroutine(MoveToPos(_bolt.transform, _bolt.transform.localPosition + new Vector3(0, 0, -0.07f), _fireRate / 3));
        //StartCoroutine(MoveToPos(transform, transform.localPosition + new Vector3(0, 0, -0.05f), _fireRate / 3));
        //Invoke("StartResetBolt", _fireRate / 3);
        RaycastHit hit;
        if (Physics.Raycast(_barrelPos.position, _barrelPos.forward, out hit, LayerMask.GetMask("Hittable", "Damageable")))
        {
            if (hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Player/Enemy hit");
                //get health comp and do damage
                GameObject bloodVFXObject = Instantiate(_bloodVFXObj); //todo should be done in player/enemy self
                bloodVFXObject.transform.position = hit.point;
                bloodVFXObject.transform.rotation = Quaternion.LookRotation(-hit.normal);
                RaycastHit wallHit;
                if (Physics.Raycast(hit.point, transform.forward, out wallHit, DISTANCE_WALL_BLOOD, LayerMask.GetMask("Hittable"))) //hits wall behind enemy/player
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

            else if (hit.collider.gameObject.CompareTag("Object"))
            {
                GameObject bulletHoleObj = ObjectPool.Instance.RequestBulletHoleObject();
                if (bulletHoleObj != null)
                {
                    bulletHoleObj.transform.position = hit.point + hit.normal * 0.01f;
                    bulletHoleObj.transform.rotation = Quaternion.LookRotation(-hit.normal);
                }
            }

            else if (hit.collider.gameObject.CompareTag("Glass"))
            {
                //Instantiate(_bulletHoleGlassObj, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal));
            }
        }

        //WITH BULLET POOL
        //GameObject bullet = BulletsManager.Instance.RequestBullet();
        //if (bullet != null)
        //{
        //    bullet.transform.position = _barrelPos.position;
        //    bullet.transform.forward = _barrelPos.forward + new Vector3(0, _bulletExtraDirUp, 0); //sending bullet a tad up to get equal with sight
        //    bullet.GetComponent<BulletComponent>().SetBulletActive(_damageWeapon, _speedBullet);
        //    SpawnBulletShell();
        //    StartCoroutine(MoveToPos(_bolt.transform, _bolt.transform.localPosition + new Vector3(0, 0, -0.07f), _fireRate/3));
        //    StartCoroutine(MoveToPos(transform, transform.localPosition + new Vector3(0, 0, -0.05f), _fireRate / 3));
        //    Invoke("StartResetBolt", _fireRate / 3);
        //}
        
        SetAmountBulletsText();
        _recoilComponent.AddRecoil();
    }

    public void RevertShoot(Vector3 origin)
    {
        RaycastHit hit;
        if(Physics.Linecast(origin, _barrelPos.position, out hit, LayerMask.GetMask("Hittable", "Damageable")))
        {
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit");
                //get health comp and do damage
                GameObject bloodVFXObject = Instantiate(_bloodVFXObj); //todo should be done in player/enemy self
                bloodVFXObject.transform.position = hit.point;
                bloodVFXObject.transform.rotation = Quaternion.LookRotation(-hit.normal); //optimally should also show blood in front of character, since bullet goes through
            }
        }
        if (_currentAmountBullets == 0)
        {
            _gunAnimator.Play("FireWeaponReverseFromEmpty");
        }
        else
        {
            _gunAnimator.Play("FireWeaponReverse");
        }
            
        AddBullet();
    }

    private void SpawnBulletShell()
    {
        GameObject bulletShell = ObjectPool.Instance.RequestBulletShellObject();
        bulletShell.transform.position = _ejectionPoint.position;
        bulletShell.transform.rotation = _ejectionPoint.rotation;

        Rigidbody shellRb = bulletShell.GetComponentInChildren<Rigidbody>();

        shellRb.isKinematic = false;
        shellRb.linearVelocity = Vector3.zero;
        shellRb.angularVelocity = Vector3.zero;

        Vector3 ejectionForce = _ejectionPoint.right * Random.Range(1.0f, 2.0f) + _ejectionPoint.up * Random.Range(0.5f, 1.0f);
        Vector3 spinTorque = Random.insideUnitSphere * 5f;

        shellRb.AddForce(ejectionForce, ForceMode.Impulse);
        shellRb.AddTorque(spinTorque, ForceMode.Impulse);
    }

    public void StartShooting()
    {
        _shouldShoot = true;
    }

    public void StopShooting()
    {
        _shouldShoot = false;
    }

    public void StartReloading()
    {
        if (_isReloading)
            return;

        if (_currentAmountBullets < _maxBullets)
        {
            _isReloading = true;
            if (_currentAmountBullets > 0) //in real weapons there can still be a round in the barrel
            {
                _currentAmountBullets = 1;
                _gunAnimator.Play("ReloadWeapon");
            }
            else
            {
                _currentAmountBullets = 0;
                _gunAnimator.Play("ReloadWeaponFromEmpty");
                //_gunAnimator.SetBool("IsEmpty", true);
            }
        }
    }

    public void StartCheckingAmountBullets()
    {
        if(!IsInvoking("StopCheckingAmountBullets"))
        {
            _recoilComponent.ResetRecoil();
            Invoke("StopCheckingAmountBullets", _checkAmountBulletsTime);
            Invoke("ReturnFromCheckingAmountBullets", _checkAmountBulletsTime / 2);
            StartCoroutine(MoveToPos(transform, _magazineCheckPos, _checkAmountBulletsTime / 4));
            StartCoroutine(RotateTo(transform, Quaternion.Euler(_magazineCheckRot), _checkAmountBulletsTime / 4));
            _amountBulletsText.enabled = true;
        }
    }

    private void ReturnFromCheckingAmountBullets()
    {
        StartCoroutine(MoveToPos(transform, _normalWeaponPos, _checkAmountBulletsTime / 2));
        StartCoroutine(RotateTo(transform, Quaternion.Euler(_normalWeaponRot), _checkAmountBulletsTime / 2));
    }

    private void StopCheckingAmountBullets()
    {
        _amountBulletsText.enabled = false;
    }

    //private void MoveMagUpReload()
    //{
    //    StartCoroutine(MoveToPos(_magazine.transform, _normalMagPos, _reloadTime / 2));
    //}

    public void EndReloading() //dont remove, gets used in animation event
    {
        _currentAmountBullets += _maxBullets; //in real weapons there can still be a round in the barrel
        Debug.Log("Reloaded gun");
        _gunAnimator.SetBool("IsEmpty", false);
        _isReloading = false;
        SetAmountBulletsText();
    }

    private void SetAmountBulletsText()
    {
        string newText = $"{_currentAmountBullets}/{_maxBullets}";
        _amountBulletsText.SetText(newText);
    }

    public void AddBullet()
    {
        ++_currentAmountBullets;
        SetAmountBulletsText();
    }

    public Transform GetEjectionPoint()
    {
        return _ejectionPoint;
    }

    public void PlayMuzzleFlash()
    {
        _muzzleFlash.Play();
    }

    public Transform GetBarrelOutTransform()
    {
        return _barrelPos;
    }
}
