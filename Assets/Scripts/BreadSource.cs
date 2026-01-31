using UnityEngine;

public class BreadSource : Interactable
{
    public GameObject breadPrefab;

    public override void Interact(GameObject player)
    {
        PlayerHold hold = player.GetComponent<PlayerHold>();

        if (hold.heldObject == null)
        {
            GameObject bread = Instantiate(breadPrefab);
            hold.Hold(bread);
        }
    }
}