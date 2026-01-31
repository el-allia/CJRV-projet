using UnityEngine;

public class PlayerHold : MonoBehaviour
{
    PlayerPickup pickup;

    // stations still expect this to exist
    public GameObject heldObject => pickup != null ? pickup.CurrentHeldObject() : null;

    void Awake()
    {
        pickup = GetComponent<PlayerPickup>();
    }

    public void Hold(GameObject obj)
    {
        if (pickup == null) return;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) rb = obj.AddComponent<Rigidbody>();

        obj.tag = "Pickup";

        pickup.ForceHold(obj);
    }

    public void Clear()
    {
        // nothing here
    }
}
