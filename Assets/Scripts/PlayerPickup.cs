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

    void PickObject(GameObject obj)
{
    heldObject = obj;
    heldRb = heldObject.GetComponent<Rigidbody>();

    heldRb.isKinematic = true;
    heldRb.useGravity = false;

    heldRb.linearVelocity = Vector3.zero;
    heldRb.angularVelocity = Vector3.zero;

    heldObject.transform.SetParent(holdPoint);
    heldObject.transform.localPosition = Vector3.zero;
}

    void TryPickup()
{
    if (heldObject != null) return;

    Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, pickupDistance))
    {
        // CASE 1: Ingredient dispenser (tomato plate)
        IngredientDispenser dispenser = hit.collider.GetComponent<IngredientDispenser>();
        if (dispenser != null)
        {
            GameObject newItem = Instantiate(
                dispenser.ingredientPrefab,
                dispenser.spawnPoint.position,
                dispenser.spawnPoint.rotation
            );

            PickObject(newItem);
            return;
        }

        // CASE 2: Normal pickup
        if (hit.collider.CompareTag("Pickup"))
        {
            PickObject(hit.collider.gameObject);
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
