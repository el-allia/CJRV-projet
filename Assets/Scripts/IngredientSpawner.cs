using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject ingredientPrefab;
    public Transform spawnPoint;

    public GameObject Spawn()
    {
        return Instantiate(ingredientPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
