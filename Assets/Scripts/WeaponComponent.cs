using System.Collections;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [SerializeField] Vector3 _zoomWeaponPos;
    [SerializeField] public float _zoomFOV = 50f;
    Vector3 _normalWeaponPos;
    private const float TimeToZoom = 0.04f;

    private void Awake()
    {
        _normalWeaponPos = transform.localPosition;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ZoomIn()
    {
        //transform.localPosition = _zoomWeaponPos;

        StartCoroutine(MoveToPos(_zoomWeaponPos));
    }

    public void ZoomOut()
    {
        //transform.localPosition = _normalWeaponPos;
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

}
