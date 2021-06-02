using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    [SerializeField]
    float maxTransferForce;

    [SerializeField] WheelCollider wFL;
    [SerializeField] WheelCollider wFR;
    [SerializeField] WheelCollider wRL;
    [SerializeField] WheelCollider wRR;
    [SerializeField] GameObject _centerOfMass;

    Rigidbody _rigidbody;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = _centerOfMass.transform.localPosition;
    }

    void GroundWheels(WheelCollider wl, WheelCollider wr)
    {
        WheelHit hit;
        float leftDistance = 1.0f;
        float rightDistance = 1.0f;

        bool groundedL = wl.GetGroundHit(out hit);
        if (groundedL)
        {
            leftDistance = (-wl.transform.InverseTransformPoint(hit.point).y - wl.radius) / wl.suspensionDistance;
        }

        bool groundedR = wl.GetGroundHit(out hit);
        if (groundedR)
        {
            leftDistance = (-wr.transform.InverseTransformPoint(hit.point).y - wr.radius) / wr.suspensionDistance;
        }

        float antiRollForce = (leftDistance - rightDistance) * maxTransferForce;

        if (groundedL)
        {
            _rigidbody.AddForceAtPosition(wl.transform.up * antiRollForce, wl.transform.position);
        }

        if (groundedR)
        {
            _rigidbody.AddForceAtPosition(wr.transform.up * antiRollForce, wr.transform.position);
        }
    }

    private void FixedUpdate()
    {
        GroundWheels(wFL, wFR);
        GroundWheels(wRL, wRR);
    }
}
