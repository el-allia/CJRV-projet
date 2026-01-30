using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TruckQueue : MonoBehaviour
{
    [Header("Queue points")]
    public Transform frontPoint;   // TruckTarget (where the first person stands)
    public Transform exitPoint;    // ExitTarget (where served person goes)

    [Header("Queue settings")]
    public float spacing = 1.2f;   // distance between people
    public int maxCount = 30;

    private readonly List<ClientQueueMember> members = new();

    public void Join(ClientQueueMember m)
    {
        if (m == null) return;
        if (members.Contains(m)) return;

        if (members.Count >= maxCount)
        {
            // Queue is full: you can destroy client or send them away
            // Destroy(m.gameObject);
            return;
        }

        members.Add(m);
        RecomputeTargets();
    }

    public void Leave(ClientQueueMember m)
    {
        if (m == null) return;
        if (members.Remove(m))
            RecomputeTargets();
    }

    public void ServeFront()
    {
        if (members.Count == 0) return;

        var first = members[0];
        Leave(first); // removes them and makes everyone move up

        if (exitPoint != null)
            first.GoToExit();

    }

    private void RecomputeTargets()
    {
        if (frontPoint == null) return;

        Vector3 dirBack = -frontPoint.forward; // line goes behind the truck

        for (int i = 0; i < members.Count; i++)
        {
            Vector3 rawPos = frontPoint.position + dirBack * spacing * i;

            // Snap each spot onto the NavMesh so agents don't get stuck
            if (NavMesh.SamplePosition(rawPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                members[i].SetQueueDestination(hit.position);
            else
                members[i].SetQueueDestination(rawPos);
        }
    }

    // Optional: quick test key (press Space to serve first client)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ServeFront();
    }
}
