using UnityEngine;

public class RawPattySpawner : MonoBehaviour
{
    public GameObject rawPattyPrefab;
    private GameObject currentPatty;

    void Start()
    {
        SpawnNew();
    }

    public void PattyTaken()
    {
        Invoke(nameof(SpawnNew), 0.2f);
    }

    void SpawnNew()
    {
        currentPatty = Instantiate(rawPattyPrefab, transform.position, transform.rotation);
    }
}
