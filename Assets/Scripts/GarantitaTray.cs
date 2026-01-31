using UnityEngine;

public class GarantitaTray : Interactable
{
    public override void Interact(GameObject player)
    {
        PlayerHold hold = player.GetComponent<PlayerHold>();

        if (hold.heldObject != null && hold.heldObject.CompareTag("CutBread"))
        {
            hold.heldObject.GetComponent<CutBread>().AddSlice();
        }
    }
}