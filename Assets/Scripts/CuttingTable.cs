using UnityEngine;

public class CuttingTable : Interactable
{
    public GameObject cutBreadPrefab;

    public override void Interact(GameObject player)
    {
        PlayerHold hold = player.GetComponent<PlayerHold>();

        if (hold.heldObject != null && hold.heldObject.CompareTag("Bread"))
        {
            Destroy(hold.heldObject);
            hold.Clear();

            GameObject cutBread = Instantiate(cutBreadPrefab);
            hold.Hold(cutBread);
        }
    }
}