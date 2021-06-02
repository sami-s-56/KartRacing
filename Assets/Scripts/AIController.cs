using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    public float brakingSensitivity = 3f;
    VehicleControllerScript ds;
    public float steeringSensitivity = 0.01f;
    public float accelSensitivity = 0.3f;
    Vector3 target;
    Vector3 nextTarget;
    int currentWP = 0;
    float totalDistanceToTarget;

    GameObject tracker;
    int currentTrackerWP = 0;
    float lookAhead = 10;

    RaceHandler rHandler;

    float lastTimeMoving = 0;
   


    // Start is called before the first frame update
    void Start()
    {
        ds = this.GetComponent<VehicleControllerScript>();
        rHandler = FindObjectOfType<RaceHandler>();

        target = circuit.waypoints[currentWP].transform.position;
        nextTarget = circuit.waypoints[currentWP + 1].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, ds.rb.gameObject.transform.position);

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.transform.position = ds.rb.gameObject.transform.position;
        tracker.transform.rotation = ds.rb.gameObject.transform.rotation;
    }


    void ProgressTracker()
    {
        Debug.DrawLine(ds.rb.gameObject.transform.position, tracker.transform.position);

        if (Vector3.Distance(ds.rb.gameObject.transform.position, tracker.transform.position) > lookAhead) return;

        tracker.transform.LookAt(circuit.waypoints[currentTrackerWP].transform.position);
        tracker.transform.Translate(0, 0, 1.0f);  //speed of tracker

        if (Vector3.Distance(tracker.transform.position, circuit.waypoints[currentTrackerWP].transform.position) < 1)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.waypoints.Count)
                currentTrackerWP = 0;
        }

    }

    
    void Update()
    {
        ProgressTracker();
        Vector3 localTarget;

        if (Time.time < GetComponent<Aviodance>().avoidTime)
        {
            localTarget = tracker.transform.right * GetComponent<Aviodance>().avoidPath;
        }
        else
        {
            localTarget = ds.rb.gameObject.transform.InverseTransformPoint(tracker.transform.position);
        }

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(ds.currentSpeed);

        float speedFactor = ds.currentSpeed / ds.maxSpeed;

        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90f;

        float brake;
        if(corner > 20 && speedFactor > 0.1)
        {
            brake = Mathf.Lerp(0, 1 + speedFactor, cornerFactor);
        }
        else
        {
            brake = 0;
        }

        float accel = 1;
        if(corner > 20 && speedFactor > 0.2)
        {
            accel = Mathf.Lerp(0, 1, 1 - cornerFactor);
        }
        else
        {
            accel = 1;
        }

        if (rHandler.isRaceOn)
        {
            ds.Drive(accel, steer, brake);
        }

        if(ds.rb.velocity.magnitude > 1 || !rHandler.isRaceOn)
        {
            lastTimeMoving = Time.time;
        }
        else
        {
            if(Time.time > lastTimeMoving + 3f)
            {
                transform.position = circuit.waypoints[currentTrackerWP - 1].transform.position + Vector3.up;
                tracker.transform.position = transform.position;
                gameObject.layer = 8;
                GetComponent<Ghost>().enabled = true;
                Invoke("ResetLayer", 3f);
            }
        }

    }

    void ResetLayer()
    {
        gameObject.layer = default;
        GetComponent<Ghost>().enabled = false;
    }
}