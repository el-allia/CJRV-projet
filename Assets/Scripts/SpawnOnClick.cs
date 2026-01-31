using UnityEngine;

public class SpawnOnClick : MonoBehaviour
{
    public GameObject breadPrefab;
    public Transform spawnPoint;

    void OnMouseDown()
    {
        GameObject bread = Instantiate(breadPrefab, spawnPoint.position, spawnPoint.rotation);

        AssemblyState.breadReady = true;
        AssemblyState.currentBread = bread;
    }
}