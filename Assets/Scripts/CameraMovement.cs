using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private float moveSpeed = 50f;
    public float minFov = 15f;
    public float maxFov = 90f;
    public float zoomSensitivity = 10f;

    public Vector3 start = new Vector3(-20, 10, 0);
    public Vector3 lookAt = new Vector3(0, 0, 0);
    public float radius = 10;
    private float moveDirection = 0f;
    void Start()
    {
        transform.position = start;

    }

    private void Update()
    {
        if(moveDirection != 0f)
        {
            transform.RotateAround(lookAt, Vector3.up, moveDirection * moveSpeed * Time.deltaTime);
            transform.LookAt(lookAt);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        string actionName = context.action.name;

        if (actionName == "Left")
        {
            if (context.performed) moveDirection = 1f;
            else if (context.canceled) moveDirection = 0f;
        }
        else if (actionName == "Right")
        {
            if (context.performed) moveDirection = -1f;
            else if (context.canceled) moveDirection = 0f;
        }
    }
    /*void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.RotateAround(lookAt, Vector3.up, -Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime + 0.0000001f);
        }
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float fov = Camera.main.fieldOfView;
            fov += -Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            Camera.main.fieldOfView = fov;
        }
        transform.LookAt(lookAt);
    }*/
}
