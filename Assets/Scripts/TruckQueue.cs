using System.Collections.Generic;
using UnityEngine;

public class TruckQueue : MonoBehaviour
{
    [Header("Queue points (Front -> Back)")]
    [SerializeField] private List<Transform> queuePoints = new List<Transform>();
    [SerializeField] private bool autoCollectChildrenIfEmpty = true;

    [Header("If more customers than points")]
    [SerializeField] private float overflowSpacing = 1.2f;
    [SerializeField] private Vector3 overflowDirectionLocal = Vector3.back; // local direction from last point

    private readonly List<ClientQueueMember> members = new List<ClientQueueMember>();

    private void Awake()
    {
        if (autoCollectChildrenIfEmpty && (queuePoints == null || queuePoints.Count == 0))
        {
            queuePoints = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
                queuePoints.Add(transform.GetChild(i));
        }
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
        if (m == null) return;
        if (members.Remove(m))
            Rebuild();
    }

    public ClientQueueMember GetFrontMember()
    {
        if (members.Count == 0) return null;
        return members[0];
    }

    public bool IsFront(ClientQueueMember m)
    {
        return members.Count > 0 && members[0] == m;
    }

    private Vector3 GetSlotPosition(int index)
    {
        if (queuePoints == null || queuePoints.Count == 0)
            return transform.position;

        if (index < queuePoints.Count)
            return queuePoints[index].position;

        // Overflow behind last point
        Transform last = queuePoints[queuePoints.Count - 1];
        Vector3 dirWorld = last.TransformDirection(overflowDirectionLocal).normalized;
        int extra = index - (queuePoints.Count - 1);
        return last.position + dirWorld * overflowSpacing * extra;
    }

    private void Rebuild()
    {
        for (int i = 0; i < members.Count; i++)
        {
            if (members[i] == null) continue;
            members[i].SetQueueDestination(GetSlotPosition(i));
        }
    }
}
