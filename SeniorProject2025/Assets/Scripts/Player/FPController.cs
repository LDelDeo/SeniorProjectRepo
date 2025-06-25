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
    private bool hasJumped = false;
    private bool wasGrounded = false;


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

    [Header("Offsets")]
    public Vector3 bobAndSwayOffset = Vector3.zero;
    public Vector3 externalShakeOffset = Vector3.zero;

    void Start()
    {
        fpShooting = FindFirstObjectByType<FPShooting>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 2f);

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
        HandleSprint();

    }
    public void UpdateSensitivityFromPrefs()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 2f);
    }


    void GroundCheck()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && !wasGrounded)
        {
            hasJumped = false;
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
        float moveX = Input.GetAxisRaw("Horizontal"); 
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = new Vector3(moveX, 0f, moveZ).normalized;
        Vector3 worldDirection = transform.TransformDirection(inputDirection);

        moveDirection = worldDirection;

        verticalVelocity += gravity * gravityMultiplier * Time.deltaTime;

        // Jump
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f; 
            }

            if (Input.GetButtonDown("Jump") && !hasJumped)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                hasJumped = true;
            }
        }

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            hasJumped = false;

            if (moveX == 0 && moveZ == 0)
            {
                moveDirection = Vector3.zero;
            }
        }
    }


    void HandleCameraEffects()
    {
        Vector3 targetPosition = cameraStartPos;
        bobAndSwayOffset = Vector3.zero;

        isMoving = moveDirection.magnitude > 0.1f && isGrounded;

        if (isMoving)
        {
            bobTimer += Time.deltaTime * currentBobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmount;
            float swayOffset = Mathf.Sin(bobTimer) * swayAmount;

            bobAndSwayOffset = new Vector3(swayOffset, bobOffset, 0f);
        }

        // Final camera position = base + sway/bob + shake
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            cameraStartPos + bobAndSwayOffset + externalShakeOffset,
            Time.deltaTime * currentSwaySpeed
        );
    }

    void HandleSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting)
        {
            isSprinting = true;
            currentBobSpeed = sprintBobSpeed;
            currentSwaySpeed = sprintSwaySpeed;

            moveSpeed = sprintSpeed;

            if (!fpShooting.gunAnim.GetCurrentAnimatorStateInfo(0).IsName("Sprinting"))
            {
                fpShooting.gunAnim.SetBool("IsSprintingBool", true);
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && isSprinting)
        {
            isSprinting = false;
            currentBobSpeed = bobSpeed;
            currentSwaySpeed = swaySpeed;

            moveSpeed = 5f; 

            if (fpShooting.gunAnim.GetCurrentAnimatorStateInfo(0).IsName("SprintPose") || fpShooting.gunAnim.GetCurrentAnimatorStateInfo(0).IsName("Sprinting"))
            {
                fpShooting.gunAnim.SetBool("IsSprintingBool", false);
            }
        }
        
    }
    void OnEnable()
    {
        if (this.GetComponent<CharacterController>() != null)
        {
            this.GetComponent<CharacterController>().enabled = true;
        }
    }
    void OnDisable()
    {
        if (this.GetComponent<CharacterController>() != null)
        {
            this.GetComponent<CharacterController>().enabled = false;
        }
    }
}
