using UnityEngine;

public class InfiniteItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;

    public void ItemTaken()
    {
        Invoke(nameof(SpawnNew), 0.15f);
    }

    void SpawnNew()
    {
        Instantiate(itemPrefab, transform.position, transform.rotation);
    }
}
