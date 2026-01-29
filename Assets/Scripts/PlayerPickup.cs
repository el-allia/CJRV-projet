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
        if (heldObject != null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null && hit.collider.CompareTag("Pickup"))
            {
                heldObject = rb.gameObject;
                heldRb = rb;

                heldRb.isKinematic = true;
                heldRb.useGravity = false;

                heldObject.transform.SetParent(holdPoint);
                heldObject.transform.localPosition = Vector3.zero;
            }
        }
    }


    void Drop()
{
    if (heldObject == null || heldRb == null)
        return;

    heldObject.transform.SetParent(null);


    heldRb.isKinematic = false;
    heldRb.useGravity = true;


    heldRb.linearVelocity = Vector3.zero;
    heldRb.angularVelocity = Vector3.zero;

    heldObject = null;
    heldRb = null;
}

}
