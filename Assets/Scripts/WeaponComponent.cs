using System.Collections;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField] Vector3 _zoomWeaponPos;
    [SerializeField] Transform _barrelPos;
    [SerializeField] private float _zoomFOV = 50f;
    [SerializeField] private int _damageWeapon = 30;
    [SerializeField] private float _speedBullet = 1.0f;
    [SerializeField] private float _fireRate = 0.2f;
    Vector3 _normalWeaponPos;
    private const float TimeToZoom = 0.04f;

    private void Awake()
    {
        _normalWeaponPos = transform.localPosition;
    }

    public void ZoomIn()
    {
        StartCoroutine(MoveToPos(_zoomWeaponPos));
    }

    public void ZoomOut()
    {
        StartCoroutine(MoveToPos(_normalWeaponPos));
    }

    private IEnumerator MoveToPos(Vector3 target)
    {
        Vector3 start = transform.localPosition;
        float t = 0f;

        while (t < TimeToZoom)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, target, t / TimeToZoom);
            yield return null;
        }

        transform.localPosition = target;
    }

    public float GetZoomFOV()
    {
        return _zoomFOV;
    }
    public float GetFireRate()
    {
        return _fireRate;
    }

    public void Shoot()
    {
        GameObject bullet = BulletsManager.Instance.RequestBullet();
        if (bullet != null)
        {
            bullet.transform.position = _barrelPos.position;
            bullet.transform.forward = _barrelPos.forward;
            bullet.GetComponent<BulletComponent>().SetBulletActive(_damageWeapon, _speedBullet);
        }
    }

}
