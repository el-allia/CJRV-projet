using UnityEngine;

/// <summary>
/// Handles player interaction with stations and customers
/// Unity 6 compatible version - Single station system
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Maximum distance for raycasting to stations")]
    [Range(1f, 10f)]
    public float rayDistance = 3f;

    [Tooltip("Layer mask for cooking stations")]
    public LayerMask stationLayer;

    [Tooltip("Layer mask for customers")]
    public LayerMask customerLayer;

    [Tooltip("Radius for detecting nearby customers")]
    [Range(1f, 5f)]
    public float customerDetectionRadius = 2f;

    [Header("Current State")]
    [Tooltip("Currently targeted station")]
    public StationController currentStation;

    [Tooltip("Food currently being held")]
    public FoodData heldFood;

    [Header("Input Settings")]
    [Tooltip("Key to interact with stations")]
    public KeyCode interactKey = KeyCode.E;

    [Tooltip("Key to serve customers")]
    public KeyCode serveKey = KeyCode.F;

    [Tooltip("Key to reset burnt stations")]
    public KeyCode resetKey = KeyCode.R;

    [Header("Debug")]
    [Tooltip("Show interaction debug info")]
    public bool showDebugInfo = true;

    [Header("Visual Feedback")]
    [Tooltip("Transform where held food should appear (optional)")]
    public Transform heldFoodTransform;

    [Tooltip("Current held food visual (spawned prefab)")]
    private GameObject heldFoodVisual;

    private Camera playerCamera;

    private void Start()
    {
        // Get player camera
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("PlayerInteraction: No main camera found!");
        }
    }

    private void Update()
    {
        // Detect nearby station
        DetectStation();

        // Handle station interaction
        if (Input.GetKeyDown(interactKey))
        {
            HandleStationInteraction();
        }

        // Handle station reset (for burnt food)
        if (Input.GetKeyDown(resetKey) && currentStation != null)
        {
            if (currentStation.currentState == StationController.StationState.Burnt)
            {
                currentStation.ResetStation();
                if (showDebugInfo)
                {
                    Debug.Log("Station reset!");
                }
            }
        }

        // Handle serving customers
        if (Input.GetKeyDown(serveKey) && heldFood != null)
        {
            ServeNearestCustomer();
        }
    }

    /// <summary>
    /// Detects the station in front of the player
    /// </summary>
    private void DetectStation()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, stationLayer))
        {
            StationController station = hit.collider.GetComponent<StationController>();
            
            if (station != currentStation)
            {
                currentStation = station;
                
                if (showDebugInfo && currentStation != null)
                {
                    Debug.Log($"Targeting station: {currentStation.foodData.foodName}");
                }
            }
        }
        else
        {
            currentStation = null;
        }
    }

    /// <summary>
    /// Handles interaction with the current station
    /// </summary>
    private void HandleStationInteraction()
    {
        if (currentStation == null)
        {
            if (showDebugInfo)
            {
                Debug.Log("No station in range!");
            }
            return;
        }

        switch (currentStation.currentState)
        {
            case StationController.StationState.Idle:
                // Start cooking
                if (heldFood == null) // Can only cook if not holding food
                {
                    bool started = currentStation.StartCooking();
                    if (showDebugInfo && started)
                    {
                        Debug.Log($"Started cooking at station: {currentStation.foodData.foodName}");
                    }
                }
                else
                {
                    if (showDebugInfo)
                    {
                        Debug.Log("Cannot start cooking while holding food!");
                    }
                }
                break;

            case StationController.StationState.Ready:
                // Take the cooked food
                if (heldFood == null) // Can only take if not already holding food
                {
                    FoodData.FoodType? foodType = currentStation.TakeFood();
                    
                    if (foodType != null)
                    {
                        heldFood = currentStation.foodData;
                        CreateHeldFoodVisual();
                        
                        if (showDebugInfo)
                        {
                            Debug.Log($"Picked up food: {heldFood.foodName}");
                        }
                    }
                }
                else
                {
                    if (showDebugInfo)
                    {
                        Debug.Log("Already holding food! Serve it first.");
                    }
                }
                break;

            case StationController.StationState.Cooking:
                if (showDebugInfo)
                {
                    Debug.Log($"Food is still cooking... {currentStation.GetCookProgress() * 100f:F0}%");
                }
                break;

            case StationController.StationState.Burnt:
                if (showDebugInfo)
                {
                    Debug.Log($"Food is burnt! Press {resetKey} to reset the station.");
                }
                break;
        }
    }

    /// <summary>
    /// Creates visual representation of held food
    /// </summary>
    private void CreateHeldFoodVisual()
    {
        // Clean up existing visual
        if (heldFoodVisual != null)
        {
            Destroy(heldFoodVisual);
        }

        // Create new visual if we have a prefab
        if (heldFood != null && heldFood.cookedFoodPrefab != null && heldFoodTransform != null)
        {
            heldFoodVisual = Instantiate(
                heldFood.cookedFoodPrefab,
                heldFoodTransform.position,
                heldFoodTransform.rotation,
                heldFoodTransform
            );
        }
    }

    /// <summary>
    /// Destroys held food visual
    /// </summary>
    private void DestroyHeldFoodVisual()
    {
        if (heldFoodVisual != null)
        {
            Destroy(heldFoodVisual);
            heldFoodVisual = null;
        }
    }

    /// <summary>
    /// Serves food to the nearest customer
    /// </summary>
    private void ServeNearestCustomer()
    {
        if (heldFood == null)
        {
            if (showDebugInfo)
            {
                Debug.Log("Not holding any food!");
            }
            return;
        }

        // Find all customers within range
        Collider[] hits = Physics.OverlapSphere(transform.position, customerDetectionRadius, customerLayer);

        if (hits.Length == 0)
        {
            if (showDebugInfo)
            {
                Debug.Log("No customers nearby!");
            }
            return;
        }

        // Find nearest customer
        Transform nearestCustomer = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider col in hits)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCustomer = col.transform;
            }
        }

        // Serve the nearest customer
        if (nearestCustomer != null)
        {
            Customer customer = nearestCustomer.GetComponent<Customer>();

            if (customer != null)
            {
                // Convert FoodData.FoodType to Customer.OrderType
                Customer.OrderType orderType = (Customer.OrderType)heldFood.foodType;
                bool success = customer.ReceiveFood(orderType);

                if (showDebugInfo)
                {
                    Debug.Log(success 
                        ? $"✓ Correct order served: {heldFood.foodName}" 
                        : $"✗ Wrong order! Customer wanted {customer.orderType}, got {heldFood.foodType}");
                }

                // Clear held food after serving (regardless of correctness)
                heldFood = null;
                DestroyHeldFoodVisual();
            }
        }
    }

    /// <summary>
    /// Gets interaction prompt text for UI
    /// </summary>
    public string GetInteractionPrompt()
    {
        if (currentStation != null)
        {
            return currentStation.GetStateText();
        }

        if (heldFood != null)
        {
            return $"Holding: {heldFood.foodName} - Press {serveKey} to serve";
        }

        return "";
    }

    /// <summary>
    /// Check if player is holding food
    /// </summary>
    public bool IsHoldingFood() => heldFood != null;

    /// <summary>
    /// Get currently held food
    /// </summary>
    public FoodData GetHeldFood() => heldFood;

    // Debug visualization
    private void OnDrawGizmos()
    {
        // Draw raycast line for station detection
        Gizmos.color = currentStation != null ? Color.green : Color.red;
        Vector3 start = transform.position + Vector3.up;
        Gizmos.DrawLine(start, start + transform.forward * rayDistance);

        // Draw sphere for customer detection
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, customerDetectionRadius);
    }
}
