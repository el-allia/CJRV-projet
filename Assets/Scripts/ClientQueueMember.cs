using UnityEngine;
using UnityEngine.AI;

public class ClientQueueMember : MonoBehaviour
{
    private NavMeshAgent agent;
    private TruckQueue queue;

    [Header("Exit")]
    [SerializeField] private Transform exitTarget;
    [SerializeField] private float destroyDistance = 0.25f;

    private bool goingToExit = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        queue = FindFirstObjectByType<TruckQueue>();

        // Auto-find ExitTarget by name if you didn't assign it in Inspector
        if (exitTarget == null)
        {
            GameObject go = GameObject.Find("ExitTarget");
            if (go != null) exitTarget = go.transform;
        }

        if (queue != null) queue.Join(this);
    }

    void Update()
    {
        if (!goingToExit || agent == null || exitTarget == null) return;
        if (agent.pathPending) return;

        // "Arrived" check
        if (agent.remainingDistance <= agent.stoppingDistance + destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetQueueDestination(Vector3 p)
    {
        if (agent != null) agent.SetDestination(p);
    }

    public void GoToExit()
    {
        if (exitTarget == null || agent == null) return;

        goingToExit = true;
        agent.SetDestination(exitTarget.position);
    }

    void OnDestroy()
    {
        if (queue != null) queue.Leave(this);
    }
}
