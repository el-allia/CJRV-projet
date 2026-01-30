using UnityEngine;

public class DebugServeCustomer : MonoBehaviour
{
    private TruckQueue queue;

    private void Start()
    {
        queue = FindFirstObjectByType<TruckQueue>();
    }

    private void Update()
    {
        // Press F to serve the first customer in queue
        if (Input.GetKeyDown(KeyCode.F))
        {
            ServeFrontCustomer();
        }
    }

    private void ServeFrontCustomer()
    {
        if (queue == null) return;

        // Find all customers in scene
        Customer[] customers = FindObjectsByType<Customer>(FindObjectsSortMode.None);

        foreach (var c in customers)
        {
            var qm = c.GetComponent<ClientQueueMember>();
            if (qm != null && queue.IsFront(qm))
            {
                Debug.Log("DEBUG: Serving front customer " + c.name);
                c.ReceiveFood(c.orderType); // auto correct food
                break;
            }
        }
    }
}
