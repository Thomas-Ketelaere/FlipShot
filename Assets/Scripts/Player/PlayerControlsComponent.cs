using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlsComponent : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _throwableThrowStrength = 5f;
    private Rigidbody _rigidBody;
    private Vector2 _inputMoveDirection;

    private WeaponComponent _currentWeapon;
    private CameraComponent _cameraComp;
    private Transform _cameraTransform;
    private Reverter _reverter;

    //ALL TEMP STUFF HERE
    [SerializeField] private GameObject _grenadePrefab; //will change when inventory system

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
        if (_reverter.InTemporalMode && context.started)
        {
            _reverter.Revert();
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

    public void ThrowThrowable(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            GameObject grenade = Instantiate(_grenadePrefab); //todo should get from inventory or elsewhere, should not be created in playerControls
            //grenade.transform.position = transform.position + new Vector3(0f, 0.6f, 0.6f);
            grenade.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            Vector3 direction = Camera.main.transform.forward;
            rb.AddForce(_throwableThrowStrength * direction, ForceMode.Impulse);
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
