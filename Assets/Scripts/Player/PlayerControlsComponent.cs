using UnityEngine;
using UnityEngine.InputSystem;

//TODO not only movement so or split up or rename
public class PlayerControlsComponent : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    private Rigidbody _rigidBody;
    private Vector2 _inputMoveDirection;

    private WeaponComponent _currentWeapon;
    private CameraComponent _cameraComp;
    private Transform _cameraTransform;
    private Reverter _reverter;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>(); 
        _currentWeapon = GetComponentInChildren<WeaponComponent>();
        _cameraComp = GetComponentInChildren<CameraComponent>();
        _reverter = GetComponent<Reverter>();
        _cameraTransform = Camera.main.transform;
    }

    void FixedUpdate()
    {
        Vector3 rotatedMoveDirection = (_cameraTransform.forward * _inputMoveDirection.y + _cameraTransform.right * _inputMoveDirection.x).normalized;
        if(IsGrounded())
        {
            rotatedMoveDirection.y = 0f;
        }
        Vector3 newVelocity = rotatedMoveDirection * _speed; 
        _rigidBody.linearVelocity = newVelocity;
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


    public void Shoot(InputAction.CallbackContext context)
    {
        if (_reverter.InTemporalMode)
        {
            
            if (context.started)
            {
                OnReverterShoot(true);
            }

            else if (context.canceled)
            {
                OnReverterShoot(false);
            }
        }
        else
        {
            if (context.started)
            {
                OnWeaponShoot(true);
            }

            else if (context.canceled)
            {
                OnWeaponShoot(false);
            }
        }

    }

    private void OnWeaponShoot(bool started)
    {
        if (started)
        {
            _currentWeapon.StartShooting();
        }
        else
        {
            _currentWeapon.StopShooting();
        }

    }

    private void OnReverterShoot(bool started)
    {
        _reverter.Revert(started);
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if (_reverter.InTemporalMode)
        {
            return;
        }
        if (context.started)
        {
            _currentWeapon.StartReloading();
        }
    }

    public void CheckAmountBullets(InputAction.CallbackContext context)
    {
        if (_reverter.InTemporalMode)
        {
            return;
        }
        if (context.started)
        {
            _currentWeapon.StartCheckingAmountBullets();
        }
    }

    private bool IsGrounded()
    {
        bool hit = Physics.Raycast(transform.position, Physics.gravity.normalized, 1.1f);
        Debug.DrawLine(transform.position, transform.position + Physics.gravity.normalized * 1.1f, Color.green);
        return hit;
    }

    public WeaponComponent GetPlayerWeapon()
    {
        return _currentWeapon;
    }
}
