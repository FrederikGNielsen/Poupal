using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Android;
using TMPro;

public class ActivityManager : MonoBehaviour
{
    public InputAction stepAction;
    public int steps;
    public float distanceThreshold;
    public float currentDistance;

    public float maxSpeed;
    
    public float currentSpeed;

    bool phoneIsMoving = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.ACTIVITY_RECOGNITION"))
        {
            Permission.RequestUserPermission("android.permission.ACTIVITY_RECOGNITION");
        }
 
        InputSystem.EnableDevice(StepCounter.current);
        steps = 0;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        currentSpeed = new Vector3(Accelerometer.current.acceleration.ReadValue().x, 0, Accelerometer.current.acceleration.ReadValue().z).magnitude;
        Application.targetFrameRate = Mathf.RoundToInt(currentSpeed * 5f);
        Vector3 movement = Accelerometer.current.acceleration.ReadValue();
        Debug.Log("");
        
        if (StepCounter.current != null && StepCounter.current.enabled) {
            if (StepCounter.current.stepCounter.ReadValue() > steps) {
                steps = StepCounter.current.stepCounter.ReadValue();
                print(steps);
            }
        } else {
            print("No step counter found!");
        }
    }
}