using UnityEngine;
using UnityEngine.InputSystem;


//TODO this class should not do player stuff, or rename for clarity
public class Reverter : MonoBehaviour
{
    private bool _isReverting = false;
    private const float SPHERE_CAST_RADIUS = 0.2f;
    private int _reverterLayerMask;
    private const float SPHERE_CAST_MAX_DISTANCE = 10f;
    private const string REVERTABLE_LAYER_NAME = "Revertable";

    private bool _hadHitLastFrame;
    private float _currentHitDistance;


    void Start()
    {
        _reverterLayerMask = LayerMask.GetMask(REVERTABLE_LAYER_NAME);
    }

    private void FixedUpdate()
    {
        if (!_isReverting)
        {
            _hadHitLastFrame = false;
            return;
        }

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, SPHERE_CAST_RADIUS, transform.forward, out hit, SPHERE_CAST_MAX_DISTANCE, _reverterLayerMask))
        {
            RevertableBase revertable = hit.transform.GetComponentInParent<RevertableBase>();
            if (revertable == null)
            {
                Debug.LogError($"[Reverter] Object: {hit.transform.gameObject.name} does not have an IRevertable!");
                return;
            }
            revertable.RevertObject();
            Debug.Log("Hit");
            _hadHitLastFrame = true;
            _currentHitDistance = hit.distance;
        }
        else
        {
            _hadHitLastFrame = false;
        }
    }

    public void Revert(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isReverting = true;
        }
        else if (context.canceled)
        {
            _isReverting = false;
        }
    }

    private void OnDrawGizmos()
    {
        // Only draw if the game is running and the player is actively reverting
        if (!Application.isPlaying || !_isReverting) return;

        // Change color based on whether it hit something or not
        Gizmos.color = _hadHitLastFrame ? Color.red : Color.green;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        // Calculate how far the debug drawing should extend
        float castLength = _hadHitLastFrame ? _currentHitDistance : SPHERE_CAST_MAX_DISTANCE;
        Vector3 endPoint = origin + direction * castLength;

        // 1. Draw the origin sphere
        Gizmos.DrawWireSphere(origin, SPHERE_CAST_RADIUS);

        // 2. Draw the destination sphere (either at max distance or at the hit point)
        Gizmos.DrawWireSphere(endPoint, SPHERE_CAST_RADIUS);

        // 3. Draw connecting lines to represent the outer bounds of the cast capsule
        // We calculate the orthogonal directions (Up and Right relative to the forward direction)
        Vector3 upOffset = transform.up * SPHERE_CAST_RADIUS;
        Vector3 rightOffset = transform.right * SPHERE_CAST_RADIUS;

        // Top and Bottom lines
        Gizmos.DrawLine(origin + upOffset, endPoint + upOffset);
        Gizmos.DrawLine(origin - upOffset, endPoint - upOffset);

        // Left and Right lines
        Gizmos.DrawLine(origin + rightOffset, endPoint + rightOffset);
        Gizmos.DrawLine(origin - rightOffset, endPoint - rightOffset);
    }
}
