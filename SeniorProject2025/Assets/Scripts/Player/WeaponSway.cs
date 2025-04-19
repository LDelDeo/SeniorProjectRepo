using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Values")]
    public float swayAmount = 2f;
    public float swaySpeed = 4f;
    public float sprintSwayMultiplier = 2.5f;
    [Header("Booleans")]
    public bool onlyWhenMoving = true;

    private Quaternion initialRotation;
    private FPController player;
    

    void Start()
    {
        initialRotation = transform.localRotation;
        player = FindObjectOfType<FPController>();
    }

    void Update()
    {
        if (player == null) return;

        bool isMoving = player.moveDirection.magnitude > 0.1f && player.isGrounded;
        float currentSwaySpeed = player.isSprinting ? swaySpeed * sprintSwayMultiplier : swaySpeed;

        if (onlyWhenMoving && !isMoving)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, initialRotation, Time.deltaTime * currentSwaySpeed);
            return;
        }

        float swayX = Mathf.Sin(Time.time * currentSwaySpeed) * swayAmount;
        float swayY = Mathf.Cos(Time.time * currentSwaySpeed * 0.5f) * swayAmount * 0.5f;

        Quaternion targetRotation = initialRotation * Quaternion.Euler(swayY, swayX, 0f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * currentSwaySpeed);
    }
}
