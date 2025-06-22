using UnityEngine;

public class TempObjectComponent : MonoBehaviour
{
    [SerializeField] private float LifeTime = 10f;

    void Awake()
    {
        Invoke(nameof(DestroySelf), LifeTime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

}
