using System.Collections.Generic;
using UnityEngine;

public class RevertableChecker : MonoBehaviour
{
    private List<RevertableBase> _revertablesInView = new();

    private void OnTriggerEnter(Collider other)
    {
        RevertableBase revertableBase = other.GetComponentInParent<RevertableBase>();
        Debug.Log("Hit: " + revertableBase.GetType().Name);
        _revertablesInView.Add(revertableBase);
    }

    private void OnTriggerExit(Collider other)
    {
        RevertableBase revertableBase = other.GetComponentInParent<RevertableBase>();
        _revertablesInView.Remove(revertableBase); //todo can this be optimized?
    }
}
