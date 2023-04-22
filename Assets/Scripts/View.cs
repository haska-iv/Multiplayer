using UnityEngine;
using Mirror;

public class View : NetworkBehaviour
{
    private static View _active;
    public static View active { get { if (_active == null) _active = FindObjectOfType<View>(); return _active; } }

    public Camera cam;
    public Transform target;
    public float sensitivity = 5f;
    public float smoothTime = 0.1f;

    private Vector3 velocity;
    private float rotationX = 0f;
    private float rotationY = 0f;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        transform.position = this.target.position;
        velocity = Vector3.zero;
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);

        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);
        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
    }
}
