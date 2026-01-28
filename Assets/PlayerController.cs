using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 8f;
    public float mouseSensitivity = 4f;

    public Transform cameraHolder;

    Rigidbody rb;
    float verticalLookRotation;
    Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // IMPORTANT : empêche vibration physique

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ===== SOURIS =====
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        // Rotation verticale caméra (haut / bas)
        verticalLookRotation -= mouseY;
        cameraHolder.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);

        // Rotation horizontale joueur (gauche / droite)
        transform.Rotate(Vector3.up * mouseX);

        // ===== CLAVIER =====
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.Z)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.A)) x = -1f;

        moveInput = (transform.forward * z + transform.right * x).normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }
}
