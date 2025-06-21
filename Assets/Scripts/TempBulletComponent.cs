using UnityEngine;

public class TempBulletComponent : MonoBehaviour
{
    [SerializeField] private float LifeTime = 20f;

    public void SetActive()
    {
        Invoke("SetInactive", LifeTime);
    }
    public void SetInactive()
    {
        CancelInvoke("SetInactive"); //when pool full, this one gets set inactive, so need to cancel for lifetime since it is shorter
        gameObject.SetActive(false);
    }
}
