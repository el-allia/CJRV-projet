using System.Collections.Generic;
using UnityEngine;

public class TruckQueue : MonoBehaviour
{
    [SerializeField] private List<Transform> queuePoints = new List<Transform>();

    private readonly List<ClientQueueMember> members = new List<ClientQueueMember>();

    public void Join(ClientQueueMember m)
    {
        if (!members.Contains(m))
        {
            members.Add(m);
            Rebuild();
        }
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
        for (int i = 0; i < members.Count; i++)
        {
            if (members[i] != null && i < queuePoints.Count)
            {
                members[i].SetQueueDestination(queuePoints[i].position);
            }
        }
    }
}
