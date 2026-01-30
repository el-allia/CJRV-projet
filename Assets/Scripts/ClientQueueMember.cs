using UnityEngine;
using UnityEngine.AI;

public class ClientQueueMember : MonoBehaviour
{
    private NavMeshAgent agent;
    private TruckQueue queue;

    [SerializeField] private Transform exitTarget;
    [SerializeField] private float destroyDistance = 0.25f;

    private bool goingToExit = false;
    public bool IsGoingToExit => goingToExit;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        queue = FindFirstObjectByType<TruckQueue>();

        if (exitTarget == null)
        {
            GameObject go = GameObject.Find("ExitTarget");
            if (go != null) exitTarget = go.transform;
        }

        if (queue != null)
            queue.Join(this);
    }

    void Update()
    {
        if (!goingToExit || agent == null || exitTarget == null) return;
        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance + destroyDistance)
{
    var customer = GetComponent<Customer>();
    if (customer != null)
    {
        Destroy(customer.gameObject);
    }
}

    }

    public void SetQueueDestination(Vector3 p)
    {
        if (!goingToExit && agent != null)
            agent.SetDestination(p);
    }

    public void GoToExit()
    {
        if (goingToExit) return;
        goingToExit = true;

        if (queue != null)
        {
            TruckQueue q = queue;
            queue = null;
            q.Leave(this); // VERY IMPORTANT → line moves forward
        }

        if (exitTarget != null && agent != null)
            agent.SetDestination(exitTarget.position);
    }
}
