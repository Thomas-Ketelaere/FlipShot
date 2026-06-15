using UnityEngine;

//NOTES
// line renderer index 0 is player, last is revertable
public abstract class RevertableBase : MonoBehaviour
{
    protected bool _isActive = true;
    public abstract void RevertObject();
    private LineRenderer _lineRenderer;
    private Transform _lineRendererTargetTransform;

    protected virtual void Start()
    {
        Debug.Log("Called start");
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, transform.position);
        _lineRenderer.enabled = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerControlsComponent playerMovementComponent = player.GetComponent<PlayerControlsComponent>();
        _lineRendererTargetTransform = playerMovementComponent.GetPlayerWeapon().GetBarrelOutTransform();


    }
    private void OnEnable()
    {
        _isActive = true;
    }

    private void FixedUpdate()
    {
        if(_lineRenderer == null)
        {
            return;
        }
        if (_lineRenderer.enabled)
        {
            _lineRenderer.SetPosition(0, _lineRendererTargetTransform.position - new Vector3(0, 0.2f, 0f));
        }
    }

    public void OnInPlayerView()
    {
        _lineRenderer.enabled = true;
    }

    public void OnOutPlayerView()
    {
        _lineRenderer.enabled = false;
    }
}
