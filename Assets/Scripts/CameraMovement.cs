using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float moveSpeed = 50f;
    public float minFov = 15f;
    public float maxFov = 90f;
    public float zoomSensitivity = 10f;

    public Vector3 start = new Vector3(-20, 10, 0);
    public Vector3 lookAt = new Vector3(0, 0, 0);
    public float radius = 10;
    void Start()
    {
        transform.position = start;

    }
    void Update()
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
    }
}
