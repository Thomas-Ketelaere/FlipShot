using Unity.VisualScripting;
using UnityEngine;

public class BulletComponent : MonoBehaviour
{
    [SerializeField] private GameObject _bulletHoleWallObj;
    [SerializeField] private GameObject _bulletHoleGlassObj;
    private int _damage;
    private float _speed;
    private const float LifeTime = 1f;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 prevPos = transform.position;
        Vector3 nextPos = prevPos + transform.forward * _speed * Time.fixedDeltaTime;

        //check if hit player/enemy
        RaycastHit hit;
        if (Physics.Linecast(prevPos, nextPos, out hit, LayerMask.GetMask("Hittable"))) //not including the walls and stuff for "bullet hole" 
        {
            if(hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Player/Enemy hit");
                //get health comp and do damage
            }

            else if(hit.collider.gameObject.CompareTag("Object"))
            {
                GameObject bulletHoleObj = BulletsManager.Instance.RequestBulletHoleObject();
                if (bulletHoleObj != null)
                {
                    bulletHoleObj.transform.position = hit.point + hit.normal * 0.1f;
                    bulletHoleObj.transform.rotation = Quaternion.LookRotation(-hit.normal);
                }
            }

            else if (hit.collider.gameObject.CompareTag("Glass"))
            {
                Instantiate(_bulletHoleGlassObj, hit.point + hit.normal * 0.1f, Quaternion.LookRotation(-hit.normal));
            }

            CancelInvoke("SetBulletInactive");
            SetBulletInactive();
        }

        transform.position = nextPos;   
    }

    public void SetBulletActive(int damage, float speed)
    {
        _damage = damage;
        _speed = speed;
        Invoke("SetBulletInactive", LifeTime);
    }

    public void SetBulletInactive()
    {
        CancelInvoke("SetBulletInactive"); //when pool full, this one gets set inactive, so need to cancel for lifetime since it is shorter
        transform.gameObject.SetActive(false);
    }
}
