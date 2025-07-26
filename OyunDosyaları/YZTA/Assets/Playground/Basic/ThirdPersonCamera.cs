using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -5);
    public float rotationSpeed = 0.1f;
    public float pitchMin = -30f;
    public float pitchMax = 60f;

    public Vector3 birdsEyeOffset = new Vector3(0, 15, 0);
    public float birdsEyePitch = 90f;
    public float birdsEyeYaw = 0f;

    public float transitionSpeed = 5f;

    private float yaw = 0f;
    private float pitch = 10f;
    public float floatingHeight = 12f;

    private GameObject objectToHover = null;
    private Vector2 lookInput = Vector2.zero;
    private PlayerInput playerInput;
    private InputAction rightClickAction;

    private bool birdsEyeActive = false;

    private Vector3 currentTargetPosition;
    private Quaternion currentTargetRotation;

    void Awake()
    {
        playerInput = Object.FindFirstObjectByType<PlayerInput>();
        if (playerInput == null)
            Debug.LogError("PlayerInput not found in scene");

        rightClickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/rightButton");
        rightClickAction.performed += ctx => OnRightClick();
    }

    void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Look"].performed += OnLook;
            playerInput.actions["Look"].canceled += OnLook;
        }
        rightClickAction.Enable();
    }

    void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions["Look"].performed -= OnLook;
            playerInput.actions["Look"].canceled -= OnLook;
        }
        rightClickAction.Disable();
    }

    void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void OnRightClick()
    {
        if (birdsEyeActive)
        {
            SetBirdsEyeMode(false, objectToHover);
        }
    }

    void Update()
    {
        if (birdsEyeActive)
        {
            yaw = birdsEyeYaw;
            pitch = birdsEyePitch;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
            int groundHitCount = 0;
            Vector3 lastGroundPoint = Vector3.zero;

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("ground"))
                {
                    groundHitCount++;
                    Debug.Log($"Ray hit ground #{groundHitCount} at: {hit.point}");
                    lastGroundPoint = hit.point;
                    if (groundHitCount == 2)
                    {
                        objectToHover.transform.position = Vector3.up * floatingHeight + hit.point;
                        break;
                    }
                }
            }

            // If only one ground hit, fallback to first
            if (groundHitCount == 1)
            {
                objectToHover.transform.position = Vector3.up * floatingHeight + lastGroundPoint;
            }
        }
        else
        {
            yaw += lookInput.x * rotationSpeed;
            pitch -= lookInput.y * rotationSpeed;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (birdsEyeActive)
        {
            // Kuşbakışı hedef pozisyon ve rotasyon
            currentTargetPosition = target.position + birdsEyeOffset;
            currentTargetRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredOffset = rotation * Vector3.back * offset.magnitude + Vector3.up * offset.y;
            currentTargetPosition = target.position + desiredOffset;
            currentTargetRotation = Quaternion.LookRotation(target.position + Vector3.up * 1.5f - currentTargetPosition);
        }

        // Smooth geçiş: Lerp ve Slerp
        transform.position = Vector3.Lerp(transform.position, currentTargetPosition, Time.deltaTime * transitionSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentTargetRotation, Time.deltaTime * transitionSpeed);
    }

    public void SetBirdsEyeMode(bool active, GameObject _object = null)
    {
        objectToHover = _object;
        birdsEyeActive = active;
        if (active && objectToHover != null)
        {
            objectToHover.transform.position = transform.position + Vector3.up * 100;
            objectToHover.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            lookInput = Vector2.zero;
            objectToHover.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
