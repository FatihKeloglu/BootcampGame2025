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
    public Transform cameraTransform;

    private Rigidbody rb;
    private InputHandler inputHandler;
    private MovementHandler movementHandler;
    private JumpHandler jumpHandler;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = new InputHandler(GetComponent<PlayerInput>());
        movementHandler = new MovementHandler(rb, () => inputHandler.MoveInput, () => inputHandler.RunPressed, walkSpeed, runSpeed, rotationSpeed);
        jumpHandler = new JumpHandler(rb, jumpForce, () => inputHandler.JumpPressed, () => IsGrounded());
    }

    void Start() => inputHandler.Bind();
    void OnDestroy() => inputHandler.Unbind();

    void FixedUpdate()
    {
        if (cameraTransform == null) return;

        movementHandler.Move(cameraTransform);
        jumpHandler.HandleJump();
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    // ------------------------------
    // Input Handler (move, jump, run)
    // ------------------------------
    private class InputHandler
    {
        private readonly PlayerInput playerInput;

        public Vector2 MoveInput { get; private set; }
        public bool JumpPressed { get; private set; } = false;
        public bool RunPressed { get; private set; } = false;

        public InputHandler(PlayerInput playerInput)
        {
            this.playerInput = playerInput;
        }

        public void Bind()
        {
            playerInput.actions["Move"].performed += OnMove;
            playerInput.actions["Move"].canceled += OnMove;

            playerInput.actions["Jump"].performed += ctx => JumpPressed = true;

            playerInput.actions["Run"].performed += ctx => RunPressed = true;
            playerInput.actions["Run"].canceled += ctx => RunPressed = false;
        }

        public void Unbind()
        {
            playerInput.actions["Move"].performed -= OnMove;
            playerInput.actions["Move"].canceled -= OnMove;

            playerInput.actions["Jump"].performed -= ctx => JumpPressed = true;

            playerInput.actions["Run"].performed -= ctx => RunPressed = true;
            playerInput.actions["Run"].canceled -= ctx => RunPressed = false;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }
    }

    // ------------------------------
    // Movement Handler (walk/run/rotate)
    // ------------------------------
    private class MovementHandler
    {
        private readonly Rigidbody rb;
        private readonly System.Func<Vector2> moveInputProvider;
        private readonly System.Func<bool> isRunningProvider;
        private readonly float walkSpeed, runSpeed, rotationSpeed;

        public MovementHandler(Rigidbody rb, System.Func<Vector2> moveInput, System.Func<bool> run, float walkSpeed, float runSpeed, float rotationSpeed)
        {
            this.rb = rb;
            this.moveInputProvider = moveInput;
            this.isRunningProvider = run;
            this.walkSpeed = walkSpeed;
            this.runSpeed = runSpeed;
            this.rotationSpeed = rotationSpeed;
        }

        public void Move(Transform cameraTransform)
        {
            float currentSpeed = isRunningProvider() ? runSpeed : walkSpeed;
            Vector2 input = moveInputProvider();

            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = camRight * input.x + camForward * input.y;
            moveDir.Normalize();

            if (moveDir.sqrMagnitude > 0.01f)
            {
                // Hız uygula
                rb.AddForce(moveDir * currentSpeed * 10f, ForceMode.Acceleration);

                // Maksimum hız sınırı
                Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                float inputSpeedInMoveDir = Vector3.Dot(horizontalVelocity, moveDir);
                if (inputSpeedInMoveDir > currentSpeed)
                {
                    Vector3 excessVelocity = moveDir * (inputSpeedInMoveDir - currentSpeed);
                    rb.linearVelocity -= new Vector3(excessVelocity.x, 0, excessVelocity.z);
                }

                // Rotasyon
                Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                rb.transform.rotation = Quaternion.RotateTowards(rb.transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }

    // ------------------------------
    // Jump Handler
    // ------------------------------
    private class JumpHandler
    {
        private readonly Rigidbody rb;
        private readonly float jumpForce;
        private readonly System.Func<bool> isJumpPressed;
        private readonly System.Func<bool> isGrounded;

        public JumpHandler(Rigidbody rb, float jumpForce, System.Func<bool> jumpPressed, System.Func<bool> grounded)
        {
            this.rb = rb;
            this.jumpForce = jumpForce;
            this.isJumpPressed = jumpPressed;
            this.isGrounded = grounded;
        }

        public void HandleJump()
        {
            if (isJumpPressed() && isGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}
