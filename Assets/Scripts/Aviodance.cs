using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aviodance : MonoBehaviour
{
    public float avoidPath; //Collision detecting distance
    public float avoidTime;
    [SerializeField] float wanderDistance = 4; //How much away it should move from other object
    [SerializeField] float avoidLength = 1;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag != "Car") return;

        Rigidbody otherCar = collision.rigidbody;
        avoidTime = Time.time + avoidLength;

        Vector3 otherCarLocalTarget = transform.InverseTransformPoint(otherCar.transform.position);
        float otherCarAngle = Mathf.Atan2(otherCarLocalTarget.x, otherCarLocalTarget.z);
        avoidPath = wanderDistance * -Mathf.Sign(otherCarAngle);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Car") return;
        avoidTime = 0;
    }

}
