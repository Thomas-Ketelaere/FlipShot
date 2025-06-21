using System.Collections;
using TMPro;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField] Vector3 _zoomWeaponPos;
    [SerializeField] Transform _barrelPos;
    [SerializeField] TMP_Text _amountBulletsText;
    [SerializeField] GameObject _magazine;
    [SerializeField] Vector3 _magazineCheckPos;
    [SerializeField] Vector3 _magazineCheckRot;
    [SerializeField] private float _zoomFOV = 50f;
    [SerializeField] private int _damageWeapon = 30;
    [SerializeField] private float _speedBullet = 1.0f;
    [SerializeField] private float _fireRate = 0.2f;
    [SerializeField] private float _reloadTime = 1f;
    [SerializeField] private float _checkAmountBulletsTime = 2f;
    [SerializeField] private int _maxBullets = 30;
    Vector3 _normalWeaponPos;
    Vector3 _normalWeaponRot;
    Vector3 _normalMagPos;
    private const float TimeToZoom = 0.04f;
    private float _accumulatedTime;
    private bool _shouldShoot;
    private int _currentAmountBullets;


    private void Awake()
    {
        _normalWeaponPos = transform.localPosition;
        _normalMagPos = _magazine.transform.localPosition;
        _normalWeaponRot = transform.eulerAngles;
        _currentAmountBullets = _maxBullets;
        _amountBulletsText.enabled = false;
        SetAmountBulletsText();
    }

    public void ZoomIn()
    {
        StartCoroutine(MoveToPos(transform, _zoomWeaponPos, TimeToZoom));
    }

    public void ZoomOut()
    {
        StartCoroutine(MoveToPos(transform, _normalWeaponPos, TimeToZoom));
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
        GameObject bullet = BulletsManager.Instance.RequestBullet();
        if (bullet != null)
        {
            bullet.transform.position = _barrelPos.position;
            bullet.transform.forward = _barrelPos.forward;
            bullet.GetComponent<BulletComponent>().SetBulletActive(_damageWeapon, _speedBullet);
        }
        --_currentAmountBullets;
        SetAmountBulletsText();
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
        if (_currentAmountBullets < _maxBullets)
        {
            if(!IsInvoking("Reload"))
            {
                if(_currentAmountBullets > 0) //in real weapons there can still be a round in the barrel
                {
                    _currentAmountBullets = 1;
                }
                else
                {
                    _currentAmountBullets = 0;
                }
                Invoke("Reload", _reloadTime);
                StartCoroutine(MoveToPos(_magazine.transform, _magazine.transform.localPosition + new Vector3(0, -0.5f, 0), _reloadTime / 2));
                Invoke("MoveMagUpReload", _reloadTime / 2);
            }
        }
    }

    public void StartCheckingAmountBullets()
    {
        if(!IsInvoking("StopCheckingAmountBullets"))
        {
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

    private void MoveMagUpReload()
    {
        StartCoroutine(MoveToPos(_magazine.transform, _normalMagPos, _reloadTime / 2));
    }

    private void Reload()
    {
        _currentAmountBullets += _maxBullets; //in real weapons there can still be a round in the barrel
        Debug.Log("Reloaded gun");
        SetAmountBulletsText();
    }

    private void SetAmountBulletsText()
    {
        string newText = $"{_currentAmountBullets}/{_maxBullets}";
        _amountBulletsText.SetText(newText);
    }
}
