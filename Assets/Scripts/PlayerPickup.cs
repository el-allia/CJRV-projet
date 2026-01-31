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
        playerCol = GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            HandleE();

        if (Input.GetKeyDown(KeyCode.R))
            TryAssemble();

        if (Input.GetKeyDown(KeyCode.F))
            Drop();
    }

    // ---------- E KEY ----------
    void HandleE()
    {
        if (heldObject != null)
        {
            // Flip patty
            PattyCook pc = heldObject.GetComponent<PattyCook>();
            if (pc != null)
            {
                pc.Flip();
                return;
            }
        }

        // Otherwise pickup
        TryPickup();
    }

    // ---------- PICKUP ----------
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

                // Raw patty infinite spawn
                if (heldObject.CompareTag("RawMeat"))
                {
                    RawPattySpawner spawner = FindObjectOfType<RawPattySpawner>();
                    if (spawner != null)
                        spawner.PattyTaken();
                }

                heldRb = rb;
                heldCol = heldObject.GetComponent<Collider>();

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

    // ---------- R KEY (ASSEMBLE) ----------
    void TryAssemble()
    {
        if (heldObject == null) return;
        if (!heldObject.CompareTag("Sliceable")) return;

        BreadAssembly[] breads = FindObjectsOfType<BreadAssembly>();

        foreach (var bread in breads)
        {
            float dist = Vector3.Distance(transform.position, bread.transform.position);

            if (dist < 2f)
            {
                bread.Fill(heldObject);

                heldObject = null;
                heldRb = null;
                heldCol = null;

                if (handVisualRoot != null)
                    handVisualRoot.SetActive(false);

                return;
            }
        }
    }

    // ---------- DROP ----------
    void Drop()
    {
        if (heldObject == null || heldRb == null) return;
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        heldObject.transform.SetParent(null, true);

        Vector3 dropPos;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, dropRayDistance))
            dropPos = hit.point + hit.normal * surfaceOffset;
        else
            dropPos = playerCamera.transform.position + playerCamera.transform.forward * 1.2f;

        heldObject.transform.position = dropPos;

        yield return null;

        heldRb.isKinematic = false;
        heldRb.useGravity = true;
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;

        var freezer = heldObject.GetComponent<FreezeOnFirstCollision>();
        if (freezer == null) freezer = heldObject.AddComponent<FreezeOnFirstCollision>();
        freezer.Arm();

        yield return new WaitForSeconds(0.15f);
        if (heldCol != null && playerCol != null)
            Physics.IgnoreCollision(heldCol, playerCol, false);

        heldObject = null;
        heldRb = null;
        heldCol = null;

        if (handVisualRoot != null)
            handVisualRoot.SetActive(false);
    }

    bool IsPickupTag(string tag)
    {
        return tag == "Pickup" ||
               tag == "Bread" ||
               tag == "CutBread" ||
               tag == "RawMeat" ||
               tag == "CookedMeat" ||
               tag == "FilledCup" ||
               tag == "EmptyCup" ||
               tag == "Garantita";
    }

    public GameObject CurrentHeldObject()
    {
        return heldObject;
    }
    public void ForceHold(GameObject obj)
{
    if (heldObject != null) return;

    heldObject = obj;
    heldRb = obj.GetComponent<Rigidbody>();
    heldCol = obj.GetComponent<Collider>();

    if (heldRb == null) heldRb = obj.AddComponent<Rigidbody>();
    if (heldCol == null) heldCol = obj.AddComponent<BoxCollider>();

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
}