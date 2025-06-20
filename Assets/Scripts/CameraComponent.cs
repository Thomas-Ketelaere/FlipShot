using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraComponent : MonoBehaviour
{
    [SerializeField] private float _sensitivity = 2f;
    private float _cameraRotationY;
    private const float TimeToZoom = 0.04f;
    private float _zoomOutFOV;
    private Camera _camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       Cursor.lockState = CursorLockMode.Locked;
        _camera = GetComponent<Camera>();
        _zoomOutFOV = _camera.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Mouse X") * _sensitivity;
        float inputY = Input.GetAxis("Mouse Y") * _sensitivity;

        //vertical
        _cameraRotationY -= inputY;
        _cameraRotationY = Mathf.Clamp(_cameraRotationY, -90f, 90f);
        transform.localEulerAngles = Vector3.right * _cameraRotationY;
        transform.parent.Rotate(Vector3.up * inputX);
    }

    public void ZoomInCamera(float newZoomInFOV)
    {
        StartCoroutine(ZoomToFOV(newZoomInFOV));
    }

    public void ZoomOutCamera()
    {
        StartCoroutine(ZoomToFOV(_zoomOutFOV));
    }

    private IEnumerator ZoomToFOV(float targetFOV)
    {
        float startFOV = _camera.fieldOfView;
        float t = 0f;

        while (t < TimeToZoom)
        {
            t += Time.deltaTime;
            _camera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t / TimeToZoom);
            yield return null;
        }

        _camera.fieldOfView = targetFOV;
    }
}
