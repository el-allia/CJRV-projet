using UnityEngine;
using System.Collections;

public class PlayerPickup : MonoBehaviour
{
    public Camera playerCamera;
    public float pickupDistance = 3f;
    public Transform holdPoint;

    [Header("Optional: hand visuals")]
    public GameObject handVisualRoot;

    [Header("Drop")]
    public float dropRayDistance = 3f;
    public float surfaceOffset = 0.03f;

    GameObject heldObject;
    Rigidbody heldRb;
    Collider heldCol;
    Collider playerCol;

    void Awake()
    {
        playerCol = GetComponent<Collider>(); // your player capsule collider
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) TryPickup();
        if (Input.GetKeyDown(KeyCode.F)) Drop();
    }

    void TryPickup()
    {
        if (heldObject != null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null && IsPickupTag(hit.collider.tag))

            {
                heldObject = rb.gameObject;
                heldRb = rb;

                heldCol = heldObject.GetComponent<Collider>();

                // Ignore player collision while holding
                if (heldCol != null && playerCol != null)
                    Physics.IgnoreCollision(heldCol, playerCol, true);

                heldRb.isKinematic = true;
                heldRb.useGravity = false;

                heldObject.transform.SetParent(holdPoint, true);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject.transform.localRotation = Quaternion.identity;

                if (handVisualRoot != null)
                    handVisualRoot.SetActive(true);
            }
        }
    }
    bool IsPickupTag(string tag)
{
    return tag == "Pickup" ||
           tag == "Bread" ||
           tag == "CutBread" ||
           tag == "RawMeat" ||
           tag == "CookedMeat" ||
           tag == "FilledCup" ||
           tag == "EmptyCup";
}

public void ForceHold(GameObject obj)
{
    if (heldObject != null) return;

    heldObject = obj;
    heldRb = obj.GetComponent<Rigidbody>();
    heldCol = obj.GetComponent<Collider>();

    if (heldRb == null) heldRb = obj.AddComponent<Rigidbody>();
    if (heldCol == null) heldCol = obj.AddComponent<BoxCollider>();

    // Ignore player collision
    if (heldCol != null && playerCol != null)
        Physics.IgnoreCollision(heldCol, playerCol, true);

    heldRb.isKinematic = true;
    heldRb.useGravity = false;

    obj.transform.SetParent(holdPoint, true);
    obj.transform.localPosition = Vector3.zero;
    obj.transform.localRotation = Quaternion.identity;

    if (handVisualRoot != null)
        handVisualRoot.SetActive(true);
}
public GameObject CurrentHeldObject()
{
    return heldObject;
}

    void Drop()
    {
        if (heldObject == null || heldRb == null) return;
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        // Detach
        heldObject.transform.SetParent(null, true);

        // Choose a drop position where you are looking (counter/floor)
        Vector3 dropPos;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, dropRayDistance))
        {
            // place on the surface
            dropPos = hit.point + hit.normal * surfaceOffset;
        }
        else
        {
            // fallback: in front of camera
            dropPos = playerCamera.transform.position + playerCamera.transform.forward * 1.2f;
        }

        heldObject.transform.position = dropPos;

        // Wait 1 frame to fully detach from hand animation
        yield return null;

        // Enable physics briefly so we can detect the first collision
        heldRb.isKinematic = false;
        heldRb.useGravity = true;
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;

        // Arm "freeze on first collision"
        var freezer = heldObject.GetComponent<FreezeOnFirstCollision>();
        if (freezer == null) freezer = heldObject.AddComponent<FreezeOnFirstCollision>();
        freezer.Arm();

        // Re-enable collision with player AFTER a short moment
        // (prevents object exploding out of the player capsule)
        yield return new WaitForSeconds(0.15f);
        if (heldCol != null && playerCol != null)
            Physics.IgnoreCollision(heldCol, playerCol, false);

        heldObject = null;
        heldRb = null;
        heldCol = null;

        if (handVisualRoot != null)
            handVisualRoot.SetActive(false);
    }
}