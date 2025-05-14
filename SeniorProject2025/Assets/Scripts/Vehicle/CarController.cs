using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarController : MonoBehaviour
{
    public float horizontalInput, verticalInput;
    public float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    [Header("Car Settings")]
    [SerializeField] public float motorForce = 1000f;
    [SerializeField] public float breakForce = 1500f;
    [SerializeField] public float maxSteerAngle = 28f;
    [SerializeField] public float decelerationSpeed = 2.5f;
    [SerializeField] private float maxSpeed = 22.35f; // ~50 mph
    [SerializeField] private float downforce = 100f;

    [Header("Wheel Colliders")]
    [SerializeField] public WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] public WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    [Header("UI")]
    [SerializeField] private TMP_Text speedText;

    public Rigidbody rb;

    // Friction handling
    private float currentForwardStiffness = 2.0f;
    private float currentSidewaysStiffness = 2.5f;
    private float targetForwardStiffness = 2.0f;
    private float targetSidewaysStiffness = 2.5f;
    private float frictionLerpSpeed = 3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.1f, 0);

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
        CapMaxSpeedWithFriction();
        ApplyDownforce();
        UpdateWheelFrictionSmoothly();
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
        float velocityZ = transform.InverseTransformDirection(rb.linearVelocity).z;

        if (verticalInput != 0)
        {
            bool switchingDirection = (verticalInput > 0 && velocityZ < -0.5f) || (verticalInput < 0 && velocityZ > 0.5f);
            float torqueMultiplier = switchingDirection ? 1.75f : 1f;

            float torque = -verticalInput * motorForce * torqueMultiplier;
            frontLeftWheelCollider.motorTorque = torque;
            frontRightWheelCollider.motorTorque = torque;

            if (switchingDirection)
            {
                targetForwardStiffness = 1.5f;
                targetSidewaysStiffness = 2.0f;
            }
            else
            {
                targetForwardStiffness = 2.0f;
                targetSidewaysStiffness = 2.5f;
            }
        }
        else
        {
            ApplyDeceleration();
            targetForwardStiffness = 2.0f;
            targetSidewaysStiffness = 2.5f;
        }

        // Extra brake force if player is holding forward/reverse and braking
        if (isBreaking && Mathf.Abs(verticalInput) > 0.1f)
        {
            currentbreakForce = breakForce * 1.5f; // Stronger brake while accelerating
            frontLeftWheelCollider.motorTorque = 0f;
            frontRightWheelCollider.motorTorque = 0f;
        }
        else
        {
            currentbreakForce = isBreaking ? breakForce : 0f;
        }
        ApplyBraking();

    }

    private void ApplyBraking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void ApplyDeceleration()
    {
        frontLeftWheelCollider.motorTorque = 0f;
        frontRightWheelCollider.motorTorque = 0f;
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
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform, true);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform, false);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform, true);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform, false);
    }

    private void UpdateSingleWheel(WheelCollider collider, Transform transform, bool isLeft)
    {
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        transform.rotation = rot * Quaternion.Euler(0, isLeft ? 90 : -90, 90);
        transform.position = pos;
    }

    private void DisplaySpeed()
    {
        float speed = rb.linearVelocity.magnitude * 2.23694f;
        speed = Mathf.Min(speed, 50f);
        speedText.text = Mathf.Round(speed) + " mph";
    }

    private void CapMaxSpeedWithFriction()
    {
        float frictionFactor = Mathf.Lerp(0.75f, 1f, currentForwardStiffness / 2.0f);
        float adjustedMaxSpeed = maxSpeed * frictionFactor;

        if (rb.linearVelocity.magnitude > adjustedMaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * adjustedMaxSpeed;
        }
    }

    private void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.linearVelocity.magnitude);
    }

    private void ReduceSideSlip()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        localVelocity.x *= 0.95f;
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

    private void UpdateWheelFrictionSmoothly()
    {
        currentForwardStiffness = Mathf.Lerp(currentForwardStiffness, targetForwardStiffness, Time.fixedDeltaTime * frictionLerpSpeed);
        currentSidewaysStiffness = Mathf.Lerp(currentSidewaysStiffness, targetSidewaysStiffness, Time.fixedDeltaTime * frictionLerpSpeed);
        ApplyFrictionToAllWheels(currentForwardStiffness, currentSidewaysStiffness);
    }

    private void ApplyFrictionToAllWheels(float forwardStiffness, float sidewaysStiffness)
    {
        SetWheelFriction(frontLeftWheelCollider, forwardStiffness, sidewaysStiffness);
        SetWheelFriction(frontRightWheelCollider, forwardStiffness, sidewaysStiffness);
        SetWheelFriction(rearLeftWheelCollider, forwardStiffness, sidewaysStiffness);
        SetWheelFriction(rearRightWheelCollider, forwardStiffness, sidewaysStiffness);
    }

    private void SetWheelFriction(WheelCollider wheel, float forwardStiffness, float sidewaysStiffness)
    {
        WheelFrictionCurve forward = wheel.forwardFriction;
        forward.stiffness = forwardStiffness;
        wheel.forwardFriction = forward;

        WheelFrictionCurve sideways = wheel.sidewaysFriction;
        sideways.stiffness = sidewaysStiffness;
        wheel.sidewaysFriction = sideways;
    }
}
