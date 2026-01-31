using UnityEngine;

public class Grill : Interactable
{
    public GameObject cookedMeatPrefab;

    public override void Interact(GameObject player)
    {
        PlayerHold hold = player.GetComponent<PlayerHold>();

        if (hold.heldObject != null && hold.heldObject.CompareTag("RawMeat"))
        {
            Destroy(hold.heldObject);
            hold.Clear();

            GameObject meat = Instantiate(cookedMeatPrefab);
            hold.Hold(meat);
        }
    }
}