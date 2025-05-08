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
    [SerializeField] public float motorForce = 1000f;
    [SerializeField] public float breakForce = 1500f;
    [SerializeField] public float maxSteerAngle = 28f;
    [SerializeField] public float decelerationSpeed = 2.5f;
    [SerializeField] private float maxSpeed = 22.35f; // ≈ 50 MPH in m/s
    [SerializeField] private float downforce = 100f;

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

        // Adjust friction on all wheels
        AdjustWheelFriction(frontLeftWheelCollider);
        AdjustWheelFriction(frontRightWheelCollider);
        AdjustWheelFriction(rearLeftWheelCollider);
        AdjustWheelFriction(rearRightWheelCollider);
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        DisplaySpeed();
        CapMaxSpeed();

        ApplyDownforce();
    }

    private void LateUpdate()
    {
        ReduceSideSlip();
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

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void ApplyDeceleration()
    {
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, decelerationSpeed * Time.fixedDeltaTime);
    }

    private void HandleSteering()
    {
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / 10f);
        float steerLimit = Mathf.Lerp(maxSteerAngle, maxSteerAngle * 0.4f, speedFactor);
        currentSteerAngle = steerLimit * horizontalInput;

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
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.rotation = rot * Quaternion.Euler(0, 90, 90);
        wheelTransform.position = pos;
    }

    private void UpdateSingleWheelRight(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.rotation = rot * Quaternion.Euler(0, -90, 90);
        wheelTransform.position = pos;
    }

    private void DisplaySpeed()
    {
        float speed = rb.linearVelocity.magnitude * 2.23694f; // Convert to mph
        speed = Mathf.Min(speed, 50f);
        speedText.text = Mathf.Round(speed) + " mph";
    }

    private void CapMaxSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.linearVelocity.magnitude);
    }

    private void ReduceSideSlip()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        localVelocity.x *= 0.95f; // Damp sideways velocity
        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }

    private void AdjustWheelFriction(WheelCollider wheel)
    {
        WheelFrictionCurve forward = wheel.forwardFriction;
        forward.stiffness = 2.0f;
        wheel.forwardFriction = forward;

        WheelFrictionCurve sideways = wheel.sidewaysFriction;
        sideways.stiffness = 2.5f;
        wheel.sidewaysFriction = sideways;
    }
}
