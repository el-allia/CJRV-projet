using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 8f;
    public float mouseSensitivity = 4f;
    public Transform cameraHolder;

    CharacterController controller;
    float verticalLookRotation;
    Vector3 moveInput;

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
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -80f, 80f);
        cameraHolder.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);

        // ===== MOVEMENT =====
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.Z)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.Q)) x = -1f; // AZERTY FIX

        moveInput = (transform.forward * z + transform.right * x).normalized;

        controller.Move(moveInput * speed * Time.deltaTime);
    }
}
