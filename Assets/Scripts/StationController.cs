using UnityEngine;

/// <summary>
/// Controls a cooking station (Idle, Cooking, Ready, Burnt)
/// Unity 6 compatible version - Single station system
/// </summary>
public class StationController : MonoBehaviour
{
    /// <summary>
    /// States of the cooking station
    /// </summary>
    public enum StationState
    {
        Idle,       // Ready to start cooking
        Cooking,    // Currently cooking
        Ready,      // Food is ready to be picked up
        Burnt       // Food was left too long and is burnt
    }

    [Header("Food Configuration")]
    [Tooltip("The food that this station cooks")]
    public FoodData foodData;

    [Header("Station State")]
    [Tooltip("Current state of the station")]
    public StationState currentState = StationState.Idle;

    [Header("Cooking Progress")]
    [Tooltip("Current cooking timer")]
    [SerializeField] private float cookTimer;

    [Tooltip("Timer for how long food has been ready")]
    [SerializeField] private float readyTimer;

    [Header("Player Detection")]
    [Tooltip("Is the player currently near this station?")]
    public bool isPlayerNearby;

    [Header("Visual Feedback")]
    [Tooltip("Material or renderer to change color based on state")]
    public Renderer stationRenderer;

    [Header("Debug")]
    [Tooltip("Show cooking information in console")]
    public bool showDebugInfo = true;

    // Maximum cooking time (cached from foodData)
    private float maxCookTime;

    // Burn delay (cached from foodData)
    private float burnDelay;

    // Material property block for efficient color changes
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        // Initialize property block
        if (stationRenderer != null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }
    }

    private void Start()
    {
        // Validate food data
        if (foodData == null)
        {
            Debug.LogError($"StationController on {gameObject.name}: FoodData is not assigned!");
            enabled = false;
            return;
        }

        // Cache cooking parameters
        maxCookTime = foodData.cookingTime;
        burnDelay = foodData.burnDelay;

        // Update initial visual state
        UpdateVisuals();

        if (showDebugInfo)
        {
            Debug.Log($"Station initialized: {foodData.foodName} - Cook time: {maxCookTime}s");
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case StationState.Cooking:
                HandleCooking();
                break;

            case StationState.Ready:
                HandleReady();
                break;
        }

        // Update visual feedback
        UpdateVisuals();
    }

    /// <summary>
    /// Starts cooking process
    /// </summary>
    public bool StartCooking()
    {
        // Can only start cooking if station is idle
        if (currentState != StationState.Idle)
        {
            if (showDebugInfo)
            {
                Debug.Log($"Cannot start cooking - Station is {currentState}");
            }
            return false;
        }

        if (foodData == null)
        {
            Debug.LogError("Cannot start cooking - No food data assigned!");
            return false;
        }

        // Change to cooking state
        currentState = StationState.Cooking;
        cookTimer = maxCookTime;

        if (showDebugInfo)
        {
            Debug.Log($"Started cooking: {foodData.foodName} ({maxCookTime}s)");
        }

        return true;
    }

    /// <summary>
    /// Handles the cooking state
    /// </summary>
    private void HandleCooking()
    {
        // Decrease cook timer
        cookTimer -= Time.deltaTime;

        // Check if cooking is complete
        if (cookTimer <= 0f)
        {
            cookTimer = 0f;
            currentState = StationState.Ready;
            readyTimer = 0f;

            if (showDebugInfo)
            {
                Debug.Log($"Food ready: {foodData.foodName}! Pick it up within {burnDelay}s");
            }
        }
    }

    /// <summary>
    /// Handles the ready state (before burning)
    /// </summary>
    private void HandleReady()
    {
        // Increment ready timer
        readyTimer += Time.deltaTime;

        // Check if food has burnt
        if (readyTimer >= burnDelay)
        {
            currentState = StationState.Burnt;

            if (showDebugInfo)
            {
                Debug.Log($"Food burnt: {foodData.foodName}! Station needs reset.");
            }
        }
    }

    /// <summary>
    /// Attempts to take food from the station
    /// </summary>
    /// <returns>Food type if successful, null if station is not ready</returns>
    public FoodData.FoodType? TakeFood()
    {
        if (currentState != StationState.Ready)
        {
            if (showDebugInfo)
            {
                Debug.Log($"Cannot take food - Station is {currentState}");
            }
            return null;
        }

        // Get the food type
        FoodData.FoodType type = foodData.foodType;

        // Calculate timing quality for scoring
        float timingQuality = 1f - (readyTimer / burnDelay);

        if (showDebugInfo)
        {
            Debug.Log($"Food taken: {foodData.foodName} - Timing quality: {timingQuality:F2}");
        }

        // Reset the station
        ResetStation();

        return type;
    }

    /// <summary>
    /// Resets the station to idle state (also clears burnt food)
    /// </summary>
    public void ResetStation()
    {
        currentState = StationState.Idle;
        cookTimer = 0f;
        readyTimer = 0f;

        if (showDebugInfo)
        {
            Debug.Log($"Station reset: {foodData.foodName}");
        }
    }

    /// <summary>
    /// Gets cooking progress (0 to 1)
    /// </summary>
    public float GetCookProgress()
    {
        if (currentState != StationState.Cooking || maxCookTime <= 0f)
            return 0f;

        return 1f - (cookTimer / maxCookTime);
    }

    /// <summary>
    /// Gets burn progress (0 to 1) - only when Ready
    /// </summary>
    public float GetBurnProgress()
    {
        if (currentState != StationState.Ready || burnDelay <= 0f)
            return 0f;

        return readyTimer / burnDelay;
    }

    /// <summary>
    /// Updates visual feedback based on state
    /// </summary>
    private void UpdateVisuals()
    {
        if (stationRenderer == null || propertyBlock == null) return;

        Color targetColor = currentState switch
        {
            StationState.Idle => Color.gray,
            StationState.Cooking => Color.yellow,
            StationState.Ready => Color.green,
            StationState.Burnt => Color.red,
            _ => Color.white
        };

        stationRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", targetColor);
        stationRenderer.SetPropertyBlock(propertyBlock);
    }

    /// <summary>
    /// Gets the current state as a string
    /// </summary>
    public string GetStateText()
    {
        return currentState switch
        {
            StationState.Idle => "Press E to Cook",
            StationState.Cooking => $"Cooking... {cookTimer:F1}s",
            StationState.Ready => "Press E to Take",
            StationState.Burnt => "Burnt! Press R to Reset",
            _ => "Unknown"
        };
    }

    // Trigger detection for player proximity
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    // Debug visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = currentState switch
        {
            StationState.Idle => Color.gray,
            StationState.Cooking => Color.yellow,
            StationState.Ready => Color.green,
            StationState.Burnt => Color.red,
            _ => Color.white
        };

        Gizmos.DrawWireCube(transform.position + Vector3.up * 1.5f, Vector3.one * 0.3f);
    }
}
