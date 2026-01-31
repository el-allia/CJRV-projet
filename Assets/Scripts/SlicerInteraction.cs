using UnityEngine;

public class SlicerInteraction : MonoBehaviour
{
    public PlayerPickup playerPickup;
    public Camera playerCamera;

    public float interactDistance = 2f;
    public GameObject guarantitaPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TrySlice();
        }
    }

    void TrySlice()
    {
        // 1. Vérifier qu'on tient un objet
        GameObject heldObject = playerPickup.CurrentHeldObject();
        if (heldObject == null) return;

        // 2. Vérifier que c'est le cheese-slicer
        if (heldObject.GetComponent<SlicerTool>() == null) return;

        // 3. Raycast depuis la caméra (FPS-style)
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            // 4. Vérifier que c'est sliceable
            if (hit.collider.CompareTag("Sliceable"))
            {
                SpawnOnSlicer(heldObject);
            }
        }
    }

    void SpawnOnSlicer(GameObject slicer)
    {
        Transform slicePoint = slicer.transform.Find("SlicePoint");
        if (slicePoint == null)
        {
            Debug.LogError("SlicePoint missing on slicer!");
            return;
        }

        GameObject slice = Instantiate(
            guarantitaPrefab,
            slicePoint.position,
            slicePoint.rotation
        );

        slice.transform.SetParent(slicePoint);
        slice.transform.localPosition = Vector3.zero;
        slice.transform.localRotation = Quaternion.identity;
    }
}
