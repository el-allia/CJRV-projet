using UnityEngine;
using UnityEngine.AI;

public class ClientWalkToTruck : MonoBehaviour
{
    public Transform truckTarget;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Find target automatically if you forgot to assign it
        if (truckTarget == null)
        {
            GameObject t = GameObject.Find("TruckTarget");
            if (t != null) truckTarget = t.transform;
        }

        // Make sure we start ON the navmesh (super important for spawned characters)
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            transform.position = hit.position;

        if (truckTarget != null)
            agent.SetDestination(truckTarget.position);
    }
}
