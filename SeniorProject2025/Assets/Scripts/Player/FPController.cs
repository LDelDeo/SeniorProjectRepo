using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("Script Grabs")]
    public FPShooting fpShooting;


    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;
    public float gravityMultiplier = 1.5f;
    public float jumpHeight = 1.5f;
    public bool isMoving;
    public bool isSprinting = false;


    [Header("Camera Settings")]
    public Transform cameraTransform;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Bobbing")]
    public float bobAmount = 0.05f;
    public float bobSpeed = 6f;
    public float currentBobSpeed = 6f;
    public float sprintBobSpeed = 8f;


    private float bobTimer = 0f;
    private Vector3 cameraStartPos;

    [Header("Side Sway")]
    public float swayAmount = 0.05f;
    public float swaySpeed = 6f;
    public float currentSwaySpeed = 6f;
    public float sprintSwaySpeed = 8f;

    public CharacterController controller;
    private float verticalVelocity;
    private float xRotation = 0f;
    public bool isGrounded;

    private Vector3 lastPosition;
    public Vector3 moveDirection;

    void Start()
    {
        fpShooting = FindObjectOfType<FPShooting>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (cameraTransform != null)
            cameraStartPos = cameraTransform.localPosition;

        lastPosition = transform.position;
    }

    void Update()
    {
        GroundCheck();
        HandleMouseLook();
        HandleMovement();
        HandleCameraEffects();
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = transform.right * moveX + transform.forward * moveZ;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftShift) && moveDirection.magnitude > 0.1f && !isSprinting)
        {
            controller.Move(moveDirection * sprintSpeed * Time.deltaTime);
            isSprinting = true;
            currentBobSpeed = sprintBobSpeed;
            currentSwaySpeed = sprintSwaySpeed;
            if (!fpShooting.gunAnim.GetCurrentAnimatorStateInfo(0).IsName("Sprinting"))
            {
                fpShooting.gunAnim.SetBool("IsSprintingBool", true);
            }

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || moveDirection.magnitude <= 0.1f)
        {
            isSprinting = false;
            currentBobSpeed = bobSpeed;
            currentSwaySpeed = swaySpeed;
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            if (fpShooting.gunAnim.GetCurrentAnimatorStateInfo(0).IsName("SprintPose") || fpShooting.gunAnim.GetCurrentAnimatorStateInfo(0).IsName("Sprinting"))
            {
                fpShooting.gunAnim.SetBool("IsSprintingBool", false);
            }
        }
       
        

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += (gravity * gravityMultiplier) * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    } 
   

    void HandleCameraEffects()
    {
        Vector3 targetPosition = cameraStartPos;

        // Only do effects if we're moving and grounded
        isMoving = moveDirection.magnitude > 0.1f && isGrounded;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * currentBobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmount;

            float swayOffset = Mathf.Sin(bobTimer) * swayAmount;

            targetPosition += new Vector3(swayOffset, bobOffset, 0f);
        }
        else
        {
            //bobTimer = 0f; // reset for consistent sin wave
        }

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * currentSwaySpeed);
    }
}
