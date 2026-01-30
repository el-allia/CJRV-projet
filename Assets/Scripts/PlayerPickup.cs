using UnityEngine;
using System.Collections;

public class PlayerPickup : MonoBehaviour
{
    public Camera playerCamera;
    public float pickupDistance = 3f;
    public Transform holdPoint;

    [Header("Optional: hand visuals")]
    public GameObject handVisualRoot;

    GameObject heldObject;
    Rigidbody heldRb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TryPickup();

        if (Input.GetKeyDown(KeyCode.F))
            Drop();
    }

    void TryPickup()
    {
        if (heldObject != null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null && hit.collider.CompareTag("Pickup"))
            {
                heldObject = rb.gameObject;
                heldRb = rb;

                // STOP physics FIRST
                heldRb.isKinematic = true;
                heldRb.useGravity = false;

                // Attach to animated hand
                heldObject.transform.SetParent(holdPoint, true);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject.transform.localRotation = Quaternion.identity;

                if (handVisualRoot != null)
                    handVisualRoot.SetActive(true);
            }
        }
    }

    void Drop()
    {
        if (heldObject == null) return;
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
{
    heldObject.transform.SetParent(null, true);

    Vector3 dropPos;

    // 1️⃣ Raycast where player looks
    Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

    if (Physics.Raycast(ray, out RaycastHit hit, 3f))
    {
        // Place object ON the surface hit (counter, table, floor, etc.)
        dropPos = hit.point + hit.normal * 0.05f;
    }
    else
    {
        // Fallback: floor in front
        dropPos = playerCamera.transform.position +
                  playerCamera.transform.forward * 1.2f;

        if (Physics.Raycast(dropPos + Vector3.up, Vector3.down, out RaycastHit floorHit, 2f))
        {
            dropPos = floorHit.point + Vector3.up * 0.05f;
        }
    }

    // 2️⃣ Move object BEFORE physics
    heldObject.transform.position = dropPos;

    yield return null;

    // 3️⃣ Enable physics safely
    heldRb.isKinematic = false;
    heldRb.useGravity = true;
    heldRb.linearVelocity = Vector3.zero;
    heldRb.angularVelocity = Vector3.zero;

    heldObject = null;
    heldRb = null;

    if (handVisualRoot != null)
        handVisualRoot.SetActive(false);
}

}