using System.Collections;
using UnityEngine;

public class GravityChangerComponent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(RotateGravity(new Vector3(0f, 10f, 0f), 2f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator RotateGravity(Vector3 newGravity, float inTime)
    {
        Vector3 start = Physics.gravity;
        float t = 0f;

        while (t < inTime)
        {
            t += Time.deltaTime;
            Physics.gravity = Vector3.Lerp(start, newGravity, t / inTime);
            yield return null;
        }

        Physics.gravity = newGravity;
    }
}
