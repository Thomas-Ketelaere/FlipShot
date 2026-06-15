using System.Collections.Generic;
using UnityEngine;

public class RevertableChecker : MonoBehaviour
{
    private List<RevertableBase> _revertablesInView = new();
    private Reverter _reverter;
    private MeshCollider _meshCollider;

    private void Start()
    {
        _reverter = GetComponentInParent<Reverter>();
        _reverter.OnEnterTemporalMode.AddListener(OnTemporalEnter); 
        _reverter.OnExitTemporalMode.AddListener(OnTemporalExit);
        _meshCollider = GetComponent<MeshCollider>();
        _meshCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        RevertableBase revertableBase = other.GetComponentInParent<RevertableBase>();
        Debug.Log("Hit: " + revertableBase.GetType().Name);
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
}
