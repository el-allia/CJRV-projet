using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Camera playerCamera;
    public float pickupDistance = 3f;
    public Transform holdPoint;

    GameObject heldObject;
    Rigidbody heldRb;

    void Update()
    {
        // PRENDRE
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }

        // LÂCHER
        if (Input.GetKeyDown(KeyCode.F))
        {
            Drop();
        }
    }

    void TryPickup()
    {
        if (heldObject != null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                heldObject = hit.collider.gameObject;
                heldRb = heldObject.GetComponent<Rigidbody>();

                heldRb.isKinematic = true;
                heldRb.useGravity = false;

                heldRb.linearVelocity = Vector3.zero;
                heldRb.angularVelocity = Vector3.zero;

                heldObject.transform.SetParent(holdPoint);
                heldObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    void Drop()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);
        heldRb.useGravity = true;

        heldObject = null;
        heldRb = null;
        heldRb.isKinematic = false;
        heldRb.useGravity = true;

    }
}
