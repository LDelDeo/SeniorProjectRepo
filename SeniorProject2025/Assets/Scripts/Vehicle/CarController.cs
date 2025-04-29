using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    // Settings
    [SerializeField] public float motorForce, breakForce, maxSteerAngle, decelerationSpeed;
    [SerializeField] private float maxSpeed = 22.35f; // ≈ 50 MPH in m/s

    // Wheel Colliders
    [SerializeField] public WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] public WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    // UI Speed Display
    [SerializeField] private TMP_Text speedText;

    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.1f, 0);
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        DisplaySpeed();
        CapMaxSpeed();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        if (verticalInput != 0)
        {
            frontLeftWheelCollider.motorTorque = -verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = -verticalInput * motorForce;
        }
        else
        {
            ApplyDeceleration();
        }

        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    public void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    public void ApplyDeceleration()
    {
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, decelerationSpeed * Time.fixedDeltaTime);
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheelRight(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheelRight(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot * Quaternion.Euler(0, 90, 90);
        wheelTransform.position = pos;
    }

    private void UpdateSingleWheelRight(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot * Quaternion.Euler(0, -90, 90);
        wheelTransform.position = pos;
    }

    private void DisplaySpeed()
    {
        float speed = rb.linearVelocity.magnitude * 2.23694f; // Convert to mph
        speed = Mathf.Min(speed, 50f); // Cap display at 50 MPH
        speedText.text = Mathf.Round(speed) + " mph";
    }

    private void CapMaxSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}
