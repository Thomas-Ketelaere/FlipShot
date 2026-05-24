using UnityEngine;

public class GravityRotator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 gravity = Physics.gravity.normalized;
        float angle = Mathf.Atan2(gravity.y, gravity.x);
        angle += Mathf.PI / 2; // y is down in circle so need to change it
        transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);
    }
}
