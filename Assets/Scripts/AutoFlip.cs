using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFlip : MonoBehaviour
{

    float lastTimeChecked;
    Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.up.y > 0.3 || rigidbody.velocity.magnitude > 1)
        {
            lastTimeChecked = Time.time;
        }
        else
        {
            if(Time.time > lastTimeChecked + 3f)
            {
                transform.position += Vector3.up;
                transform.rotation = Quaternion.LookRotation(transform.forward);
            }
        }
    }
}
