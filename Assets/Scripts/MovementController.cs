using UnityEngine;

public class MovementController : MonoBehaviour
{
    //For testing
    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _velocity;

    [Header("Animation smoothing")]
    [SerializeField] private float _dampTime = 0.1f;
    [SerializeField] private float _rotationSpeed = 3f;

    private Animator _animator;

    private static readonly int VelocityZHash = Animator.StringToHash("VelocityZ");

    private Vector3 _worldMoveDirection;
    private float _targetSpeed;
    private bool _isMoving;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _animator.applyRootMotion = true;
    }

    public void SetMovement(Vector3 worldVelocity, float speed)
    {
        if (worldVelocity.sqrMagnitude > 0.01f)
        {
            _worldMoveDirection = worldVelocity.normalized;
            _targetSpeed = speed;
            _isMoving = true;
        }
        else
        {
            StopMovement();
        }
    }

    public void StopMovement()
    {
        _isMoving = false;
        _targetSpeed = 0f;
        _worldMoveDirection = Vector3.zero;
    }

    void Update()
    {
        if (_isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_worldMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime); //navmesh is world space
        }

        Vector3 localMove = transform.InverseTransformDirection(_worldMoveDirection) * _targetSpeed; //blend tree is local space

        _animator.SetFloat(VelocityZHash, localMove.z, _dampTime, Time.deltaTime);
    }


    private void OnAnimatorMove()
    {
        transform.position += _animator.deltaPosition;
    }

}