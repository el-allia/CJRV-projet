using UnityEngine;

public class SpawnOnClick : MonoBehaviour
{
    public GameObject cutBreadPrefab;
    public Transform spawnPoint; // the cutting board

    void OnMouseDown()
    {
        Instantiate(cutBreadPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}