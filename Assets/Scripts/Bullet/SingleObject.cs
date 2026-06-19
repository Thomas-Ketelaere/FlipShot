using UnityEngine;

//Base class for all objects that need to be in object pool
public class SingleObject : MonoBehaviour
{
    [SerializeField] private float LifeTime = 20f;

    public virtual void SetActive()
    {
        Invoke("SetInactive", LifeTime);
    }
    public virtual void SetInactive()
    {
        CancelInvoke("SetInactive"); //when pool full, this one gets set inactive, so need to cancel for lifetime since it is shorter
        gameObject.SetActive(false);
    }
}
