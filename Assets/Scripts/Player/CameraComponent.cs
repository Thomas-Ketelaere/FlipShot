using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraComponent : MonoBehaviour
{
    [SerializeField] private float _sensitivity = 2f;
    private float _cameraRotationY;
    private float _cameraRotationX;
    private const float TimeToZoom = 0.04f;
    private float _zoomOutFOV;
    private Camera _camera;

    [Header("Head Bob")]
    [SerializeField] private float _bobFrequency = 2.5f;
    [SerializeField] private float _bobAmplitudeY = 0.05f;
    [SerializeField] private float _bobAmplitudeX = 0.025f;
    [SerializeField] private float _bobSmoothing = 10f;

    [Header("Strafe Tilt")]
    [SerializeField] private float _maxTiltAngle = 2.5f;
    [SerializeField] private float _tiltSpeed = 8f;

    private Vector3 _baseCameraLocalPos;
    private Vector3 _bobOffset;
    private float _bobTimer;
    private float _currentTilt;

    private Rigidbody _playerRigidbody;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _camera = GetComponent<Camera>();
        _zoomOutFOV = _camera.fieldOfView;
        _baseCameraLocalPos = transform.localPosition;
        _playerRigidbody = GetComponentInParent<Rigidbody>();
    }

    void Update()
    {
        HandleMouseLook();
        HandleStrafeTilt();
        HandleHeadBob();

        //apply all at once
        transform.localPosition = _baseCameraLocalPos + _bobOffset;
        transform.localEulerAngles = new Vector3(_cameraRotationY, 0f, _currentTilt);
    }

    private void HandleMouseLook()
    {
        float inputX = Input.GetAxis("Mouse X") * _sensitivity;
        float inputY = Input.GetAxis("Mouse Y") * _sensitivity;

        _cameraRotationX += inputX;
        _cameraRotationY -= inputY;
        _cameraRotationY = Mathf.Clamp(_cameraRotationY, -90f, 90f);

        transform.parent.Rotate(Vector3.up * inputX);
    }

    private void HandleStrafeTilt()
    {
        Vector3 localVelocity = transform.parent.InverseTransformDirection(_playerRigidbody.linearVelocity);
        float targetTilt = -localVelocity.x * _maxTiltAngle * 0.1f;
        targetTilt = Mathf.Clamp(targetTilt, -_maxTiltAngle, _maxTiltAngle);
        _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, Time.deltaTime * _tiltSpeed);
    }

    private void HandleHeadBob()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        float speed = flatVelocity.magnitude;
        bool isMoving = speed > 0.1f;

        if (isMoving)
        {
            _bobTimer += Time.deltaTime * _bobFrequency;
            float targetY = Mathf.Sin(_bobTimer) * _bobAmplitudeY;
            float targetX = Mathf.Sin(_bobTimer * 0.5f) * _bobAmplitudeX;
            _bobOffset = Vector3.Lerp(_bobOffset, new Vector3(targetX, targetY, 0f), Time.deltaTime * _bobSmoothing);
        }
        else
        {
            _bobTimer = 0f;
            _bobOffset = Vector3.Lerp(_bobOffset, Vector3.zero, Time.deltaTime * _bobSmoothing);
        }
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
        float currentZoomTime = 0f;

        while (currentZoomTime < TimeToZoom)
        {
            currentZoomTime += Time.deltaTime;
            _camera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, currentZoomTime / TimeToZoom);
            yield return null;
        }

        _camera.fieldOfView = targetFOV;
    }
}
