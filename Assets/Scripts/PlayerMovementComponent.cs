using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovementComponent : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    private Rigidbody _rigidBody;
    private Vector2 _inputMoveDirection;


    private WeaponComponent _currentWeapon;
    private CameraComponent _cameraComp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>(); 
        _currentWeapon = GetComponentInChildren<WeaponComponent>();
        _cameraComp = GetComponentInChildren<CameraComponent>();
    }
    void FixedUpdate()
    {
        Vector3 rotatedMoveDirection = (transform.forward * _inputMoveDirection.y + transform.right * _inputMoveDirection.x).normalized;
        _rigidBody.linearVelocity = rotatedMoveDirection * _speed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _inputMoveDirection = context.ReadValue<Vector2>();
    }

    public void Zoom(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _currentWeapon.ZoomIn();
            _cameraComp.ZoomInCamera(_currentWeapon.GetZoomFOV());
        }

        else if(context.canceled)
        {
            _currentWeapon.ZoomOut();
            _cameraComp.ZoomOutCamera();
        }
    }

}
