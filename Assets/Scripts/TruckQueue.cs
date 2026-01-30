using System.Collections.Generic;
using UnityEngine;

public class TruckQueue : MonoBehaviour
{
    [Header("Queue shape")]
    [SerializeField] private Transform frontPoint;      // where the first customer stands
    [SerializeField] private Vector3 backDirection = Vector3.back; // local direction
    [SerializeField] private float spacing = 1.2f;      // distance between customers
    [SerializeField] private int maxQueuePositions = 20;

    private readonly List<ClientQueueMember> members = new();

    private void Reset()
    {
        // sensible defaults
        backDirection = Vector3.back;
        spacing = 1.2f;
        maxQueuePositions = 20;
    }

    public void Join(ClientQueueMember m)
    {
        if (m == null) return;
        if (members.Contains(m)) return;

        members.Add(m);
        Rebuild();
    }

    public void Leave(ClientQueueMember m)
    {
        if (members.Remove(m))
            Rebuild();
    }

    public bool IsFront(ClientQueueMember m)
    {
        return members.Count > 0 && members[0] == m;
    }

    private void Rebuild()
    {
        if (frontPoint == null) return;

        int count = Mathf.Min(members.Count, maxQueuePositions);

        for (int i = 0; i < count; i++)
        {
            var mem = members[i];
            if (mem == null) continue;

            // queue position: frontPoint + (direction * spacing * index)
            Vector3 dirWorld = frontPoint.TransformDirection(backDirection).normalized;
            Vector3 targetPos = frontPoint.position + dirWorld * spacing * i;

            mem.SetQueueDestination(targetPos);
        }
    }
}
