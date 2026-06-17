using UnityEngine;

//NOTES
// line renderer index 0 is player, last is revertable
public abstract class RevertableBase : MonoBehaviour
{
    [SerializeField] private Material _aimedAtMaterial;
    [SerializeField] private Material _notAimedAtMaterial;
    protected bool _isActive = true;
    public abstract void RevertObject();
    private LineRenderer _lineRenderer;
    private Transform _lineRendererTargetTransform;
    protected PlayerControlsComponent _playerControlsComponent;
    private bool _isAimedAt = false;

    public bool IsAimedAt => _isAimedAt;

    protected virtual void Start()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, transform.position);
        _lineRenderer.enabled = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerControlsComponent = player.GetComponent<PlayerControlsComponent>();

        OnAimedLost();

    }
    private void OnEnable()
    {
        _isActive = true;
    }

    private void LateUpdate()
    {
        if(_lineRenderer == null)
        {
            return;
        }
        if (_lineRenderer.enabled)
        {
            _lineRenderer.SetPosition(0, _lineRendererTargetTransform.position);
        }
    }

    public void OnInPlayerView()
    {
        _lineRenderer.enabled = true;
        _lineRendererTargetTransform = _playerControlsComponent.GetPlayerWeapon().GetBarrelOutTransform(); //when switching weapon later in inventory (although wont fix when switching weapon in temporal mode)
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, transform.position);
    }

    public void OnOutPlayerView()
    {
        _lineRenderer.enabled = false;
        OnAimedLost();
    }

    public void OnAimedAt()
    {
        _lineRenderer.material = _aimedAtMaterial;
        _isAimedAt = true;
    }

    public void OnAimedLost()
    {
        _lineRenderer.material = _notAimedAtMaterial;
        _isAimedAt = false;
    }
}
