using UnityEngine;

public class GarantitaClickSpawner : MonoBehaviour
{
    public GameObject sandwichPrefab;

    void OnMouseDown()
    {
        PlayerPickup player = FindObjectOfType<PlayerPickup>();
        if (player == null) return;

        GameObject sandwich = Instantiate(sandwichPrefab);
        player.ForceHold(sandwich);
    }
}