using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    VehicleControllerScript vCS;

    [SerializeField]
    RaceHandler raceHandler;

    Vector3 lastGoodPosition;
    float lastTimeChecked;

    private void Update()
    {
        if (raceHandler.isRaceOn)
        {
            vCS.Drive(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetAxis("Jump"));
            CheckForOffRoad();
        }
        

    }

    private void CheckForOffRoad()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit, 10f))
        {
            print("Hit");
            if(hit.transform.tag == "Road")
            {
                lastGoodPosition = vCS.rb.position;
                lastTimeChecked += Time.time;
                print("Road");
            }
            else
            {
                //print(hit.transform.name);
            }
        }

        if(Time.time > lastTimeChecked + 5f)
        {
            transform.position = lastGoodPosition;
            gameObject.layer = 8;
            GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3f);
        }
    }

    void ResetLayer()
    {
        gameObject.layer = default;
        GetComponent<Ghost>().enabled = false;
    }
}
