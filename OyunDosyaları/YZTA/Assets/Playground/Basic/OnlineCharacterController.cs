using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class OnlineCharacterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 7f;
    public float jumpForce = 5f;
    public float rotationSpeed = 720f;

    private Rigidbody rb;
    private PlayerInput playerInput;
    public Transform cameraTransform;

    private Vector2 moveInput;
    private bool jumpPressed = false;
    private bool runPressed = false;

    void Awake()
    {
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        if (playerInput != null)
        {
            playerInput.actions["Jump"].performed += OnJump;
            playerInput.actions["Run"].performed += OnRunPerformed;
            playerInput.actions["Run"].canceled += OnRunCanceled;
            playerInput.actions["Move"].performed += OnMove;
            playerInput.actions["Move"].canceled += OnMove;
        }
    }

    void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.actions["Jump"].performed -= OnJump;
            playerInput.actions["Run"].performed -= OnRunPerformed;
            playerInput.actions["Run"].canceled -= OnRunCanceled;
            playerInput.actions["Move"].performed -= OnMove;
            playerInput.actions["Move"].canceled -= OnMove;
        }
    }

    void OnJump(InputAction.CallbackContext context) => jumpPressed = true;
    void OnRunPerformed(InputAction.CallbackContext context) => runPressed = true;
    void OnRunCanceled(InputAction.CallbackContext context) => runPressed = false;
    void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    void FixedUpdate()
    {
        float currentSpeed = runPressed ? runSpeed : walkSpeed;

        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camRight * moveInput.x + camForward * moveInput.y;
            moveDirection.Normalize();

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                // Kuvvet uygula
                Vector3 force = moveDirection * currentSpeed * 10f;
                rb.AddForce(force, ForceMode.Acceleration);

                // Input yönündeki hız bileşenini sınırlıyoruz
                Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                float inputSpeedInMoveDir = Vector3.Dot(horizontalVelocity, moveDirection);

                if (inputSpeedInMoveDir > currentSpeed)
                {
                    Vector3 excessVelocity = moveDirection * (inputSpeedInMoveDir - currentSpeed);
                    rb.linearVelocity -= new Vector3(excessVelocity.x, 0, excessVelocity.z);
                }

                // Karakter rotasyonu
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }

        if (jumpPressed && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
