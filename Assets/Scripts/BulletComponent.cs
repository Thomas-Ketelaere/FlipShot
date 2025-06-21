using Unity.VisualScripting;
using UnityEngine;

public class BulletComponent : MonoBehaviour
{
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
        if (Physics.Linecast(prevPos, nextPos, out hit, LayerMask.GetMask("Player, Enemy"))) //not including the walls and stuff for "bullet hole" 
        {
            Debug.Log("Player/Enemy hit");
            //get health comp and do damage
            CancelInvoke("SetInactive");
            SetInactive();
        }

        transform.position = nextPos;   
    }

    public void SetBulletActive(int damage, float speed)
    {
        _damage = damage;
        _speed = speed;
        transform.gameObject.SetActive(true);
        Invoke("SetInactive", LifeTime);
    }

    private void SetInactive()
    {
        transform.gameObject.SetActive(false);
    }
}
