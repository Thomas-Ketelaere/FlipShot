using UnityEngine;

public class WeaponSway: MonoBehaviour
{
    [Header("Sway")]
    [SerializeField] private float _swayAmount = 0.02f;
    [SerializeField] private float _maxSwayAmount = 0.06f;
    [SerializeField] private float _swaySmoothness = 8f;

    [Header("Rotational Sway")]
    [SerializeField] private float _rotationSwayAmount = 4f;
    [SerializeField] private float _maxRotationSway = 5f;
    [SerializeField] private float _rotationSwaySmoothness = 8f;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private void Start()
    {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
    }
    
    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
    
        float swayX = Mathf.Clamp(-mouseX * _swayAmount, -_maxSwayAmount, _maxSwayAmount);
        float swayY = Mathf.Clamp(-mouseY * _swayAmount, -_maxSwayAmount, _maxSwayAmount);
        Vector3 targetPosition = _initialPosition + new Vector3(swayX, swayY, 0f);
    
        float tiltY = Mathf.Clamp(mouseX * _rotationSwayAmount, -_maxRotationSway, _maxRotationSway);
        float tiltX = Mathf.Clamp(mouseY * _rotationSwayAmount, -_maxRotationSway, _maxRotationSway);
        Quaternion targetRotation = _initialRotation * Quaternion.Euler(-tiltX, tiltY, tiltY);
    
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * _swaySmoothness);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * _rotationSwaySmoothness);
    }
}