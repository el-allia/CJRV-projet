using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float mouseSensitivity = 3f;
    public Transform cameraHolder;

    CharacterController controller;
    float verticalLookRotation;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ===== MOUSE LOOK =====
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

        // ===== MOVEMENT =====
        float x = Input.GetAxisRaw("Horizontal"); // Q D
        float z = Input.GetAxisRaw("Vertical");   // Z S

        Vector3 move = (transform.right * x + transform.forward * z).normalized;
        controller.Move(move * speed * Time.deltaTime);
    }
}
