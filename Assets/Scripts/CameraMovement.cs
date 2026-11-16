using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private float moveSpeed = 50f;
    private float minFov = 15f;
    private float maxFov = 90f;
    private float zoomSensitivity = 100f;
    private float lookSensitivity = 0.2f;

    private Vector3 start = new Vector3(-20, 10, 0);
    private Vector3 lookAt = new Vector3(15f, 1.5f, 15f);

    private bool freeMovement = true;

    private Camera cam;

    private float horizontalInput = 0f;
    private float verticalInput = 0f;
    private float zoomValue = 0f;

    private bool isRightMouseDown = false;
    private Vector2 lookInput = Vector2.zero;
    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        this.cam = GetComponent<Camera>();

        this.transform.position = start;
        this.transform.LookAt(lookAt);

        Vector3 initialAngles = this.transform.eulerAngles;
        this.yaw = initialAngles.y;
        this.pitch = initialAngles.x;
    }

    void Update()
    {
        if (this.freeMovement)
        {
            if (this.isRightMouseDown)
            {
                this.yaw += this.lookInput.x * this.lookSensitivity;
                this.pitch -= this.lookInput.y * this.lookSensitivity;

                this.pitch = Mathf.Clamp(this.pitch, -89f, 89f);

                this.transform.eulerAngles = new Vector3(this.pitch, this.yaw, 0f);
            }

            Vector3 moveDir = (this.transform.forward * this.verticalInput + this.transform.right * this.horizontalInput).normalized;
            this.transform.position += moveDir * this.moveSpeed * Time.deltaTime;
        }
        else
        {
            if (this.horizontalInput != 0f)
            {
                this.transform.RotateAround(this.lookAt, Vector3.up, this.horizontalInput * this.moveSpeed * Time.deltaTime);
                this.transform.LookAt(this.lookAt);
            }

            if (this.cam != null && this.zoomValue != 0f)
            {
                this.cam.fieldOfView -= this.zoomValue * this.zoomSensitivity * Time.deltaTime;
                this.cam.fieldOfView = Mathf.Clamp(this.cam.fieldOfView, this.minFov, this.maxFov);
            }
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.isRightMouseDown = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (context.canceled)
        {
            this.isRightMouseDown = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        this.lookInput = context.ReadValue<Vector2>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;

        if (actionName == "Left")
        {
            if (context.performed) this.horizontalInput = this.freeMovement ? -1f : 1f;
            else if (context.canceled) this.horizontalInput = 0f;
        }
        else if (actionName == "Right")
        {
            if (context.performed) this.horizontalInput = this.freeMovement ? 1f : -1f;
            else if (context.canceled) this.horizontalInput = 0f;
        }
        else if (actionName == "Forward")
        {
            if (context.performed) this.verticalInput = 1f;
            else if (context.canceled) this.verticalInput = 0f;
        }
        else if (actionName == "Backward")
        {
            if (context.performed) this.verticalInput = -1f;
            else if (context.canceled) this.verticalInput = 0f;
        }
    }

    public void Zoom(InputAction.CallbackContext context)
    {
        this.zoomValue = context.ReadValue<Vector2>().y;

        if (context.canceled)
        {
            this.zoomValue = 0f;
        }
    }

    public void SetFreeMovement(bool freeMovement)
    {
        this.freeMovement = freeMovement;
        if (freeMovement)
        {
            Vector3 currentAngles = this.transform.eulerAngles;
            this.yaw = currentAngles.y;
            this.pitch = currentAngles.x;
        } else
        {
            this.transform.LookAt(this.lookAt);
        }
    }

    public bool IsFreeMovement()
    {
        return this.freeMovement;
    }
}
