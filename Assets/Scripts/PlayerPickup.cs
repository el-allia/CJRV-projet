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

    [Header("Debug")]
    public bool showDebugLogs = true;

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
        if (Input.GetKeyDown(KeyCode.E)) TryPickup();
        if (Input.GetKeyDown(KeyCode.F)) Drop();
    }

    void PickObject(GameObject obj)
    {
        heldObject = obj;
        heldRb = obj.GetComponent<Rigidbody>();
        heldCol = obj.GetComponent<Collider>();

        if (heldCol != null && playerCol != null)
            Physics.IgnoreCollision(heldCol, playerCol, true);

        heldRb.isKinematic = true;
        heldRb.useGravity = false;

        obj.transform.SetParent(holdPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        if (handVisualRoot != null)
            handVisualRoot.SetActive(true);

        if (showDebugLogs) Debug.Log($"✓ Picked up: {obj.name}");
    }

    void TryPickup()
    {
        if (heldObject != null)
        {
            if (showDebugLogs) Debug.Log("Already holding something!");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            if (showDebugLogs) Debug.Log($"Raycast hit: {hit.collider.gameObject.name}, tag: {hit.collider.tag}");

            // Check for ingredient spawners FIRST
            if (hit.collider.CompareTag("IngredientSource"))
            {
                IngredientSpawner spawner = hit.collider.GetComponent<IngredientSpawner>();
                if (spawner != null)
                {
                    GameObject newItem = spawner.Spawn();
                    if (showDebugLogs) Debug.Log($"Spawned: {newItem.name}");
                    PickObject(newItem);
                    return;
                }
            }

            // Then check for regular pickups
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null && hit.collider.CompareTag("Pickup"))
            {
                PickObject(rb.gameObject);
            }
        }
        else
        {
            if (showDebugLogs) Debug.Log("Raycast didn't hit anything");
        }
    }

    void Drop()
    {
        if (heldObject == null || heldRb == null)
        {
            if (showDebugLogs) Debug.Log("Nothing to drop!");
            return;
        }

        if (showDebugLogs) Debug.Log($"=== DROPPING {heldObject.name} ===");
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        if (heldObject == null) yield break;

        // CRITICAL: Temporarily disable held object's collider so raycast doesn't hit it!
        bool colliderWasEnabled = false;
        if (heldCol != null)
        {
            colliderWasEnabled = heldCol.enabled;
            heldCol.enabled = false; // Disable during raycast
            if (showDebugLogs) Debug.Log("Disabled held object collider for raycast");
        }

        // Raycast where player is looking
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Vector3 dropPos;

        if (showDebugLogs) Debug.Log($"Drop raycast from: {ray.origin}, direction: {ray.direction}");

        if (Physics.Raycast(ray, out RaycastHit hit, dropRayDistance))
        {
            if (showDebugLogs) Debug.Log($"Drop raycast HIT: {hit.collider.gameObject.name}, tag: {hit.collider.tag}");

            // DROPPED ON WORKTABLE?
            if (hit.collider.CompareTag("WorkTable"))
            {
                if (showDebugLogs) Debug.Log("✓ Hit WorkTable! Looking for BurgerStack...");

                BurgerStack burger = hit.collider.GetComponent<BurgerStack>();
                if (burger != null)
                {
                    if (showDebugLogs) Debug.Log("✓ Found BurgerStack! Calling AddIngredient...");

                    // Re-enable collider before adding to stack
                    if (heldCol != null)
                        heldCol.enabled = true;

                    // The BurgerStack will handle positioning
                    burger.AddIngredient(heldObject);

                    // Clear hand
                    heldObject = null;
                    heldRb = null;
                    heldCol = null;

                    if (handVisualRoot != null)
                        handVisualRoot.SetActive(false);

                    if (showDebugLogs) Debug.Log("✓ Ingredient added to burger!");

                    yield break;
                }
                else
                {
                    if (showDebugLogs) Debug.LogWarning("WorkTable has no BurgerStack component!");
                }
            }

            // DROPPED ON OTHER SURFACE
            if (showDebugLogs) Debug.Log($"Dropped on: {hit.collider.name} (not WorkTable)");

            float objectHeight = 0f;
            if (heldCol != null)
            {
                // Re-enable to get bounds
                heldCol.enabled = true;
                objectHeight = heldCol.bounds.size.y;
            }

            dropPos = hit.point + Vector3.up * (objectHeight / 2f + surfaceOffset);
        }
        else
        {
            if (showDebugLogs) Debug.Log("Drop raycast missed - using fallback position");
            
            // Re-enable collider
            if (heldCol != null)
                heldCol.enabled = true;

            dropPos = playerCamera.transform.position + playerCamera.transform.forward * 1.2f;
        }

        // Detach from hand
        heldObject.transform.SetParent(null, true);
        heldObject.transform.position = dropPos;

        yield return null;

        // Enable physics
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

        if (showDebugLogs) Debug.Log("Drop complete.");
    }
}