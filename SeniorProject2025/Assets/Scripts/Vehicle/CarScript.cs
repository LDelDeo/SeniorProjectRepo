using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    [Header("Wheel Transforms")]
    public Transform frontLeftTransform, frontRightTransform, rearLeftTransform, rearRightTransform;

    [Header("Statistical Values")]
    public float maxTorque = 200f;
    public float maxSteerAngle = 30f;

    private void FixedUpdate() 
    {
       float acceleration = Input.GetAxis("Vertical") * maxTorque;
       float steering = Input.GetAxis("Horizontal") * maxSteerAngle; 

       // Torque to Rear Wheels
       rearLeftWheel.motorTorque = acceleration;
       rearRightWheel.motorTorque = acceleration;

       // Steering to Front Wheels
       frontLeftWheel.steerAngle = steering;
       frontRightWheel.steerAngle = steering;

       UpdateWheelPos(frontLeftWheel, frontLeftTransform);
       UpdateWheelPos(frontRightWheel, frontRightTransform);
       UpdateWheelPos(rearLeftWheel, rearLeftTransform);
       UpdateWheelPos(rearRightWheel, rearRightTransform);
    }

    private void UpdateWheelPos(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;

        collider.GetWorldPose(out pos, out rot);

        wheelTransform.position = pos;
        wheelTransform.rotation = rot * Quaternion.Euler(0, 90, 0);
    }
}
