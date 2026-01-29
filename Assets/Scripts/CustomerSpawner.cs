using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns customers based on time of day
/// Unity 6 compatible version
/// </summary>
public class CustomerSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the TimeManager")]
    public TimeManager timeManager;
    
    [Tooltip("Customer prefab to spawn")]
    public GameObject customerPrefab;
    
    [Tooltip("Position where customers spawn")]
    public Transform spawnPoint;

    [Header("Spawn Settings")]
    [Tooltip("Maximum number of customers at once")]
    [Range(1, 20)]
    public int maxCustomers = 10;

    [Header("Debug")]
    [Tooltip("Show spawn information in console")]
    public bool showDebugInfo = true;

    // List of currently active customers
    private List<GameObject> activeCustomers = new List<GameObject>();

    // Current spawn coroutine
    private Coroutine spawnCoroutine;

    // Is spawning currently active
    private bool isSpawning = false;

    private void Start()
    {
        // Validate references
        if (!ValidateReferences())
        {
            Debug.LogError("CustomerSpawner: Missing required references! Please assign them in the Inspector.");
            enabled = false;
            return;
        }

        // Subscribe to period changes
        timeManager.OnPeriodChanged += OnPeriodChanged;

        // Start spawning customers
        StartSpawning();

        if (showDebugInfo)
        {
            Debug.Log($"CustomerSpawner initialized - Max customers: {maxCustomers}");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (timeManager != null)
        {
            timeManager.OnPeriodChanged -= OnPeriodChanged;
        }

        // Stop spawning
        StopSpawning();

        // Clean up remaining customers
        CleanupAllCustomers();
    }

    /// <summary>
    /// Validates that all required references are assigned
    /// </summary>
    private bool ValidateReferences()
    {
        bool valid = true;

        if (timeManager == null)
        {
            Debug.LogError("CustomerSpawner: TimeManager reference is missing!");
            valid = false;
        }

        if (customerPrefab == null)
        {
            Debug.LogError("CustomerSpawner: Customer prefab is missing!");
            valid = false;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("CustomerSpawner: Spawn point is missing! Using spawner position.");
            spawnPoint = transform;
        }

        return valid;
    }

    /// <summary>
    /// Called when the time period changes
    /// </summary>
    private void OnPeriodChanged(TimeManager.DayPeriod newPeriod)
    {
        if (showDebugInfo)
        {
            Debug.Log($"Period changed to {newPeriod} - Restarting spawn loop");
        }

        // Restart spawn loop with new timing
        StopSpawning();
        StartSpawning();
    }

    /// <summary>
    /// Starts the spawn coroutine
    /// </summary>
    private void StartSpawning()
    {
        if (isSpawning) return;

        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Stops the spawn coroutine
    /// </summary>
    private void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        isSpawning = false;
    }

    /// <summary>
    /// Main spawn loop - spawns customers at intervals
    /// </summary>
    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            // Wait before next spawn
            float spawnDelay = GetSpawnDelay();
            yield return new WaitForSeconds(spawnDelay);

            // Spawn customer if below max
            if (activeCustomers.Count < maxCustomers)
            {
                SpawnCustomer();
            }
            else if (showDebugInfo)
            {
                Debug.Log($"Max customers reached ({maxCustomers}), waiting for space...");
            }
        }
    }

    /// <summary>
    /// Spawns a single customer
    /// </summary>
    private void SpawnCustomer()
    {
        if (customerPrefab == null || spawnPoint == null) return;

        // Instantiate customer
        GameObject customer = Instantiate(
            customerPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        // Set spawner reference on customer
        Customer customerScript = customer.GetComponent<Customer>();
        if (customerScript != null)
        {
            customerScript.SetSpawner(this);
        }

        // Add to active list
        activeCustomers.Add(customer);

        if (showDebugInfo)
        {
            Debug.Log($"Customer spawned! Active customers: {activeCustomers.Count}/{maxCustomers}");
        }
    }

    /// <summary>
    /// Gets spawn delay based on current time period
    /// </summary>
    private float GetSpawnDelay()
    {
        TimeManager.DayPeriod currentPeriod = timeManager.GetCurrentPeriod();

        return currentPeriod switch
        {
            TimeManager.DayPeriod.Morning => Random.Range(15f, 20f),    // Slow morning
            TimeManager.DayPeriod.Noon => Random.Range(5f, 8f),         // Busy lunch rush
            TimeManager.DayPeriod.Afternoon => Random.Range(12f, 18f),  // Moderate afternoon
            _ => 10f
        };
    }

    /// <summary>
    /// Removes a customer from the active list and destroys it
    /// </summary>
    public void RemoveCustomer(GameObject customer)
    {
        if (customer == null) return;

        if (activeCustomers.Contains(customer))
        {
            activeCustomers.Remove(customer);
            
            if (showDebugInfo)
            {
                Debug.Log($"Customer removed. Active customers: {activeCustomers.Count}/{maxCustomers}");
            }
        }

        // Destroy the customer GameObject
        if (customer != null)
        {
            Destroy(customer);
        }
    }

    /// <summary>
    /// Cleans up all active customers
    /// </summary>
    private void CleanupAllCustomers()
    {
        foreach (GameObject customer in activeCustomers)
        {
            if (customer != null)
            {
                Destroy(customer);
            }
        }
        activeCustomers.Clear();
    }

    /// <summary>
    /// Get current number of active customers
    /// </summary>
    public int GetActiveCustomerCount() => activeCustomers.Count;

    /// <summary>
    /// Check if at max capacity
    /// </summary>
    public bool IsAtMaxCapacity() => activeCustomers.Count >= maxCustomers;

    // Debug visualization
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
