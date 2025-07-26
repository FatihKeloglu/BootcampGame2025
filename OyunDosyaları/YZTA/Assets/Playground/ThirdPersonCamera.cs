using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -5);
    public float rotationSpeed = 0.1f;
    public float pitchMin = -30f;
    public float pitchMax = 60f;

    private float yaw = 0f;
    private float pitch = 10f;

    private Vector2 lookInput = Vector2.zero;
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = Object.FindFirstObjectByType<PlayerInput>();
        if (playerInput == null)
            Debug.LogError("PlayerInput not found in scene");
    }

    void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Look"].performed += OnLook;
            playerInput.actions["Look"].canceled += OnLook;
        }
    }

    void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Look"].performed -= OnLook;
            playerInput.actions["Look"].canceled -= OnLook;
        }
    }

    void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        yaw += lookInput.x * rotationSpeed;
        pitch -= lookInput.y * rotationSpeed;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredOffset = rotation * Vector3.back * offset.magnitude + Vector3.up * offset.y;
        Vector3 desiredPosition = target.position + desiredOffset;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
