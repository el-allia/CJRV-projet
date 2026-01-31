using UnityEngine;

public class CoffeeMachine : Interactable
{
    public GameObject filledCupPrefab;

    public override void Interact(GameObject player)
    {
        PlayerHold hold = player.GetComponent<PlayerHold>();

        if (hold.heldObject != null && hold.heldObject.CompareTag("EmptyCup"))
        {
            Destroy(hold.heldObject);
            hold.Clear();

            GameObject cup = Instantiate(filledCupPrefab);
            hold.Hold(cup);
        }
    }
}