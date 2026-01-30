using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("References")]
    public TimeManager timeManager;
    public GameObject customerPrefab;
    public Transform spawnPoint;

    [Header("Spawn Settings")]
    [Range(1, 20)] public int maxCustomers = 10;

    [Header("Debug")]
    public bool showDebugInfo = true;

    private readonly List<Customer> activeCustomers = new List<Customer>();
    private Coroutine spawnCoroutine;
    private bool isSpawning;

    private void Start()
    {
        if (!ValidateReferences())
        {
            Debug.LogError("CustomerSpawner: Missing required references! Assign them in the Inspector.");
            enabled = false;
            return;
        }

        timeManager.OnPeriodChanged += OnPeriodChanged;
        StartSpawning();

        if (showDebugInfo)
            Debug.Log($"CustomerSpawner ready. Max customers: {maxCustomers}");
    }

    private void OnDestroy()
    {
        if (timeManager != null)
            timeManager.OnPeriodChanged -= OnPeriodChanged;

        StopSpawning();
        CleanupAllCustomers();
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        if (timeManager == null)
        {
            Debug.LogError("CustomerSpawner: TimeManager missing!");
            valid = false;
        }

        if (customerPrefab == null)
        {
            Debug.LogError("CustomerSpawner: customerPrefab missing!");
            valid = false;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("CustomerSpawner: spawnPoint missing, using spawner position.");
            spawnPoint = transform;
        }

        return valid;
    }

   private void OnPeriodChanged(TimeManager.DayPeriod newPeriod)
{
    if (showDebugInfo)
        Debug.Log($"Period changed to {newPeriod}. Spawn rate updated.");

    // DO NOTHING here
    // The loop already runs forever and will use the new delay automatically
}


    private void StartSpawning()
    {
        if (isSpawning) return;
        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    private void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        isSpawning = false;
    }

    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            // Clean nulls (customers destroyed without telling spawner)
            activeCustomers.RemoveAll(c => c == null);

            float spawnDelay = GetSpawnDelay();
            yield return new WaitForSeconds(spawnDelay);

            if (activeCustomers.Count < maxCustomers)
            {
                SpawnCustomer();
            }
            else if (showDebugInfo)
            {
                Debug.Log($"Max customers reached ({maxCustomers}). Waiting...");
            }
        }
    }

    private void SpawnCustomer()
    {
       GameObject obj = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);

Customer c = obj.GetComponent<Customer>();
if (c != null)
{
    c.SetSpawner(this);
    activeCustomers.Add(c);
}


    }

    private float GetSpawnDelay()
    {
        var currentPeriod = timeManager.GetCurrentPeriod();

        return currentPeriod switch
        {
            TimeManager.DayPeriod.Morning => Random.Range(15f, 20f),
            TimeManager.DayPeriod.Noon => Random.Range(5f, 8f),
            TimeManager.DayPeriod.Afternoon => Random.Range(12f, 18f),
            _ => 10f
        };
    }

public void RemoveCustomer(Customer customer)
{
    if (customer == null) return;

    activeCustomers.Remove(customer);

    if (showDebugInfo)
        Debug.Log($"Customer removed. Active: {activeCustomers.Count}/{maxCustomers}");

    // INSTANTLY spawn next if space available
    if (activeCustomers.Count < maxCustomers)
    {
        SpawnCustomer();
    }
}

    private void CleanupAllCustomers()
    {
        for (int i = activeCustomers.Count - 1; i >= 0; i--)
        {
            Customer c = activeCustomers[i];
            if (c != null) Destroy(c.gameObject);
        }
        activeCustomers.Clear();
    }

    public int GetActiveCustomerCount() => activeCustomers.Count;
    public bool IsAtMaxCapacity() => activeCustomers.Count >= maxCustomers;

    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
            Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 2f);
        }
    }
}
