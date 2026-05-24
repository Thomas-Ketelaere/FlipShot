using UnityEngine;

public abstract class RevertableBase : MonoBehaviour
{
    protected bool _isActive = true;
    public abstract void RevertObject();

    private void OnEnable()
    {
        _isActive = true;
    }
}
