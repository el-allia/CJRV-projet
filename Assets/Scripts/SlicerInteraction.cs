using UnityEngine;

public class SlicerInteraction : MonoBehaviour
{
    public PlayerPickup playerPickup;
    public Camera playerCamera;

    public float interactDistance = 2f;
    public GameObject guarantitaPrefab;
    public GameObject sandwichPrefab;   // NEW

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TrySlice();
        }
    }

    void TrySlice()
    {
        GameObject heldObject = playerPickup.CurrentHeldObject();
        if (heldObject == null) return;

        if (heldObject.GetComponent<SlicerTool>() == null) return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Sliceable"))
            {
                HandleSliceOrAssemble(heldObject);
            }
        }
    }

    void HandleSliceOrAssemble(GameObject slicer)
    {
        // If bread is already placed on board → ASSEMBLE
        if (AssemblyState.breadReady && AssemblyState.currentBread != null)
        {
            // Remove bread
            Destroy(AssemblyState.currentBread);

            // Spawn sandwich directly in player hand
            GameObject sandwich = Instantiate(sandwichPrefab);
            playerPickup.ForceHold(sandwich);

            AssemblyState.breadReady = false;
            AssemblyState.currentBread = null;

            return;
        }

        // Otherwise just create normal slice on slicer
        SpawnOnSlicer(slicer);
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