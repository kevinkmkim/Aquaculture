using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1f;
    public float fastMoveMultiplier = 2f;
    public float verticalMoveSpeed = 10f;

    [Header("Rotation Settings")]
    public float mouseSensitivity = 100f;
    public bool invertY = false;
    public float maxLookAngle = 90f;

    [Header("Field of View Settings")]
    public float zoomSpeed = 2f;
    public float minFOV = 10f;
    public float maxFOV = 120f;

    [Header("Input Settings")]
    public KeyCode fastMoveKey = KeyCode.LeftShift;
    public KeyCode upKey = KeyCode.E;
    public KeyCode downKey = KeyCode.Q;
    public bool enableMouseLook = true;
    public bool lockCursor = true;

    private Camera cam;
    private float rotationX = 0f;
    private Vector3 lastMousePosition;
    private bool isMouseLookActive = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraController requires a Camera component!");
            enabled = false;
            return;
        }

        if (lockCursor && enableMouseLook)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isMouseLookActive = true;
        }

        Vector3 rot = transform.localRotation.eulerAngles;
        rotationX = rot.x;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleFieldOfView();
        HandleCursorToggle();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float upDown = 0f;

        if (Input.GetKey(upKey))
            upDown = 1f;
        else if (Input.GetKey(downKey))
            upDown = -1f;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = Vector3.up;

        forward.y = 0f;
        forward.Normalize();
        right.y = 0f;
        right.Normalize();

        Vector3 movement = (forward * vertical + right * horizontal + up * upDown).normalized;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(fastMoveKey))
            currentSpeed *= fastMoveMultiplier;

        transform.Translate(movement * currentSpeed * Time.deltaTime, Space.World);
    }

    void HandleRotation()
    {
        if (!enableMouseLook || !isMouseLookActive)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (invertY)
            mouseY = -mouseY;

        transform.Rotate(Vector3.up * mouseX, Space.World);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);

        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = rotationX;
        transform.eulerAngles = targetRotation;
    }

    void HandleFieldOfView()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            float newFOV = cam.fieldOfView - (scroll * zoomSpeed * 10f);
            cam.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);
        }

        if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus))
        {
            cam.fieldOfView = Mathf.Clamp(
                cam.fieldOfView - zoomSpeed * Time.deltaTime * 10f,
                minFOV,
                maxFOV
            );
        }
        else if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
        {
            cam.fieldOfView = Mathf.Clamp(
                cam.fieldOfView + zoomSpeed * Time.deltaTime * 10f,
                minFOV,
                maxFOV
            );
        }
    }

    void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isMouseLookActive = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isMouseLookActive = true;
            }
        }
    }

    void OnValidate()
    {
        moveSpeed = Mathf.Max(0.1f, moveSpeed);
        fastMoveMultiplier = Mathf.Max(1f, fastMoveMultiplier);
        mouseSensitivity = Mathf.Max(0.1f, mouseSensitivity);
        maxLookAngle = Mathf.Clamp(maxLookAngle, 0f, 90f);
        zoomSpeed = Mathf.Max(0.1f, zoomSpeed);
        minFOV = Mathf.Clamp(minFOV, 1f, 179f);
        maxFOV = Mathf.Clamp(maxFOV, minFOV + 1f, 179f);
    }
}
