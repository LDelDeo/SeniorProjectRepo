using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;
    Animator anim;
    Transform cam;

    float velocityY;
    Vector3 moveInput;
    Vector3 dir;

    [Header("Settings")]
    [SerializeField] float gravity = 25f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float rotateSpeed = 3f;
    [SerializeField] float jumpForce = 10f;

    public bool lockMovement;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        GetInput();
        Movement();
        if (!lockMovement) PlayerRotation();
    }

    private void GetInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //Use Raw for instant input

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        dir = (forward * moveInput.y + right * moveInput.x).normalized;
    }

    private void Movement()
    {
        if (controller.isGrounded)
        {
            velocityY = -1f; //Slight downward force to keep grounded

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocityY = jumpForce;
            }
        }
        else
        {
            velocityY -= gravity * Time.deltaTime;
        }

        Vector3 velocity = dir * moveSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);

        //Optional animator values
        //anim.SetFloat("Movement", dir.magnitude);
        //anim.SetFloat("Horizontal", moveInput.x);
        //anim.SetFloat("Vertical", moveInput.y);
    }

    private void PlayerRotation()
    {
        if (dir.magnitude == 0) return;

        Vector3 rotDir = new Vector3(dir.x, 0, dir.z); //Only rotate on the Y axis
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotDir), Time.deltaTime * rotateSpeed);
    }
}