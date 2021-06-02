using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleControllerScript : MonoBehaviour
{
    [SerializeField]
    float maxTorque, maxStearAngle, maxBreakTorque;

    [SerializeField]
    List<WheelCollider> wheelColliders = new List<WheelCollider>();
    [SerializeField]
    List<GameObject> wheelMeshes = new List<GameObject>();

    [SerializeField]
    ParticleSystem[] tireSmoke;

    [SerializeField]
    AudioSource wheelSkidAudio;

    [SerializeField]
    List<GameObject> breakLights = new List<GameObject>();

    //Engine and Gear
    [Header("Engine and Gear")]
    public Rigidbody rb;
    [SerializeField] float gearLength = 3;
    public float currentSpeed { get { return rb.velocity.magnitude * gearLength; } }
    [SerializeField] float lowPitch = 1f;
    [SerializeField] float highPitch = 6f;
    [SerializeField] int numGears = 5;
    float rpm;
    int currentGear = 1;
    float currentGearPerc;
    public float maxSpeed = 200;

    [SerializeField] AudioSource highAccel;

    public void Drive(float accelVal, float stearVal, float breakVal)
    {
        ProcessMovement(accelVal);
        StearCar(stearVal);
        ApplyBreak(breakVal);
        CheckForSkid();
        UpdateWheelMesh();
        CalculateEngineSound();
    }

    private void ApplyBreak(float appliedBreakVal)
    {
        wheelColliders[0].brakeTorque = maxBreakTorque * Mathf.Clamp(appliedBreakVal, 0, 1);
        wheelColliders[1].brakeTorque = maxBreakTorque * Mathf.Clamp(appliedBreakVal, 0, 1);

        if(appliedBreakVal != 0)
        {
            foreach (var light in breakLights)
            {
                light.SetActive(true);
            }
        }
        else
        {
            foreach (var light in breakLights)
            {
                light.SetActive(false);
            }
        }
    }

    private void ProcessMovement(float acceleration)
    {
        
        foreach (var wheel in wheelColliders)
        {
            wheel.motorTorque = acceleration * maxTorque;   
        }
    }

    private void UpdateWheelMesh()
    {
        for (int i = 0; i < wheelMeshes.Count; i++)
        {
            Vector3 pos;
            Quaternion rot;
            wheelColliders[i].GetWorldPose(out pos, out rot);
            wheelMeshes[i].transform.position = pos;
            wheelMeshes[i].transform.rotation = rot;
        }
    }

    private void StearCar(float stearVal)
    {
        wheelColliders[0].steerAngle = maxStearAngle * stearVal;
        wheelColliders[1].steerAngle = maxStearAngle * stearVal;
    }

    private void CheckForSkid()
    {
        int numSkidding = 0;
        for(int i = 0; i < wheelColliders.Count; i++)
        {
            WheelHit wheelHit;
            wheelColliders[i].GetGroundHit(out wheelHit);
            if(Mathf.Abs(wheelHit.forwardSlip) > 0.5f || Mathf.Abs(wheelHit.sidewaysSlip) > 0.5f)
            {
                numSkidding++;
                if(!wheelSkidAudio.isPlaying)
                    wheelSkidAudio.Play();
                if(tireSmoke[i].emission.enabled == false)
                {
                    var emmitorModule = tireSmoke[i].emission;
                    emmitorModule.enabled = true;
                }
            }
            else
            {
                var emmitorModule = tireSmoke[i].emission;
                emmitorModule.enabled = false;
            }
        }
        if(numSkidding == 0 && wheelSkidAudio.isPlaying)
        {
            wheelSkidAudio.Stop();
        }
    }

    private void CalculateEngineSound()
    {
        float gearPercentage = (1 / (float)numGears);
        float targetGearFactor = Mathf.InverseLerp(gearPercentage * currentGear, gearPercentage * (currentGear + 1),
                                                    Mathf.Abs(currentSpeed / maxSpeed));
        currentGearPerc = Mathf.Lerp(currentGearPerc, targetGearFactor, Time.deltaTime * 5f);

        var gearNumFactor = currentGear / (float)numGears;
        rpm = Mathf.Lerp(gearNumFactor, 1, currentGearPerc);

        float speedPercentage = Mathf.Abs(currentSpeed / maxSpeed);
        float upperGearMax = (1 / (float)numGears) * (currentGear + 1);
        float downGearMax = (1 / (float)numGears) * currentGear;

        if (currentGear > 0 && speedPercentage < downGearMax)
            currentGear--;

        if (speedPercentage > upperGearMax && (currentGear < (numGears - 1)))
            currentGear++;

        float pitch = Mathf.Lerp(lowPitch, highPitch, rpm);
        highAccel.pitch = Mathf.Min(highPitch, pitch) * 0.25f;

    }
}
