using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private Transform head;

    [SerializeField]
    private Transform body;

    [SerializeField]
    private float mouseSensitivity = 10.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Update()
    {
        Debug.Assert(head != null);
        Debug.Assert(body != null);

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        yaw += mouseDelta.x * Time.deltaTime * mouseSensitivity;
        pitch -= mouseDelta.y * Time.deltaTime * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -89.9f, 89.9f);
     
        body.transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
        head.transform.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);
    }
}
