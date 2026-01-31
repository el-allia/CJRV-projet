using UnityEngine;

public class BreadAssembly : MonoBehaviour
{
    public Transform fillingSnapPoint;
    private bool filled = false;

    private bool playerNear = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }

    public bool CanFill(GameObject held)
    {
        return playerNear && !filled && held.CompareTag("Sliceable");
    }

    public void Fill(GameObject held)
    {
        filled = true;

        Rigidbody rb = held.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        held.transform.SetParent(transform);
        held.transform.position = fillingSnapPoint.position;
        held.transform.rotation = fillingSnapPoint.rotation;

        gameObject.tag = "ReadyMeal";

        Debug.Log("Meal Ready!");
    }
}