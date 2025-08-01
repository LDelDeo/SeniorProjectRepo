using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarController : MonoBehaviour
{
    public float horizontalInput, verticalInput;
    public float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    private EnterCarScript enterCarScript;

    [Header("Car Settings")]
    [SerializeField] public float motorForce = 5000f;
    [SerializeField] public float breakForce = 1500f;
    [SerializeField] public float maxSteerAngle = 28f;
    [SerializeField] public float decelerationSpeed = 2.5f;
    [SerializeField] private float maxSpeed = 18f; // ~39 mph
    [SerializeField] private float downforce = 100f;

    [Header("Engine Audio")]
    [SerializeField] public AudioSource engineAudioSource;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 2.0f;
    private bool engineWasPlaying = false;
    private float currentPitch = 1f;

    [Header("Wheel Colliders")]
    [SerializeField] public WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] public WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    [Header("UI")]
    [SerializeField] private TMP_Text speedText;

    [Header("VFX")]
    public ParticleSystem[] exhaustSmokes;

    public Rigidbody rb;

    // Friction handling
    private float currentForwardStiffness = 2.0f;
    private float currentSidewaysStiffness = 2.5f;
    private float targetForwardStiffness = 2.0f;
    private float targetSidewaysStiffness = 2.5f;
    private float frictionLerpSpeed = 3f;

    // Distance Tracking
    private Vector3 lastPosition;

    private void Start()
    {
        enterCarScript = GetComponent<EnterCarScript>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        lastPosition = transform.position;

        AdjustWheelFriction(frontLeftWheelCollider);
        AdjustWheelFriction(frontRightWheelCollider);
        AdjustWheelFriction(rearLeftWheelCollider);
        AdjustWheelFriction(rearRightWheelCollider);

        if (engineAudioSource != null)
        {
            engineAudioSource.loop = true;
            engineAudioSource.playOnAwake = false;
            engineAudioSource.pitch = minPitch;
            float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            engineAudioSource.volume = Mathf.Lerp(0.0001f, 1f, Mathf.Pow(savedSFXVolume, 2f));


            if (enterCarScript != null && enterCarScript.isInCar)
                engineAudioSource.Play();
            else
                engineAudioSource.Stop();
        }
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
        UpdateDriftFriction();
        UpdateWheelFrictionSmoothly();
        ApplyAntiRoll(frontLeftWheelCollider, frontRightWheelCollider);
        ApplyAntiRoll(rearLeftWheelCollider, rearRightWheelCollider);
        TrackDriveDistance();
        UpdateEngineSound();
    }

    private void TrackDriveDistance()
    {
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        float currentDistance = PlayerPrefs.GetFloat("TotalDriveDistance", 0f);
        currentDistance += distanceThisFrame;
        PlayerPrefs.SetFloat("TotalDriveDistance", currentDistance);
    }

    private void LateUpdate()
    {
        ReduceSideSlip();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = -Input.GetAxis("Vertical");
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        float velocityZ = transform.InverseTransformDirection(rb.linearVelocity).z;
        bool switchingDirection = (verticalInput > 0 && velocityZ < -0.1f) || (verticalInput < 0 && velocityZ > 0.1f);

        float torqueMultiplier = switchingDirection ? 0f : 1f; // prevent torque during switch
        float torque = Mathf.Abs(verticalInput) * motorForce * Mathf.Sign(verticalInput);

        float speed = rb.linearVelocity.magnitude;
        if (speed > 0.5f)
        {
            foreach (ParticleSystem smoke in exhaustSmokes)
            {
                if (!smoke.isPlaying)
                    smoke.Play();
            }

        }
        else
        {
            foreach (ParticleSystem smoke in exhaustSmokes)
            {
                if (smoke.isPlaying)
                    smoke.Stop();
            }
        }

        if (verticalInput > 0)
        {
            frontLeftWheelCollider.motorTorque = torque;
            frontRightWheelCollider.motorTorque = torque;
        }
        else if (verticalInput < 0)
        {
            frontLeftWheelCollider.motorTorque = torque * 1.5f;
            frontRightWheelCollider.motorTorque = torque * 1.5f;
        }

        // Apply torque only when not switching direction
        frontLeftWheelCollider.motorTorque = torque;
        frontRightWheelCollider.motorTorque = torque;

        if (switchingDirection)
        {
            // Apply a little brake to stop before switching
            currentbreakForce = breakForce * 0.6f;
        }
        else if (Mathf.Abs(verticalInput) < 0.1f)
        {
            // Natural deceleration
            ApplyDeceleration();
            return;
        }
        else
        {
            currentbreakForce = 0f;
        }

        // Apply brake if space is pressed
        if (isBreaking)
        {
            currentbreakForce = breakForce;
            frontLeftWheelCollider.motorTorque = 0f;
            frontRightWheelCollider.motorTorque = 0f;
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

        // Apply mild braking to simulate natural deceleration
        float decelBrake = breakForce * 0.4f; // You can tweak 0.4f for stronger/weaker coasting drag
        frontLeftWheelCollider.brakeTorque = decelBrake;
        frontRightWheelCollider.brakeTorque = decelBrake;
        rearLeftWheelCollider.brakeTorque = decelBrake;
        rearRightWheelCollider.brakeTorque = decelBrake;
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

    private void UpdateEngineSound()
    {
        if (engineAudioSource == null || rb == null || enterCarScript == null)
            return;

        bool inCar = enterCarScript.isInCar;

        if (inCar)
        {
            if (!engineWasPlaying)
            {
                engineAudioSource.Play();
                engineWasPlaying = true;
            }

            float speedPercent = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
            float targetPitch = Mathf.Lerp(minPitch, maxPitch, speedPercent);

            currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * 5f);
            engineAudioSource.pitch = currentPitch;
        }
        else
        {
            if (engineWasPlaying)
            {
                engineAudioSource.Stop();
                engineWasPlaying = false;
            }
        }
    }

    public void StopEngineSound()
    {
        if (engineAudioSource != null && engineAudioSource.isPlaying)
            engineAudioSource.Stop();
    }

    public void OnExitCar()
    {
        StopEngineSound();
        engineWasPlaying = false;
        foreach (ParticleSystem smoke in exhaustSmokes)
            {
                if (smoke.isPlaying)
                    smoke.Stop();
            }
    }

    private void DisplaySpeed()
    {
        float speed = rb.linearVelocity.magnitude * 2.23694f;
        speed = Mathf.Min(speed, 50f);
        speedText.text = Mathf.Round(speed) + "";
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

    private void ApplyAntiRoll(WheelCollider leftWheel, WheelCollider rightWheel)
    {
        WheelHit hit;
        float travelLeft = 1.0f;
        float travelRight = 1.0f;

        bool groundedLeft = leftWheel.GetGroundHit(out hit);
        if (groundedLeft)
            travelLeft = (-leftWheel.transform.InverseTransformPoint(hit.point).y - leftWheel.radius) / leftWheel.suspensionDistance;

        bool groundedRight = rightWheel.GetGroundHit(out hit);
        if (groundedRight)
            travelRight = (-rightWheel.transform.InverseTransformPoint(hit.point).y - rightWheel.radius) / rightWheel.suspensionDistance;

        float antiRollForce = (travelLeft - travelRight) * 5000f;

        if (groundedLeft)
            rb.AddForceAtPosition(leftWheel.transform.up * -antiRollForce, leftWheel.transform.position);
        if (groundedRight)
            rb.AddForceAtPosition(rightWheel.transform.up * antiRollForce, rightWheel.transform.position);
    }


    private void UpdateDriftFriction()
    {
        float speed = rb.linearVelocity.magnitude;
        float turnAmount = Mathf.Abs(horizontalInput);

        bool drifting = speed > 6f && turnAmount > 0.5f;

        if (drifting)
        {
            targetForwardStiffness = 1.0f;
            targetSidewaysStiffness = 1.2f;
        }
        else
        {
            targetForwardStiffness = 2.0f;
            targetSidewaysStiffness = 2.5f;
        }
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
