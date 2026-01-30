using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Customer : MonoBehaviour
{
    public enum OrderType { Burger, Garantita, Coffee }

    public OrderType orderType;

    [Header("Patience")]
    public float maxWaitTime = 30f;
    private float waitTimer;

    [Header("UI")]
    public TextMeshProUGUI orderText;

    private bool hasLeft = false;

    private NavMeshAgent agent;
    private ClientQueueMember qm;
    private TruckQueue queue;
    private CustomerSpawner spawner;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        qm = GetComponent<ClientQueueMember>();
        queue = FindFirstObjectByType<TruckQueue>();
    }

    void Start()
    {
        waitTimer = maxWaitTime;

        int count = System.Enum.GetValues(typeof(OrderType)).Length;
        orderType = (OrderType)Random.Range(0, count);

        if (orderText != null)
            orderText.text = orderType.ToString();
    }

    void Update()
    {
        if (hasLeft) return;

        // If not the FRONT customer → do not lose patience
        if (queue != null && qm != null && !queue.IsFront(qm))
            return;

        // If still walking → do not lose patience
        if (agent != null)
        {
            if (agent.pathPending) return;

            bool moving = agent.remainingDistance > agent.stoppingDistance + 0.05f;
            if (moving) return;
        }

        // Now front + standing → patience decreases
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            LeaveRestaurant();
        }
    }

    public bool ReceiveFood(OrderType food)
    {
        if (hasLeft) return false;

        if (food == orderType)
        {
            LeaveRestaurant();
            return true;
        }
        return false;
    }

    private void LeaveRestaurant()
    {
        if (hasLeft) return;
        hasLeft = true;

        if (spawner != null)
            spawner.RemoveCustomer(this);

        if (qm != null)
            qm.GoToExit();
        else
            Destroy(gameObject);
    }

    public void SetSpawner(CustomerSpawner s)
    {
        spawner = s;
    }
}
