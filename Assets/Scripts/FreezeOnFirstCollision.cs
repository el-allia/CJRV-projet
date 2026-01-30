using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FreezeOnFirstCollision : MonoBehaviour
{
    Rigidbody rb;
    bool armed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enabled = false; // we only enable it when we drop
    }

    // Call this right when you drop
    public void Arm()
    {
        armed = true;
        enabled = true;
    }

    void OnCollisionEnter(Collision col)
    {
        if (!armed) return;

        // Freeze instantly on first contact
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.useGravity = false;

        // Optional: lock it forever (even if someone re-enables physics)
        rb.constraints = RigidbodyConstraints.FreezeAll;

        armed = false;
        enabled = false;
    }
}