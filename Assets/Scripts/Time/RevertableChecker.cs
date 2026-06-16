using System.Collections.Generic;
using UnityEngine;

public class RevertableChecker : MonoBehaviour
{
    private List<RevertableBase> _revertablesInView = new();
    private Dictionary<RevertableBase, bool> _revertablesInViewDictionary = new();
    private Reverter _reverter;
    private MeshCollider _meshCollider;
    private Camera _camera;

    private const float MAX_DELTA_ANGLE_REVERTABLE = 2; //in degrees

    private void Start()
    {
        _reverter = GetComponentInParent<Reverter>();
        _reverter.OnEnterTemporalMode.AddListener(OnTemporalEnter); 
        _reverter.OnExitTemporalMode.AddListener(OnTemporalExit);
        _meshCollider = GetComponent<MeshCollider>();
        _meshCollider.enabled = false;
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (!_meshCollider.enabled)
        {
            return;
        }

        for (int i = 0; i < _revertablesInView.Count; i++)
        {
            RevertableBase revertable = _revertablesInView[i];
            Vector3 directionToRevertable = (revertable.transform.position - _camera.transform.position).normalized;
            float angle = Vector3.Angle(_camera.transform.forward, directionToRevertable);
            bool isAimedAt = angle <= MAX_DELTA_ANGLE_REVERTABLE;
            if (isAimedAt && !revertable.IsAimedAt)
            {
                revertable.OnAimedAt();
            }
            else if (!isAimedAt && revertable.IsAimedAt)
            {
                revertable.OnAimedLost();
            }

            Debug.DrawLine(_camera.transform.position, revertable.transform.position, isAimedAt ? Color.green : Color.red);
            Debug.DrawRay(_camera.transform.position, _camera.transform.forward * 10f, Color.blue);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        RevertableBase revertableBase = other.GetComponentInParent<RevertableBase>();
        revertableBase.OnInPlayerView();
        _revertablesInView.Add(revertableBase);
    }

    private void OnTriggerExit(Collider other)
    {
        RevertableBase revertableBase = other.GetComponentInParent<RevertableBase>();
        revertableBase.OnOutPlayerView();
        _revertablesInView.Remove(revertableBase); //todo can this be optimized?
    }

    private void OnTemporalEnter()
    {
        _meshCollider.enabled = true;
    }

    private void OnTemporalExit()
    {
        _meshCollider.enabled = false;
    }

    public RevertableBase GetFirstRevertableInAim()
    {
        return _revertablesInView.Find(revert => revert.IsAimedAt == true);
    }
}
