using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private float _stoppingDistance = 1.5f;
    [SerializeField] private float _runThreshold = 3f; // if remaining distance > this, run

    [Header("Speeds")]
    [SerializeField] private float _walkSpeed = 1f;
    [SerializeField] private float _runSpeed = 2f;

    private NavMeshAgent _agent;
    private MovementController _movementController;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false; // root motion handles position
        _agent.updateRotation = false; // MovementController handles rotation
        _agent.stoppingDistance = _stoppingDistance;

        _movementController = GetComponent<MovementController>();

        _agent.SetDestination(Vector3.zero);
    }

    void Update()
    {
        if (_agent.hasPath && _agent.remainingDistance > _agent.stoppingDistance)
        {
            Vector3 worldVelocity = _agent.desiredVelocity;
            float targetSpeed = _agent.remainingDistance > _runThreshold ? _runSpeed : _walkSpeed;

            _movementController.SetMovement(worldVelocity, targetSpeed);
        }
        else
        {
            _movementController.StopMovement();
        }

        _agent.nextPosition = transform.position;
    }

}
